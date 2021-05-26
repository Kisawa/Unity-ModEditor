using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModEditor
{
    public abstract class VertexCalcUtilBase
    {
        public ModEditorWindow window { get; set; }

        public VertexBrushTab Tab { get; set; }

        public abstract string Name { get; }

        public abstract string Tip { get; }

        public abstract PassCount PassCount { get; }

        public virtual bool AllowSelect { get; } = true;

        public virtual bool WithSelect { get; set; }

        public VertexCalcUtilBase() { }

        public virtual void OnFocus()
        {

        }

        public virtual void OnLostFocus()
        {

        }

        public virtual void OnDisable()
        { 
            
        }

        public virtual float[] ExecuteOne(Mesh mesh)
        {
            return null;
        }

        public virtual Vector2[] ExecuteTwo(Mesh mesh)
        {
            return null;
        }

        public virtual Vector3[] ExecuteThree(Mesh mesh)
        {
            return null;
        }

        public virtual Vector4[] ExecuteFour(Mesh mesh)
        {
            return null;
        }

        public virtual Color[] ExecuteColor(Mesh mesh)
        {
            return null;
        }

        public virtual void ExecuteOther(Mesh mesh)
        {
            
        }

        public virtual void Draw(GUIStyle labelStyle, float maxWidth)
        { 
            
        }
    }
}