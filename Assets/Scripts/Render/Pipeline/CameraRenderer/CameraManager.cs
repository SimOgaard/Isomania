using System;
using UnityEngine;
using static Render.Pipeline.CameraRenderer.CameraSettings;

namespace Render.Pipeline.CameraRenderer
{
    [ExecuteAlways]
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        private Transform rotationAxisSnap;
        [SerializeField]
        private float rotationSpeed;

        private const float snapIncrement = 360f / 8f;

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
            // so now, right before rendering we want to snapp the camera's pixels to our global pixel grid
            // using bottom left pixel as reference

            // reseting local position:
            camera.transform.localPosition = new Vector3(0f, 0f, -CameraDistance);

            // get bottom left pixel position
            Vector3 pixelPosition = camera.ScreenToWorldPoint(Vector3.zero);
            // remove the rotation from our pixel position
            Vector3 unrotatedPixelPosition = Quaternion.Inverse(camera.transform.rotation) * pixelPosition;
            // now we can snap it to the global pixel grid
            Vector3 roundedUnrotatedPixelPosition = RoundToPixel(unrotatedPixelPosition);
            // rotate it back to its original state after the global pixel grid snap
            Vector3 roundedCameraPosition = camera.transform.rotation * roundedUnrotatedPixelPosition;
            // offset camera position with rounded and unrounded pixel position difference to snap it to our grid
            camera.transform.position += roundedCameraPosition - pixelPosition;

            // get offset of rounded and actual camera position
            Vector3 offset = roundedUnrotatedPixelPosition - unrotatedPixelPosition;
            // transform offset world xy coordinates to pixel perfect coord using ppu
            Vector3 offsetPPU = offset * PixelsPerUnit;
            // transform from ppu to screen pixel offset
            return new Vector2(offsetPPU.x / RenderResolutionExtended.x, offsetPPU.y / RenderResolutionExtended.y);
        }

        /// <summary>
        /// Snaps <see cref="RotationAxisSnap"/> rotation to rotation grid
        /// </summary>
        private void RotationSnap()
        {
            // we assume that transform is done moving for this frame
            // so now, right before rendering we want to snapp the camera rotation axis snapper to our global rotation grid:

            // get the current euler angles
            Vector3 rotation = transform.rotation.eulerAngles;

            // create a new quaternion using the rounded values
            Quaternion snappedRotation = Quaternion.Euler(
                30f, // ignoring x since we always want it to be 30 deg
                Mathf.Round(rotation.y / RotationGrid) * RotationGrid,
                Mathf.Round(rotation.z / RotationGrid) * RotationGrid
            );

            // set the rotation to final snapped rotation
            rotationAxisSnap.rotation = snappedRotation;
        }

        private void Awake()
        {
            if (mainCamera is null)
                throw new NullReferenceException($"{nameof(mainCamera)} is null");
            if (rotationAxisSnap is null)
                throw new NullReferenceException($"{nameof(rotationAxisSnap)} is null");

            InitializeCameraSettings(mainCamera);
        }

        private void InitializeCameraSettings(Camera camera)
        {
            camera.orthographic = true;
            camera.orthographicSize = OrthographicSize;
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.magenta;
            camera.nearClipPlane = 0f;
            camera.farClipPlane = 500f;

            OnScreenResolutionPropertyChanged();
        }

        private int previousWidth, previousHeight;
        private void OnScreenResolutionPropertyChanged()
        {
            previousWidth = Screen.width; previousHeight = Screen.height;
            RecalculateCameraSettings(previousWidth, previousHeight);
            mainCamera.orthographicSize = OrthographicSize;

            float horizontal = RenderResolutionExtended.x / (PixelsPerUnit * 2f);
            float vertical = RenderResolutionExtended.y / (PixelsPerUnit * 2f);
            mainCamera.projectionMatrix = Matrix4x4.Ortho(-horizontal, horizontal, -vertical, vertical, mainCamera.nearClipPlane, mainCamera.farClipPlane);
        }

        private void Update()
        {
            if (Screen.width != previousWidth || Screen.height != previousHeight)
                OnScreenResolutionPropertyChanged();

            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }

        private void LateUpdate()
        {
            // first rotation snap the rotation axis
            RotationSnap();
            // and pixelsnap the main camera
            PixelSnap(mainCamera);
        }
    }
}