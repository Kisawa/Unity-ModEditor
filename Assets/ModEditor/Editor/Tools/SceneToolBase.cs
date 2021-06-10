using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public abstract class SceneToolBase
    {
        protected static ModEditorWindow ModEditor => ModEditorWindow.Self;

        protected ModEditorTool tool;

        public virtual void OnEnable() { }

        public virtual void OnSceneGUI(EditorWindow window)
        { 
            
        }

        public virtual Rect Draw(EditorWindow window, GUIStyle txtStyle, GUIStyle hotKeyStyle, GUIStyle msgStyle)
        {
            return window.position;
        }

        public abstract bool IsAvailable();
    }
}