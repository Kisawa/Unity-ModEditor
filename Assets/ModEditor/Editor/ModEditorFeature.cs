//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;
//using ModEditor;

//public class ModEditorFeature : ScriptableRendererFeature
//{
//    static ModEditorWindow window => ModEditorWindow.Self;

//    public bool GamePreview = false;

//    class CustomRenderPass : ScriptableRenderPass
//    {
//        static readonly string RenderTag = "ModEditor";

//        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//        {
//            if (window == null)
//                return;
//            CommandBuffer cmd = CommandBufferPool.Get(RenderTag);
//            //if (renderingData.cameraData.isSceneViewCamera)
//            //    window.Tab_SceneCollection.Command(cmd);
//            //else
//            //    window.Tab_SceneCollection.GameViewCommand(cmd);
//            window.Tab_SceneCollection.Command(cmd);
//            context.ExecuteCommandBuffer(cmd);
//            CommandBufferPool.Release(cmd);
//        }
//    }

//    CustomRenderPass m_ScriptablePass;

//    /// <inheritdoc/>
//    public override void Create()
//    {
//        m_ScriptablePass = new CustomRenderPass();
//        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
//        name = "ModEditor";
//    }

//    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
//    {
//        if (window == null)
//            return;
//        if (!renderingData.cameraData.isSceneViewCamera && !GamePreview)
//            return;
//        renderer.EnqueuePass(m_ScriptablePass);
//    }
//}