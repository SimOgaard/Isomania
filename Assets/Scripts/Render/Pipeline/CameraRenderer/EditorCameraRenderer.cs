using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Render.Pipeline.CameraRenderer
{
#if UNITY_EDITOR
    public sealed class EditorCameraRenderer : CameraRenderer
    {
        private static readonly Material errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        private static readonly ShaderTagId[] legacyShaderTagIds = {
            new ShaderTagId("Always"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("PrepassBase"),
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM")
        };

        public override void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing)
        {
            this.context = context;
            this.camera = camera;

            bufferSize = new Vector2Int(camera.pixelWidth, camera.pixelHeight);
            PrepareBuffer();
            PrepareForSceneWindow();
            if (!Cull())
                return;

            Setup();
            DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
            DrawUnsupportedShaders();
            DrawGizmos();
            Render();
            Submit();
            Cleanup();
        }

        protected override void Render()
        {
            buffer.Blit(colorAttachmentId, BuiltinRenderTextureType.CameraTarget);
        }

        private void DrawUnsupportedShaders()
        {
            DrawingSettings drawingSettings = new(legacyShaderTagIds[0], new SortingSettings(camera))
            {
                fallbackMaterial = errorMaterial
            };
            for (int i = 1; i < legacyShaderTagIds.Length; i++)
            {
                drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
            }
            FilteringSettings filteringSettings = FilteringSettings.defaultValue;
            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        }

        private void DrawGizmos()
        {
            if (Handles.ShouldRenderGizmos())
            {
                context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
                context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
            }
        }

        private void PrepareForSceneWindow()
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }
#endif
}