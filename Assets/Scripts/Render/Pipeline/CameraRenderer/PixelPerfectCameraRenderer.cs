using UnityEngine;
using UnityEngine.Rendering;

namespace Render.Pipeline.CameraRenderer
{
    public sealed class PixelPerfectCameraRenderer : CameraRenderer
    {
        public override void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing)
        {
            this.context = context;
            this.camera = camera;

#if UNITY_EDITOR
            PrepareBuffer();
#endif
            if (!Cull())
                return;

            Setup();
            DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
            Submit();
        }
    }
}