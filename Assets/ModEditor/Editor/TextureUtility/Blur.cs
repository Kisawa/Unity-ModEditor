using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class Blur : TextureUtilBase
    {
        public override string Name => "Blur";

        public override string Tip => "Gaussian Blur";

        ComputeShader drawShader;
        int kernel_Blur;

        int downSample { get => window.BlurDownSample; set => window.BlurDownSample = value; }
        int iterations { get => window.BlurIterations; set => window.BlurIterations = value; }
        int blurSpread { get => window.BlurSpread; set => window.BlurSpread = value; }

        public Blur()
        {
            drawShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/TextureUtility/Blur.compute");
            kernel_Blur = drawShader.FindKernel("Blur");
        }

        public override void Excute(RenderTexture texture)
        {
            base.Excute(texture);
            DrawUtil.Self.DownSample(texture, downSample);
            for (int i = 0; i < iterations; i++)
            {
                drawShader.SetInt("_Spread", blurSpread);
                drawShader.SetVector("_ColorMask", window.Manager.ColorMask);
                drawShader.SetTexture(kernel_Blur, "RW_Texture", texture);
                drawShader.Dispatch(kernel_Blur, Mathf.CeilToInt(texture.width / 32f), Mathf.CeilToInt(texture.height / 32f), 1);
            }
        }

        public override void Draw(GUIStyle labelStyle, float maxWidth)
        {
            base.Draw(labelStyle, maxWidth);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Down Sample:", labelStyle, GUILayout.Width(100));
            downSample = EditorGUILayout.IntSlider(downSample, 1, 5, GUILayout.Width(maxWidth - 100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Iterations:", labelStyle, GUILayout.Width(100));
            iterations = EditorGUILayout.IntSlider(iterations, 1, 25, GUILayout.Width(maxWidth - 100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Blur Spread:", labelStyle, GUILayout.Width(100));
            blurSpread = EditorGUILayout.IntSlider(blurSpread, 1, 10, GUILayout.Width(maxWidth - 100));
            EditorGUILayout.EndHorizontal();
        }
    }
}