using UnityEngine;
using UnityEngine.Rendering;

namespace Render.Pipeline.Runtime
{
    [CreateAssetMenu(menuName = "Render/Pipeline/Custom")]
    public class CustomRenderPipelineAsset : RenderPipelineAsset
    {
        [SerializeField]
        private bool useDynamicBatching = true, useGPUInstancing = true, useSRPBatcher = true;

        protected override RenderPipeline CreatePipeline()
        {
            return new CustomRenderPipeline(useDynamicBatching, useGPUInstancing, useSRPBatcher);
        }
    }
}