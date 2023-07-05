using UnityEngine;
using UnityEngine.Rendering;
using static Render.Pipeline.CameraRenderer.CameraSettings;

namespace Render.Pipeline.CameraRenderer
{
    public sealed class PixelPerfectCameraRenderer : CameraRenderer
    {
        /// <summary>
        /// Rounds given Vector3 position to pixel grid
        /// </summary>
        /// <param name="position">Vector position to round</param>
        /// <returns>Rounded <see cref="Vector3"/> to </returns>
        private static Vector3 RoundToPixel(Vector3 position)
        {
            return new Vector3(
                Mathf.Round(position.x * PixelsPerUnit) * UnitsPerPixel,
                Mathf.Round(position.y * PixelsPerUnit) * UnitsPerPixel,
                Mathf.Round(position.z * PixelsPerUnit) * UnitsPerPixel
            );
        }

        /// <summary>
        /// Snap <paramref name="camera"/> position to pixel grid using Camera.worldToCameraMatrix
        /// </summary>
        /// <param name="camera">Game object camera script</param>
        /// <returns><see cref="Vector2"/> containing screen pixel render offset</returns>
        private static Vector2 PixelSnap(Camera camera)
        {
            // we assume that camera.transform is done moving for this frame
            // so now, right before rendering we want to snapp the camera to our global pixel grid:

            // first reset local position to camera distance
            camera.transform.localPosition = new Vector3(CameraDistanceX, CameraDistanceY, CameraDistanceZ);

            // to snap to global pixel grid we have to first remove the rotation from our cameras world position
            Vector3 unrotatedCameraPosition = Quaternion.Inverse(camera.transform.rotation) * camera.transform.position;
            // now we can snap it to the global pixel grid
            Vector3 roundedUnrotatedCameraPosition = RoundToPixel(unrotatedCameraPosition);
            // rotate it back to its original state after the global pixel grid snap
            Vector3 roundedCameraPosition = camera.transform.rotation * roundedUnrotatedCameraPosition;
            // set camera position to snapped position
            camera.transform.position = roundedCameraPosition;

            // get offset of rounded and actual camera position
            Vector3 offset = roundedUnrotatedCameraPosition - unrotatedCameraPosition;
            // transform offset world xy coordinates to pixel perfect coord using ppu
            Vector3 offsetPPU = offset * PixelsPerUnit;
            // transform from ppu to screen pixel offset
            return new Vector2(offsetPPU.x / RenderResolutionExtended.x, offsetPPU.y / RenderResolutionExtended.y);
        }

        public override void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing)
        {
            this.context = context;
            this.camera = camera;

#if UNITY_EDITOR
            PrepareBuffer();
#endif

            PixelSnap(camera);

            if (!Cull())
                return;

            Setup();
            DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
            Submit();
        }
    }
}