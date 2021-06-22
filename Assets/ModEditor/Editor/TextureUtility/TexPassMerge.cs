using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ModEditor
{
    public class TexPassMerge : TextureUtilBase
    {
        public override string Name => "Texture Pass Merge";

        public override string Tip => "Extract color channels and merge them into a single image";

        public override bool IsAvailable => texture1 != null || texture2 != null || texture3 != null || texture4 != null;

        public override bool OnlyCustom => true;

        Texture texture1
        {
            get => window.TexPassMergeTex1; 
            set
            {
                if (window.TexPassMergeTex1 == value)
                    return;
                window.TexPassMergeTex1 = value;
                panel1.BindTexture(value);
            }
        }
        TexturePanel panel1 { get => window.TexPassMergePanel1; }

        Texture texture2
        {
            get => window.TexPassMergeTex2;
            set
            {
                if (window.TexPassMergeTex2 == value)
                    return;
                window.TexPassMergeTex2 = value;
                panel2.BindTexture(value);
            }
        }
        TexturePanel panel2 { get => window.TexPassMergePanel2; }

        Texture texture3
        {
            get => window.TexPassMergeTex3;
            set
            {
                if (window.TexPassMergeTex3 == value)
                    return;
                window.TexPassMergeTex3 = value;
                panel3.BindTexture(value);
            }
        }
        TexturePanel panel3 { get => window.TexPassMergePanel3; }

        Texture texture4
        {
            get => window.TexPassMergeTex4;
            set
            {
                if (window.TexPassMergeTex4 == value)
                    return;
                window.TexPassMergeTex4 = value;
                panel4.BindTexture(value);
            }
        }
        TexturePanel panel4 { get => window.TexPassMergePanel4; }

        ComputeShader drawShader;
        int kernel_Merge;

        public TexPassMerge()
        {
            drawShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/TextureUtility/TexPassMerge.compute");
            kernel_Merge = drawShader.FindKernel("Merge");
            Undo.undoRedoPerformed += undoRedoPerformed;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Undo.undoRedoPerformed -= undoRedoPerformed;
        }

        void undoRedoPerformed()
        {
            panel1.BindTexture(texture1);
            panel2.BindTexture(texture2);
            panel3.BindTexture(texture3);
            panel4.BindTexture(texture4);
        }

        public override void Excute(RenderTexture texture)
        {
            base.Excute(texture);
            drawShader.SetInt("_Pass1", texture1 == null ? -1 : (int)panel1.ViewPass);
            drawShader.SetInt("_Pass2", texture2 == null ? -1 : (int)panel2.ViewPass);
            drawShader.SetInt("_Pass3", texture3 == null ? -1 : (int)panel3.ViewPass);
            drawShader.SetInt("_Pass4", texture4 == null ? -1 : (int)panel4.ViewPass);
            drawShader.SetVector("_ColorMask", window.Manager.ColorMask);
            RenderTexture renderTexture1 = RenderTexture.GetTemporary(texture.descriptor);
            renderTexture1.enableRandomWrite = true;
            renderTexture1.Create();
            if(texture1 != null)
                Graphics.Blit(texture1, renderTexture1);
            RenderTexture renderTexture2 = RenderTexture.GetTemporary(texture.descriptor);
            renderTexture2.enableRandomWrite = true;
            renderTexture2.Create();
            if (texture2 != null)
                Graphics.Blit(texture2, renderTexture2);
            RenderTexture renderTexture3 = RenderTexture.GetTemporary(texture.descriptor);
            renderTexture3.enableRandomWrite = true;
            renderTexture3.Create();
            if (texture3 != null)
                Graphics.Blit(texture3, renderTexture3);
            RenderTexture renderTexture4 = RenderTexture.GetTemporary(texture.descriptor);
            renderTexture4.enableRandomWrite = true;
            renderTexture4.Create();
            if (texture4 != null)
                Graphics.Blit(texture4, renderTexture4);
            drawShader.SetTexture(kernel_Merge, "RW_Texture", texture);
            drawShader.SetTexture(kernel_Merge, "RW_Origin1", renderTexture1);
            drawShader.SetTexture(kernel_Merge, "RW_Origin2", renderTexture2);
            drawShader.SetTexture(kernel_Merge, "RW_Origin3", renderTexture3);
            drawShader.SetTexture(kernel_Merge, "RW_Origin4", renderTexture4);
            drawShader.Dispatch(kernel_Merge, Mathf.CeilToInt(texture.width / 32f), Mathf.CeilToInt(texture.height / 32f), 1);
            RenderTexture.ReleaseTemporary(renderTexture1);
            RenderTexture.ReleaseTemporary(renderTexture2);
            RenderTexture.ReleaseTemporary(renderTexture3);
            RenderTexture.ReleaseTemporary(renderTexture4);
        }

        public override void Draw(GUIStyle labelStyle, float maxWidth)
        {
            base.Draw(labelStyle, maxWidth);
            float halfWidth = (maxWidth - 20) / 2;

            int preIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("R Pass:", labelStyle, GUILayout.Width(halfWidth));
            texture1 = (Texture)EditorGUILayout.ObjectField(texture1, typeof(Texture), false, GUILayout.Width(halfWidth));
            panel1.Draw(halfWidth, false);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("G Pass:", labelStyle, GUILayout.Width(halfWidth));
            texture2 = (Texture)EditorGUILayout.ObjectField(texture2, typeof(Texture), false, GUILayout.Width(halfWidth));
            panel2.Draw(halfWidth, false);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("B Pass:", labelStyle, GUILayout.Width(halfWidth));
            texture3 = (Texture)EditorGUILayout.ObjectField(texture3, typeof(Texture), false, GUILayout.Width(halfWidth));
            panel3.Draw(halfWidth, false);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("A Pass:", labelStyle, GUILayout.Width(halfWidth));
            texture4 = (Texture)EditorGUILayout.ObjectField(texture4, typeof(Texture), false, GUILayout.Width(halfWidth));
            panel4.Draw(halfWidth, false);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = preIndentLevel;
        }
    }
}