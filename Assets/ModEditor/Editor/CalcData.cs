using UnityEngine;

namespace ModEditor
{
    public sealed class CalcData
    {
        public abstract class CalcDataWithVertexsBase
        {
            public Vector3[] _Vertexs;
        }

        public abstract class CalcDataWithVertexColorBase : CalcDataWithVertexsBase
        {
            public Color[] _Colors;
        }

        public abstract class CalcVertexColorWithScreenScopeBase : CalcDataWithVertexColorBase
        {
            public Vector3 _MouseTexcoord;
            public float _Size;
        }

        public class CalcVertexColorWithScreenScope : CalcVertexColorWithScreenScopeBase
        {
            public Matrix4x4 _MVP;
        }

        public class CalcVertexColorWithScreenScope_Batch : CalcVertexColorWithScreenScopeBase
        {
            public Matrix4x4[] _MVPs;
        }
    }
}