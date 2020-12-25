using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public sealed class CalcUtil
    {
        static CalcUtil self;
        public static CalcUtil Self
        {
            get 
            {
                if (self == null)
                    self = new CalcUtil();
                return self;
            }
        }

        public ComputeShader CalcVertexShader { get; private set; }
        public int kernel_CalcVertexsWithScreenScope { get; private set; }
        public int kernel_WriteVertexColorUseSelectData { get; private set; }

        public CalcUtil()
        {
            CalcVertexShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}Editor/Shaders/CalcViewVertex.compute");
            kernel_CalcVertexsWithScreenScope = CalcVertexShader.FindKernel("CalcVertexsWithScreenScope");
            kernel_WriteVertexColorUseSelectData = CalcVertexShader.FindKernel("WriteVertexColorUseSelectData");
        }

        public static Color[] WriteVertexColor_UseSelectData(Color color, ComputeBuffer _Selects, Color[] originColors)
        {
            Self.CalcVertexShader.SetVector("_Color", color);
            Self.CalcVertexShader.SetBuffer(Self.kernel_WriteVertexColorUseSelectData, "RW_Selects", _Selects);
            ComputeBuffer RW_Colors = new ComputeBuffer(originColors.Length, sizeof(float) * 4);
            RW_Colors.SetData(originColors);
            Self.CalcVertexShader.SetBuffer(Self.kernel_WriteVertexColorUseSelectData, "RW_Colors", RW_Colors);
            Self.CalcVertexShader.Dispatch(Self.kernel_WriteVertexColorUseSelectData, Mathf.CeilToInt((float)originColors.Length / 1024), 1, 1);
            Color[] colors = new Color[originColors.Length];
            RW_Colors.GetData(colors);
            RW_Colors.Dispose();
            return colors;
        }
    }
}