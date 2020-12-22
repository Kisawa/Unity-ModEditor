using UnityEngine;

namespace ModEditor
{
    public static class CalcData
    {
        public abstract class CalcWithScreenScopeBase
        {
            public Vector3 _MouseTexcoord;
            public float _Size;
        }

        public abstract class CalcDataWithVertexsBase : CalcWithScreenScopeBase
        {
            public Vector3[] _Vertexs;
        }

        public abstract class CalcDataWithVertexColorBase : CalcDataWithVertexsBase
        {
            public Color[] _Colors;
        }

        public class CalcVertexColorWithPenetrate : CalcDataWithVertexColorBase
        {
            public Matrix4x4[] _MVP;
        }

        public class CalcVertexColorWithDepthCull : CalcDataWithVertexColorBase
        {
            public Matrix4x4[] _MV;
            public Matrix4x4 _P;
            public RenderTexture _DepthMap;
        }
    }
}