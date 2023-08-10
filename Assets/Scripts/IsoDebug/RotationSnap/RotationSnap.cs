using UnityEditor;
using UnityEngine;

namespace IsoDebug.RotationSnap
{
    [ExecuteAlways]
    public class RotationSnap : MonoBehaviour
    {
        [SerializeField]
        private Vector3 input;

        [SerializeField]
        private Lookup lookupRotationSnapAlgorithm = new(Color.green);
        [SerializeField]
        private RotationSnapAlgorithm[] rotationSnapAlgorithms = new RotationSnapAlgorithm[]
        {
            //new Atan2(Color.blue),
            //new Axis(Color.cyan),
            //new Cos(Color.yellow),
            //new Cos_Floor_Ceil(Color.blue),
            //new Atan2_2(Color.red),            
            //new Cos_Ceil(Color.blue),
            //new Cos_Floor(Color.red),
            //new Axis_2(Color.red),            
            //new Tan(Color.red),
            //new Spherical(Color.magenta),
            new Euler(Color.magenta),
        };

        private void Update()
        {
            input = transform.forward;

            lookupRotationSnapAlgorithm.Run(input);
            foreach (RotationSnapAlgorithm algorithm in rotationSnapAlgorithms)
                algorithm.Run(input);
        }

        #region Drawing
        private void OnDrawGizmos()
        {
            DrawArrow(Vector3.zero, input, Color.white);

            DrawVectorInfo(lookupRotationSnapAlgorithm);
            foreach (RotationSnapAlgorithm algorithm in rotationSnapAlgorithms)
                DrawVectorInfo(algorithm);

            DrawReferenceSphere();
        }

        private void DrawReferenceSphere()
        {
            Gizmos.color = Color.white;

            foreach (Vector3 value in Lookup.LookupValues)
            {
                Vector3 norm = value.normalized;
                Gizmos.DrawLine(norm, norm * 1.1f);
            }

            Gizmos.color = new Color(1, 1, 1, 0.1f);

            Gizmos.matrix = Matrix4x4.Rotate(Quaternion.AngleAxis(45, Vector3.up));
            Gizmos.DrawWireSphere(Vector3.zero, 1);

            Gizmos.matrix = Matrix4x4.Rotate(Quaternion.AngleAxis(45, Vector3.right));
            Gizmos.DrawWireSphere(Vector3.zero, 1);

            Gizmos.matrix = Matrix4x4.Rotate(Quaternion.AngleAxis(45, Vector3.forward));
            Gizmos.DrawWireSphere(Vector3.zero, 1);

            Gizmos.matrix = Matrix4x4.identity;
        }

        private void DrawVectorInfo(RotationSnapAlgorithm algorithm)
        {
            Vector3 vector = algorithm.Result;
            float angle = algorithm.ResultAngle;
            Color color = algorithm.DrawColor;
            DrawArrow(Vector3.zero, vector, color);

            Handles.Label(Vector3.Lerp(input, vector, 0.5f), (Mathf.Rad2Deg * angle).ToString(), new GUIStyle { normal = new GUIStyleState { textColor = color } });

            color.a = .2f;
            Gizmos.color = color;
            Gizmos.DrawLine(input, vector);
        }

        private static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Quaternion dir = Quaternion.LookRotation(direction, pos - SceneView.currentDrawingSceneView.camera.transform.position);

            Vector3 right = dir * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = dir * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }
        #endregion
    }
}
