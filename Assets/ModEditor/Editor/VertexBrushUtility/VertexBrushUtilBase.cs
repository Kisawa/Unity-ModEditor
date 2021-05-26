using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ModEditor
{
    public abstract class VertexBrushUtilBase
    {
        public ModEditorWindow window { get; set; }

        public VertexBrushTab Tab { get; set; }

        public abstract string Name { get; }

        public abstract string Tip { get; }

        public virtual WriteTargetType UtilTarget { get; } = WriteTargetType.None;

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

        public virtual void OnFocus()
        { 
            
        }

        public virtual void OnLostFocus()
        { 
            
        }

        public virtual void OnDisable()
        { 
            
        }

        public virtual void WriteStart(Mesh mesh)
        { 
            
        }

        public virtual bool BrushWrite(Mesh mesh, CalcManager data)
        {
            return false;
        }

        public virtual void WriteEnd(Mesh mesh)
        {

        }

        public virtual void Draw(GUIStyle labelStyle, float maxWidth)
        {

        }
    }
}