using UnityEngine;
using UnityEngine.Rendering;
using static Render.Pipeline.CameraRenderer.CameraSettings;

namespace Render.Pipeline.CameraRenderer
{
    public sealed class PixelPerfectCameraRenderer : CameraRenderer
    {
        public override void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing)
        {
            this.context = context;
            this.camera = camera;

            bufferSize = RenderResolutionExtended;
#if UNITY_EDITOR
            PrepareBuffer();
#endif
            if (!Cull())
                return;

            Setup();
            DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
            Render();
            Submit();
            Cleanup();
        }
    }
}