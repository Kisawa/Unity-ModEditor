/*
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ModEditor;

public class ModEditorFeature : ScriptableRendererFeature
{
    static ModEditorWindow window => ModEditorWindow.Self;

    class CustomRenderPass : ScriptableRenderPass
    {
        static readonly string RenderTag = "ModEditor";

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (window == null || !renderingData.cameraData.isSceneViewCamera)
                return;
            CommandBuffer cmd = CommandBufferPool.Get(RenderTag);
            window.Tab_SceneCollection.Command(cmd);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        name = "ModEditor";
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (window == null || !renderingData.cameraData.isSceneViewCamera)
            return;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
*/