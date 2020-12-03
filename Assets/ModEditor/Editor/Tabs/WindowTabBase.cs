using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public abstract class WindowTabBase
    {
        protected EditorWindow window;

        public WindowTabBase(EditorWindow window)
        {
            this.window = window;
        }

        public virtual void OnEnable() { }

        public virtual void OnDiable() { }

        public virtual void OnFocus() { }

        public virtual void OnLostFocus() { }

        public abstract void Draw();

        public virtual void OnInspectorUpdate() { }

        public virtual void OnValidate() { }

        protected void Repaint()
        {
            window.Repaint();
        }
    }
}