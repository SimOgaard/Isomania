using UnityEngine;
using UnityEngine.Rendering;
using Render.Pipeline.CameraRenderer;

namespace Render.Pipeline.Runtime
{
    public class CustomRenderPipeline : RenderPipeline
    {
#if UNITY_EDITOR
        private readonly EditorCameraRenderer editorCameraRenderer = new();
#endif
        private bool useDynamicBatching, useGPUInstancing;

        public CustomRenderPipeline(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher)
        {
            this.useDynamicBatching = useDynamicBatching;
            this.useGPUInstancing = useGPUInstancing;
            GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        }

        private readonly PixelPerfectCameraRenderer pixelPerfectCameraRenderer = new();
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (Camera camera in cameras)
            {
#if UNITY_EDITOR
                if (camera.cameraType == CameraType.SceneView)
                {
                    editorCameraRenderer.Render(context, camera, useDynamicBatching, useGPUInstancing);
                    continue;
                }
#endif
                pixelPerfectCameraRenderer.Render(context, camera, useDynamicBatching, useGPUInstancing);
            }
        }
    }
}