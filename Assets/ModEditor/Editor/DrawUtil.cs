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

        public DrawUtil()
        {
            DrawShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}/Editor/Shaders/DrawUtil.compute");
            kernel_Init = DrawShader.FindKernel("Init");
        }

        public RenderTexture Init()
        {
            RenderTexture tex = new RenderTexture(1080, 1080, 0);
            tex.enableRandomWrite = true;
            tex.Create();
            DrawShader.SetTexture(kernel_Init, "Result", tex);
            DrawShader.Dispatch(kernel_Init, Mathf.CeilToInt(tex.width / 32f), Mathf.CeilToInt(tex.height / 32f), 1);
            return tex;
        }
    }
}