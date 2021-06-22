using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class Blend : TextureUtilBase
    {
        public override string Name => "Blend";

        public override string Tip => "Blend two texture";

        public override bool IsAvailable => texture != null;

        Texture texture 
        { 
            get => window.BlendTexture;
            set 
            {
                if (window.BlendTexture == value)
                    return;
                window.BlendTexture = value;
                panel.BindTexture(value);
            }
        }
        TexturePanel panel { get => window.BlendTexPanel; }
        BlendType blendType { get => window.BlendBlendType; set => window.BlendBlendType = value; }
        BlendFactor blendTexFactor { get => window.BlendBlendTexFactor; set => window.BlendBlendTexFactor = value; }
        BlendFactor originTexFactor { get => window.BlendOriginTexFactor; set => window.BlendOriginTexFactor = value; }

        public Blend()
        {
            Undo.undoRedoPerformed += undoRedoPerformed;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Undo.undoRedoPerformed -= undoRedoPerformed;
        }

        void undoRedoPerformed()
        {
            panel.BindTexture(texture);
        }

        public override void Excute(RenderTexture texture)
        {
            base.Excute(texture);
            DrawUtil.Self.Blend(texture, this.texture, blendType, originTexFactor, blendTexFactor, window.Manager.ColorMask);
        }

        public override void Draw(GUIStyle labelStyle, float maxWidth)
        {
            base.Draw(labelStyle, maxWidth);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Blend Tex:", labelStyle, GUILayout.Width(100));
            texture = (Texture)EditorGUILayout.ObjectField(texture, typeof(Texture), false, GUILayout.Width(maxWidth - 100));
            EditorGUILayout.EndHorizontal();
            int preIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            panel.Draw(maxWidth / 3);
            EditorGUILayout.BeginVertical();
            float _width = maxWidth / 3 * 2 - 20;
            EditorGUILayout.LabelField("Blend Type:", labelStyle, GUILayout.Width(_width));
            blendType = (BlendType)EditorGUILayout.EnumPopup(blendType, GUILayout.Width(_width));
            EditorGUILayout.LabelField("Blend Tex Factor:", labelStyle, GUILayout.Width(_width));
            blendTexFactor = (BlendFactor)EditorGUILayout.EnumPopup(blendTexFactor, GUILayout.Width(_width));
            EditorGUILayout.LabelField("Origin Tex Factor:", labelStyle, GUILayout.Width(_width));
            originTexFactor = (BlendFactor)EditorGUILayout.EnumPopup(originTexFactor, GUILayout.Width(_width));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = preIndentLevel;
        }
    }
}