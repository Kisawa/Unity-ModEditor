using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModEditor
{
    public abstract class VertexBrushUtilBase
    {
        public abstract string Name { get; }

        public abstract string Tip { get; }

        public VertexBrushUtilBase() { }

        public WriteTargetType TargetType;

        public CustomTargetType CustomTarget_X;

        public CustomTargetType CustomTarget_Y;

        public CustomTargetType CustomTarget_Z;

        public CustomTargetType CustomTarget_W;

        public TargetPassType CustomPass_X;

        public TargetPassType CustomPass_Y;

        public TargetPassType CustomPass_Z;

        public TargetPassType CustomPass_W;

        public virtual bool BrushWrite(Mesh mesh, CalcShaderData.CalcVertexsData data)
        {
            return false;
        }

        public virtual void Draw(GUIStyle labelStyle, float maxWidth)
        {

        }
    }
}