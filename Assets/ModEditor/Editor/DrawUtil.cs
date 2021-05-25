using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public sealed class DrawUtil
    {
        static DrawUtil self;
        public static DrawUtil Self
        {
            get
            {
                if (self == null)
                    self = new DrawUtil();
                return self;
            }
        }

        public ComputeShader DrawShader { get; private set; }
        public int kernel_Init { get; private set; }
        public int kernel_Merge { get; private set; }

        public DrawUtil()
        {
            DrawShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/Shaders/DrawUtil.compute");
            kernel_Init = DrawShader.FindKernel("Init");
            kernel_Merge = DrawShader.FindKernel("Merge");
        }

        public void Init(Color baseColor, out RenderTexture baseTexture, out RenderTexture drawTexture, out RenderTexture texture)
        {
            baseTexture = new RenderTexture(1080, 1080, 0);
            baseTexture.enableRandomWrite = true;
            baseTexture.Create();
            drawTexture = new RenderTexture(1080, 1080, 0);
            drawTexture.enableRandomWrite = true;
            drawTexture.Create();
            texture = new RenderTexture(1080, 1080, 0);
            texture.enableRandomWrite = true;
            texture.Create();
            DrawShader.SetVector("_BaseColor", baseColor);
            DrawShader.SetTexture(kernel_Init, "RW_Texture", baseTexture);
            DrawShader.Dispatch(kernel_Init, Mathf.CeilToInt(baseTexture.width / 32f), Mathf.CeilToInt(baseTexture.height / 32f), 1);
            MergeDraw(baseTexture, drawTexture, texture);
        }

        public void ChangeBase(Color baseColor, RenderTexture baseTexture, RenderTexture drawTexture, RenderTexture texture)
        {
            DrawShader.SetVector("_BaseColor", baseColor);
            DrawShader.SetTexture(kernel_Init, "RW_Texture", baseTexture);
            DrawShader.Dispatch(kernel_Init, Mathf.CeilToInt(baseTexture.width / 32f), Mathf.CeilToInt(baseTexture.height / 32f), 1);
            MergeDraw(baseTexture, drawTexture, texture);
        }

        public void MergeDraw(RenderTexture baseTexture, RenderTexture drawTexture, RenderTexture texture)
        {
            DrawShader.SetTexture(kernel_Merge, "RW_BackgroundTexture", baseTexture);
            DrawShader.SetTexture(kernel_Merge, "RW_ForegroundTexture", drawTexture);
            DrawShader.SetTexture(kernel_Merge, "RW_Texture", texture);
            DrawShader.Dispatch(kernel_Merge, Mathf.CeilToInt(texture.width / 32f), Mathf.CeilToInt(texture.height / 32f), 1);
        }
    }
}