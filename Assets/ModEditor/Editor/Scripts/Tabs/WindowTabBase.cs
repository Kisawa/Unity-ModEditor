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

        public virtual void OnFocus() { }

        public virtual void OnLostFocus() { }

        public abstract void Draw();

        protected void Repaint()
        {
            window.Repaint();
        }
    }
}