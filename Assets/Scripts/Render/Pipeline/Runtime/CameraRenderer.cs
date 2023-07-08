using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
	public const float renderScaleMin = 0.1f, renderScaleMax = 2f;

	private const string bufferName = "Render Camera";

    private static readonly ShaderTagId unlitShaderTagId = new("SRPDefaultUnlit"),
										litShaderTagId = new("CustomLit");

	private static readonly int bufferSizeId = Shader.PropertyToID("_CameraBufferSize"),
      						    colorAttachmentId = Shader.PropertyToID("_CameraColorAttachment"),
 							    depthAttachmentId = Shader.PropertyToID("_CameraDepthAttachment"),
							    colorTextureId = Shader.PropertyToID("_CameraColorTexture"),
							    depthTextureId = Shader.PropertyToID("_CameraDepthTexture"),
							    sourceTextureId = Shader.PropertyToID("_SourceTexture"),
							    srcBlendId = Shader.PropertyToID("_CameraSrcBlend"),
							    dstBlendId = Shader.PropertyToID("_CameraDstBlend");

	private static readonly CameraSettings defaultCameraSettings = new();

	private static readonly bool copyTextureSupported = SystemInfo.copyTextureSupport > CopyTextureSupport.None;

    private readonly CommandBuffer buffer = new()
	{
		name = bufferName
	};

    private ScriptableRenderContext context;

    private Camera camera;

    private CullingResults cullingResults;

    private Lighting lighting = new();

    private PostFXStack postFXStack = new();

    private bool useColorTexture, useDepthTexture, useIntermediateBuffer;

    private Vector2Int bufferSize;

    private readonly Material material;

    private readonly Texture2D missingTexture;

	public CameraRenderer(Shader shader)
	{
		material = CoreUtils.CreateEngineMaterial(shader);
		missingTexture = new(1, 1, TextureFormat.R8, false)
		{
			hideFlags = HideFlags.HideAndDontSave,
			name = "Missing"
		};
		missingTexture.SetPixel(0, 0, Color.white * 0.5f);
		missingTexture.Apply(true, true);
	}

	public void Dispose ()
	{
		CoreUtils.Destroy(material);
		CoreUtils.Destroy(missingTexture);
	}

	public void Render(
		ScriptableRenderContext context, Camera camera,
		CameraBufferSettings bufferSettings,
		bool useDynamicBatching, bool useGPUInstancing, bool useLightsPerObject,
		ShadowSettings shadowSettings, PostFXSettings postFXSettings,
		int colorLUTResolution)
	{
		this.context = context;
		this.camera = camera;

        CustomRenderPipelineCamera crpCamera = camera.GetComponent<CustomRenderPipelineCamera>();
		CameraSettings cameraSettings = crpCamera ? crpCamera.Settings : defaultCameraSettings;

		if (camera.cameraType == CameraType.Reflection)
		{
			useColorTexture = bufferSettings.CopyColorReflection;
			useDepthTexture = bufferSettings.CopyDepthReflection;
		}
		else
		{
			useColorTexture = bufferSettings.CopyColor && cameraSettings.copyColor;
			useDepthTexture = bufferSettings.CopyDepth && cameraSettings.copyDepth;
		}

		if (cameraSettings.overridePostFX)
		{
			postFXSettings = cameraSettings.postFXSettings;
		}

		PrepareBuffer();
        PrepareForSceneWindow();
		if (!Cull(shadowSettings.maxDistance))
		{
			return;
		}
		if (camera.cameraType != CameraType.SceneView)
		{
            bufferSize = bufferSettings.BufferSize;
        }

        buffer.BeginSample(SampleName);
		buffer.SetGlobalVector(bufferSizeId, new Vector4(
            1f / bufferSize.x, 1f / bufferSize.y,
            bufferSize.x, bufferSize.y
		));
		ExecuteBuffer();
		lighting.Setup(
			context, cullingResults, shadowSettings, useLightsPerObject,
			cameraSettings.maskLights ? cameraSettings.renderingLayerMask : -1
		);
		postFXStack.Setup(
			context, camera, bufferSize, postFXSettings, colorLUTResolution,
			cameraSettings.finalBlendMode
		);
		buffer.EndSample(SampleName);
		Setup();
		DrawVisibleGeometry(
			useDynamicBatching, useGPUInstancing, useLightsPerObject,
			cameraSettings.renderingLayerMask
		);
		DrawUnsupportedShaders();
		DrawGizmosBeforeFX();
		if (postFXStack.IsActive)
		{
			postFXStack.Render(colorAttachmentId);
		}
		else if (useIntermediateBuffer)
		{
			DrawFinal(cameraSettings.finalBlendMode);
			ExecuteBuffer();
		}
		DrawGizmosAfterFX();
		Cleanup();
		Submit();
	}

	private bool Cull(float maxShadowDistance)
	{
		if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
		{
			p.shadowDistance = Mathf.Min(maxShadowDistance, camera.farClipPlane);
			cullingResults = context.Cull(ref p);
			return true;
		}
		return false;
	}

	private void Setup()
	{
		context.SetupCameraProperties(camera);
		CameraClearFlags flags = camera.clearFlags;

		if (flags > CameraClearFlags.Color)
		{
			flags = CameraClearFlags.Color;
		}
		buffer.GetTemporaryRT(
			colorAttachmentId, bufferSize.x, bufferSize.y,
			0, FilterMode.Point, RenderTextureFormat.DefaultHDR
		);
		buffer.GetTemporaryRT(
			depthAttachmentId, bufferSize.x, bufferSize.y,
			32, FilterMode.Point, RenderTextureFormat.Depth
		);
		buffer.SetRenderTarget(
			colorAttachmentId,
			RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
			depthAttachmentId,
			RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
		);
		
		buffer.ClearRenderTarget(
			flags <= CameraClearFlags.Depth,
			flags == CameraClearFlags.Color,
			flags == CameraClearFlags.Color ? camera.backgroundColor.linear : Color.clear
		);
		buffer.BeginSample(SampleName);
		buffer.SetGlobalTexture(colorTextureId, missingTexture);
		buffer.SetGlobalTexture(depthTextureId, missingTexture);
		ExecuteBuffer();
	}

	private void Cleanup()
	{
		lighting.Cleanup();
		if (useIntermediateBuffer) {
			buffer.ReleaseTemporaryRT(colorAttachmentId);
			buffer.ReleaseTemporaryRT(depthAttachmentId);
			if (useColorTexture) {
				buffer.ReleaseTemporaryRT(colorTextureId);
			}
			if (useDepthTexture) {
				buffer.ReleaseTemporaryRT(depthTextureId);
			}
		}
	}

	private void Submit()
	{
		buffer.EndSample(SampleName);
		ExecuteBuffer();
		context.Submit();
	}

	private void ExecuteBuffer()
	{
		context.ExecuteCommandBuffer(buffer);
		buffer.Clear();
	}

	private void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing, bool useLightsPerObject, int renderingLayerMask)
	{
		PerObjectData lightsPerObjectFlags = useLightsPerObject ? PerObjectData.LightData | PerObjectData.LightIndices : PerObjectData.None;
        SortingSettings sortingSettings = new(camera)
		{
			criteria = SortingCriteria.CommonOpaque
		};
        DrawingSettings drawingSettings = new(unlitShaderTagId, sortingSettings)
		{
			enableDynamicBatching = useDynamicBatching,
			enableInstancing = useGPUInstancing,
			perObjectData =
				PerObjectData.ReflectionProbes |
				PerObjectData.Lightmaps | PerObjectData.ShadowMask |
				PerObjectData.LightProbe | PerObjectData.OcclusionProbe |
				PerObjectData.LightProbeProxyVolume |
				PerObjectData.OcclusionProbeProxyVolume |
				lightsPerObjectFlags
		};
		drawingSettings.SetShaderPassName(1, litShaderTagId);

        FilteringSettings filteringSettings = new(RenderQueueRange.opaque, renderingLayerMask: (uint)renderingLayerMask);

		context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

		context.DrawSkybox(camera);
		if (useColorTexture || useDepthTexture)
		{
			CopyAttachments();
		}

		sortingSettings.criteria = SortingCriteria.CommonTransparent;
		drawingSettings.sortingSettings = sortingSettings;
		filteringSettings.renderQueueRange = RenderQueueRange.transparent;

		context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
	}

	private void CopyAttachments()
	{
		if (useColorTexture)
		{
			buffer.GetTemporaryRT(
				colorTextureId, bufferSize.x, bufferSize.y,
				0, FilterMode.Point, RenderTextureFormat.DefaultHDR
			);
			if (copyTextureSupported)
			{
				buffer.CopyTexture(colorAttachmentId, colorTextureId);
			}
			else
			{
				Draw(colorAttachmentId, colorTextureId);
			}
		}
		if (useDepthTexture)
		{
			buffer.GetTemporaryRT(
				depthTextureId, bufferSize.x, bufferSize.y,
				32, FilterMode.Point, RenderTextureFormat.Depth
			);
			if (copyTextureSupported)
			{
				buffer.CopyTexture(depthAttachmentId, depthTextureId);
			}
			else
			{
				Draw(depthAttachmentId, depthTextureId, true);
			}
		}
		if (!copyTextureSupported)
		{
			buffer.SetRenderTarget(
				colorAttachmentId,
				RenderBufferLoadAction.Load, RenderBufferStoreAction.Store,
				depthAttachmentId,
				RenderBufferLoadAction.Load, RenderBufferStoreAction.Store
			);
		}
		ExecuteBuffer();
	}

	private void Draw(RenderTargetIdentifier from, RenderTargetIdentifier to, bool isDepth = false)
	{
		buffer.SetGlobalTexture(sourceTextureId, from);
		buffer.SetRenderTarget(to, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
		buffer.SetViewport(camera.pixelRect);
		buffer.DrawProcedural(Matrix4x4.identity, material, isDepth ? 1 : 0, MeshTopology.Triangles, 3);
	}

	private void DrawFinal(CameraSettings.FinalBlendMode finalBlendMode)
	{
		buffer.SetGlobalFloat(srcBlendId, (float)finalBlendMode.source);
		buffer.SetGlobalFloat(dstBlendId, (float)finalBlendMode.destination);
		buffer.SetGlobalTexture(sourceTextureId, colorAttachmentId);
		buffer.SetRenderTarget(
			BuiltinRenderTextureType.CameraTarget,
			finalBlendMode.destination == BlendMode.Zero ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load,
			RenderBufferStoreAction.Store
		);
		buffer.SetViewport(camera.pixelRect);
		buffer.DrawProcedural(Matrix4x4.identity, material, 0, MeshTopology.Triangles, 3);
		buffer.SetGlobalFloat(srcBlendId, 1f);
		buffer.SetGlobalFloat(dstBlendId, 0f);
	}
}