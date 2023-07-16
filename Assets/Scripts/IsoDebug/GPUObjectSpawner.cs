using Render;
using UnityEngine;

namespace IsoDebug
{
    [ExecuteAlways]
    public class GPUObjectSpawner : MonoBehaviour
    {
        private static readonly int baseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField]
        private Mesh mesh = default;

        [SerializeField]
        private Material material = default;

        [SerializeField]
        private float radius = 10f;

        private readonly Matrix4x4[] matrices = new Matrix4x4[1023];
        private readonly Vector4[] baseColors = new Vector4[1023];

        private MaterialPropertyBlock block;
        private void Awake()
        {
            for (int i = 0; i < matrices.Length; i++)
            {
                matrices[i] = Matrix4x4.TRS(Random.insideUnitSphere * radius, Quaternion.identity, Vector3.one);
                baseColors[i] = new Vector4(Random.value, Random.value, Random.value, 1f);
            }

            block = new MaterialPropertyBlock();
            block.SetVectorArray(baseColorId, baseColors);
        }

        private void Update()
        {
            Graphics.DrawMeshInstanced(mesh, 0, material, matrices, 1023, block);
        }
    }
}