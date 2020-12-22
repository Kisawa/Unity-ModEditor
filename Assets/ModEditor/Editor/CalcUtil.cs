using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public sealed class CalcUtil
    {
        static CalcUtil self;
        static CalcUtil Self
        {
            get 
            {
                if (self == null)
                    self = new CalcUtil();
                return self;
            }
        }

        ComputeShader calcVertexShader;
        int kernel_writeVertexColorWithPenetrate;
        int kernel_writeVertexColorWithDepthCull;

        public CalcUtil()
        {
            calcVertexShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}Editor/Shaders/CalcViewVertex.compute");
            kernel_writeVertexColorWithPenetrate = calcVertexShader.FindKernel("WriteVertexColorWithPenetrate");
            kernel_writeVertexColorWithDepthCull = calcVertexShader.FindKernel("WriteVertexColorWithDepthCull");
        }

        public static Color[] WriteVertexColor_Penetrate(CalcData.CalcVertexColorWithPenetrate data, Color color)
        {
            Self.calcVertexShader.SetVector("_MouseTexcoord", data._MouseTexcoord);
            Self.calcVertexShader.SetFloat("_Size", data._Size);
            ComputeBuffer _MVP = new ComputeBuffer(data._MVP.Length, sizeof(float) * 16);
            _MVP.SetData(data._MVP);
            Self.calcVertexShader.SetBuffer(Self.kernel_writeVertexColorWithPenetrate, "_MVP", _MVP);
            Self.calcVertexShader.SetVector("_Color", color);
            ComputeBuffer _Vertexs = new ComputeBuffer(data._Vertexs.Length, sizeof(float) * 3);
            _Vertexs.SetData(data._Vertexs);
            Self.calcVertexShader.SetBuffer(Self.kernel_writeVertexColorWithPenetrate, "_Vertexs", _Vertexs);
            ComputeBuffer RW_Colors = new ComputeBuffer(data._Vertexs.Length, sizeof(float) * 4);
            RW_Colors.SetData(data._Colors);
            Self.calcVertexShader.SetBuffer(Self.kernel_writeVertexColorWithPenetrate, "RW_Colors", RW_Colors);
            Self.calcVertexShader.Dispatch(Self.kernel_writeVertexColorWithPenetrate, Mathf.CeilToInt((float)data._Vertexs.Length / 1024), 1, 1);
            Color[] colors = new Color[data._Vertexs.Length];
            RW_Colors.GetData(colors);
            _Vertexs.Dispose();
            _MVP.Dispose();
            RW_Colors.Dispose();
            return colors;
        }

        public static Color[] WriteVertexColor_DepthCull(CalcData.CalcVertexColorWithDepthCull data, Color color)
        {
            Self.calcVertexShader.SetVector("_MouseTexcoord", data._MouseTexcoord);
            Self.calcVertexShader.SetFloat("_Size", data._Size);
            ComputeBuffer _MV = new ComputeBuffer(data._MV.Length, sizeof(float) * 16);
            _MV.SetData(data._MV);
            Self.calcVertexShader.SetBuffer(Self.kernel_writeVertexColorWithDepthCull, "_MV", _MV);
            Self.calcVertexShader.SetMatrix("_P", data._P);
            Self.calcVertexShader.SetVector("_Color", color);
            Self.calcVertexShader.SetTexture(Self.kernel_writeVertexColorWithDepthCull, "_DepthMap", data._DepthMap);
            ComputeBuffer _Vertexs = new ComputeBuffer(data._Vertexs.Length, sizeof(float) * 3);
            _Vertexs.SetData(data._Vertexs);
            Self.calcVertexShader.SetBuffer(Self.kernel_writeVertexColorWithDepthCull, "_Vertexs", _Vertexs);
            ComputeBuffer RW_Colors = new ComputeBuffer(data._Vertexs.Length, sizeof(float) * 4);
            RW_Colors.SetData(data._Colors);
            Self.calcVertexShader.SetBuffer(Self.kernel_writeVertexColorWithDepthCull, "RW_Colors", RW_Colors);
            Self.calcVertexShader.Dispatch(Self.kernel_writeVertexColorWithDepthCull, Mathf.CeilToInt((float)data._Vertexs.Length / 1024), 1, 1);
            Color[] colors = new Color[data._Vertexs.Length];
            RW_Colors.GetData(colors);
            _Vertexs.Dispose();
            _MV.Dispose();
            RW_Colors.Dispose();
            return colors;
        }
    }
}