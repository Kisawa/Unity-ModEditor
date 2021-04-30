using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModEditor
{
    public abstract class VertexCalcUtilBase
    {
        public abstract string Name { get; }

        public abstract string Tip { get; }

        public abstract PassCount PassCount { get; }

        public VertexCalcUtilBase() { }

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

        public virtual void Draw(GUIStyle labelStyle, float maxWidth)
        { 
            
        }
    }
}