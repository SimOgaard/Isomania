using UnityEngine;
using UnityEngine.Rendering;

namespace Render.Pipeline.CameraRenderer
{
    public abstract class CameraRenderer
    {
        protected static readonly ShaderTagId unlitShaderTagId = new("SRPDefaultUnlit");
        protected static readonly int bufferSizeId = Shader.PropertyToID("_CameraBufferSize"),
                                      colorAttachmentId = Shader.PropertyToID("_CameraColorAttachment"),
 							          depthAttachmentId = Shader.PropertyToID("_CameraDepthAttachment"),
							          colorTextureId = Shader.PropertyToID("_CameraColorTexture"),
							          depthTextureId = Shader.PropertyToID("_CameraDepthTexture"),
							          sourceTextureId = Shader.PropertyToID("_SourceTexture");

        protected ScriptableRenderContext context;
        protected Camera camera;

        protected Vector2Int bufferSize;
        protected const string bufferName = "Camera Renderer";
        protected readonly CommandBuffer buffer = new() { name = bufferName };
#if UNITY_EDITOR
        protected void PrepareBuffer()
        {
            buffer.name = camera.name;
        }
#endif

        public abstract void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing);

        protected void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing)
        {
            SortingSettings sortingSettings = new(camera)
            {
                criteria = SortingCriteria.CommonOpaque
            };
            DrawingSettings drawingSettings = new(unlitShaderTagId, sortingSettings)
            {
                enableDynamicBatching = useDynamicBatching,
                enableInstancing = useGPUInstancing
            };
            FilteringSettings filteringSettings = new(RenderQueueRange.opaque);

            context.DrawRenderers(
                cullingResults, ref drawingSettings, ref filteringSettings
            );

            context.DrawSkybox(camera);

            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;

            context.DrawRenderers(
                cullingResults, ref drawingSettings, ref filteringSettings
            );
        }

        protected void Setup()
        {
            context.SetupCameraProperties(camera);
            CameraClearFlags flags = camera.clearFlags;


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
#if UNITY_EDITOR
            buffer.BeginSample(buffer.name); // will gc, (required for editor frame debugging)
#else
            buffer.BeginSample(bufferName); // wont gc
#endif
            buffer.SetGlobalVector(bufferSizeId, new Vector4(
                1f / bufferSize.x, 1f / bufferSize.y,
                bufferSize.x, bufferSize.y
            ));
            ExecuteBuffer();
        }

        protected void Cleanup()
        {
            buffer.ReleaseTemporaryRT(colorAttachmentId);
            buffer.ReleaseTemporaryRT(depthAttachmentId);
        }

        protected void Render()
        {
            buffer.Blit(colorAttachmentId, BuiltinRenderTextureType.CameraTarget);
        }

        protected void Submit()
        {
#if UNITY_EDITOR
            buffer.EndSample(buffer.name); // will gc, (required for editor frame debugging)
#else
            buffer.EndSample(bufferName); // wont gc
#endif

            ExecuteBuffer();
            context.Submit();
        }

        private void ExecuteBuffer()
        {
            context.ExecuteCommandBuffer(buffer);
            buffer.Clear();
        }

        protected CullingResults cullingResults;
        protected bool Cull()
        {
            if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
            {
                cullingResults = context.Cull(ref p);
                return true;
            }
            return false;
        }
    }
}