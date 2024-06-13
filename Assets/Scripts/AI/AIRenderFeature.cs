using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

//AIが扱うカメラに利用するRenderFeature
public class AIRenderFeature : ScriptableRendererFeature
{
    [SerializeField] Material blackBlockMaterial;
    AIRenderPass renderPass;

    public override void Create()
    {
        LayerMask layerMask = (1 << LayerMask.NameToLayer("PrimeNumberBlock")) | (1 << LayerMask.NameToLayer("Ground"));
        renderPass = new AIRenderPass("AIEffect", blackBlockMaterial, layerMask);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(renderPass);
    }
}
