using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class AIRenderPass : ScriptableRenderPass
{
    private Material blackBlockMaterial;
    private string passTag;
    private LayerMask layerMask;
    private FilteringSettings filteringSettings;

    public AIRenderPass(string tag, Material material, LayerMask mask)
    {
        passTag = tag;
        blackBlockMaterial = material;
        layerMask = mask;
        filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
        renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(passTag);

        DrawingSettings drawingSettings = new DrawingSettings(new ShaderTagId("Universal2D"), new SortingSettings(renderingData.cameraData.camera))
        {
            overrideMaterial = blackBlockMaterial,
        };

        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;  

        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
