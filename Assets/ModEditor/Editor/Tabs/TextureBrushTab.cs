using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class TextureBrushTab : WindowTabBase
    {
        new ModEditorWindow window;

        public TextureBrushTab(EditorWindow window) : base(window)
        {
            this.window = window as ModEditorWindow;
        }

        public override void Draw()
        {
            EditorGUILayout.LabelField("Texture Brush");
        }
    }
}