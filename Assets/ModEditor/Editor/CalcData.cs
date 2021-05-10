using UnityEngine;

namespace ModEditor
{
    public sealed class CalcData
    {
        public class Cache
        {
            int spreadLevel = 0;
            public int SpreadLevel
            {
                get => spreadLevel;
                set
                {
                    if (value < 0)
                        value = 0;
                    spreadLevel = value;
                }
            }
            /// <summary>
            /// Buffer type is "int"
            /// </summary>
            public ComputeBuffer RW_Zone { get; set; }
            /// <summary>
            /// Buffer type is "float"
            /// </summary>
            public ComputeBuffer RW_Selects { get; set; }
            /// <summary>
            /// Buffer type is "float"
            /// </summary>
            public ComputeBuffer RW_Depths { get; set; }
            /// <summary>
            /// Buffer type is "float"
            /// </summary>
            public ComputeBuffer RW_Sizes { get; set; }
            /// <summary>
            /// Buffer type is "float4"
            /// </summary>
            public ComputeBuffer RW_BrushResult { get; set; }
            public virtual bool IsAvailable { get => !RW_Depths.IsValid() || !RW_Sizes.IsValid() || !RW_Selects.IsValid() || !RW_Zone.IsValid() || !RW_BrushResult.IsValid(); }
        }

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