using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class AIRenderPass : ScriptableRenderPass
{
    private RenderTargetIdentifier source;
    private RenderTargetHandle temporaryRT;
    private Material blackBlockMaterial;
    private string passTag;
    private LayerMask layerMask;

    public AIRenderPass(string tag, Material material, LayerMask mask)
    {
        passTag = tag;
        blackBlockMaterial = material;
        layerMask = mask;
        temporaryRT.Init("_TemporaryColorTexture");
    }


    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        source = renderingData.cameraData.renderer.cameraColorTargetHandle;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(passTag);

        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        opaqueDesc.depthBufferBits = 0;

        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
        DrawingSettings drawingSettings = new DrawingSettings(new ShaderTagId("Universal2D"), new SortingSettings(renderingData.cameraData.camera))
        {
            overrideMaterial = blackBlockMaterial,
        };

        cmd.GetTemporaryRT(temporaryRT.id, opaqueDesc, FilterMode.Bilinear);
        cmd.Blit(source, temporaryRT.Identifier(), blackBlockMaterial);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

        cmd.Blit(temporaryRT.Identifier(), source);
        cmd.ReleaseTemporaryRT(temporaryRT.id);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
