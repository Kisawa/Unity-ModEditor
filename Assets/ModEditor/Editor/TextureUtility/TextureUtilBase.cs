using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModEditor
{
    public abstract class TextureUtilBase
    {
        public ModEditorWindow window { get; set; }

        public abstract string Name { get; }

        public abstract string Tip { get; }

        public virtual bool IsAvailable { get; } = true;

        public virtual bool OnlyCustom { get; } = false;

        public TextureUtilBase() { }

        public virtual void OnFocus()
        {

        }

        public virtual void OnLostFocus()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void Excute(RenderTexture texture)
        { 
            
        }

        public virtual void Draw(GUIStyle labelStyle, float maxWidth)
        {

        }
    }
}