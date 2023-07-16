using Options;
using System;
using System.Collections;
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

        private const float rotationSnapIncrement = 360f / 8f;

        /// <summary>
        /// Rounds given Vector3 position to pixel grid
        /// </summary>
        /// <param name="position">Vector position to round</param>
        /// <returns>Rounded <see cref="Vector3"/> to </returns>
        private static Vector3 RoundToPixel(Vector3 position)
        {
            Vector3 inversedRotationPosition = inverseCameraRotation * position;

            Vector3 snapped = new Vector3(
                Mathf.Round(inversedRotationPosition.x * PixelsPerUnit) * UnitsPerPixel,
                Mathf.Round(inversedRotationPosition.y * PixelsPerUnit) * UnitsPerPixel,
                Mathf.Round(inversedRotationPosition.z * PixelsPerUnit) * UnitsPerPixel
            );

            return cameraRotation * snapped;
        }

        /// <summary>
        /// Snap <paramref name="camera"/> position to pixel grid using Camera.worldToCameraMatrix
        /// </summary>
        /// <param name="camera">Game object camera script</param>
        /// <returns><see cref="Vector2"/> containing screen pixel render offset</returns>
        private static void PixelSnap(Camera camera)
        {
            // reseting local position:
            camera.transform.localPosition = new Vector3(0f, 0f, -CameraDistance);

            // get center of bottom left pixel position because of floating point precision
            Vector3 viewPortPoint = new Vector3(
                0.5f / RenderResolutionExtended.x,
                0.5f / RenderResolutionExtended.y,
                0f
            );
            // get center of most center pixel position because of floating point precision
            Vector3 pixelPosition = camera.ViewportToWorldPoint(viewPortPoint);
            // now we can snap it to the global pixel grid
            Vector3 roundedCameraPosition = RoundToPixel(pixelPosition);
            // offset camera position with rounded and unrounded pixel position difference to snap it to our grid
            camera.transform.position += roundedCameraPosition - pixelPosition;
            /*
            // we assume that camera.transform is done moving for this frame
            // so now, right before rendering we want to snapp the camera's pixels to our global pixel grid
            // using bottom left pixel as reference

            // reseting local position:
            camera.transform.localPosition = new Vector3(0f, 0f, -CameraDistance);

            Vector3 viewPortPoint = new Vector3(
                Mathf.Round(0.5f * RenderResolutionExtended.x) / RenderResolutionExtended.x,
                Mathf.Round(0.5f * RenderResolutionExtended.y) / RenderResolutionExtended.y,
                0f
            );

            // get center of most center pixel position because of floating point precision
            Vector3 pixelPosition = camera.ViewportToWorldPoint(viewPortPoint);

            // round it to pixel grid
            camera.transform.position = RoundToPixel(pixelPosition);
            */
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
            camera.backgroundColor = Color.white;
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

        private Coroutine? rotationCoroutine = null;
        private float targetRotation; 
        private float TargetRotation
        {
            get => targetRotation;
            set
            {
                targetRotation = value;
                if (rotationCoroutine is not null)
                {
                    StopCoroutine(rotationCoroutine);
                    rotationCoroutine = null;
                }

                rotationCoroutine = StartCoroutine(SlerpRotation(targetRotation, rotationSpeed));
            }
        }
        [SerializeField]
        private float yRotation = 0.0f;
        private float YRotation
        {
            get => yRotation;
            set
            {
                // set y rotation
                yRotation = value;

                float snappedYRotation = Mathf.Round(yRotation / RotationGrid) * RotationGrid;

                // create a new quaternion using the rounded values
                cameraRotation = Quaternion.Euler(
                    30.0f, // 30 deg is isometric
                    snappedYRotation,
                    0.0f // ignoring z
                );

                // create a inverted quaternion of the rounded rotation
                inverseCameraRotation = Quaternion.Inverse(cameraRotation);

                Shader.SetGlobalMatrix(cameraRotationMatrixId, Matrix4x4.Rotate(cameraRotation));
                Shader.SetGlobalMatrix(inverseCameraRotationMatrixId, Matrix4x4.Rotate(inverseCameraRotation));

                // set the rotation to final snapped rotation
                rotationAxisSnap.rotation = cameraRotation;
            }
        }

        private static Quaternion cameraRotation;
        private static Quaternion inverseCameraRotation;
        private static readonly int cameraRotationMatrixId = Shader.PropertyToID("_CameraRotationMatrix"),
                                    inverseCameraRotationMatrixId = Shader.PropertyToID("_InverseCameraRotationMatrix");

        private void Update()
        {
            if (Screen.width != previousWidth || Screen.height != previousHeight)
                OnScreenResolutionPropertyChanged();

            if (IDevice.ConnectedDevice.RotateCameraLeft)
            {
                TargetRotation += rotationSnapIncrement;
            }
            else if (IDevice.ConnectedDevice.RotateCameraRight)
            {
                TargetRotation -= rotationSnapIncrement;
            }
        }

        private IEnumerator SlerpRotation(float endValue, float duration)
        {
            float time = 0;
            float startValue = YRotation;
            while (time < duration)
            {
                YRotation = Mathf.SmoothStep(startValue, endValue, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            YRotation = endValue.ClampRotation();
            targetRotation = YRotation;
        }

        private void LateUpdate()
        {
            // pixelsnap the main camera
            PixelSnap(mainCamera);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1,1,1,0.1f);
            const int skip = 10;

            for (int x = 0; x < RenderResolutionExtended.x; x += skip)
            {
                for (int y = 0; y < RenderResolutionExtended.y; y += skip)
                {
                    Vector3 pixelPosition = mainCamera.ViewportToWorldPoint(new Vector3((.5f + x) / RenderResolutionExtended.x, (.5f + y) / RenderResolutionExtended.y, 0f));
                    Vector3 rayDirection = mainCamera.transform.forward;
                    Gizmos.DrawRay(pixelPosition, rayDirection * 1000f);
                }
            }
        }
    }
}