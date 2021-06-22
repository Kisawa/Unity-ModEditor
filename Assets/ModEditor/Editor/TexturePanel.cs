using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    [Serializable]
    public class TexturePanel
    {
        EditorWindow window;
        Texture bindTexture;
        [SerializeField]
        RenderTexture viewTexture;
        [SerializeField]
        bool passView;
        [SerializeField]
        TexViewPass viewPass;

        public RenderTexture ViewTexture
        {
            get => viewTexture;
            set
            {
                if (viewTexture == value)
                    return;
                Undo.RecordObject(window, "TexturePanel ViewTexture Changed");
                viewTexture = value;
            }
        }
        public bool PassView
        {
            get => passView;
            set
            {
                if (passView == value)
                    return;
                Undo.RecordObject(window, "TexturePanel PassView Changed");
                passView = value;
            }
        }
        public TexViewPass ViewPass
        {
            get => viewPass;
            set
            {
                if (viewPass == value)
                    return;
                Undo.RecordObject(window, "TexturePanel ViewPass Changed");
                viewPass = value;
            }
        }

        public TexturePanel() { }

        public TexturePanel(EditorWindow window)
        {
            this.window = window;
        }

        public void BindEditor(EditorWindow window)
        {
            this.window = window;
        }

        public void BindTexture(Texture texture)
        {
            if (bindTexture == texture)
                return;
            bindTexture = texture;
            RefreshView();
        }

        public void RefreshView()
        {
            if (bindTexture == null)
            {
                ViewTexture = null;
                return;
            }
            int viewPass = PassView ? (int)ViewPass : -1;
            RenderTexture renderTexture = bindTexture as RenderTexture;
            if (renderTexture == null)
            {
                ViewTexture = new RenderTexture(bindTexture.width, bindTexture.height, 0);
                ViewTexture.enableRandomWrite = true;
                ViewTexture.Create();
                DrawUtil.Self.RefreshView(bindTexture, ViewTexture, viewPass);
            }
            else
            {
                ViewTexture = new RenderTexture(renderTexture.descriptor);
                ViewTexture.enableRandomWrite = true;
                ViewTexture.Create();
                DrawUtil.Self.RefreshView(renderTexture, ViewTexture, viewPass);
            }
        }

        public void Draw(float width, bool withRGBA = true)
        {
            EditorGUILayout.BeginVertical();
            int viewPass, pre;
            if (withRGBA)
            {
                viewPass = PassView ? (int)ViewPass + 1 : 0;
                pre = viewPass;
                if (width > 230)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(width - 220);
                    viewPass = GUILayout.Toolbar(viewPass, new string[] { "RGBA", "R", "G", "B", "A", "Gray" }, "sv_iconselector_back", GUILayout.Width(220));
                    EditorGUILayout.EndHorizontal();
                }
                else if (width > 40)
                {
                    viewPass = EditorGUILayout.Popup(viewPass, new string[] { "RGBA", "R", "G", "B", "A", "Gray" }, "IN MinMaxStateDropDown", GUILayout.Width(width));
                }
                if (viewPass == 0)
                    PassView = false;
                else
                {
                    PassView = true;
                    ViewPass = (TexViewPass)(viewPass - 1);
                }
            }
            else
            {
                viewPass = (int)ViewPass;
                pre = viewPass;

                if (width > 180)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(width - 180);
                    viewPass = GUILayout.Toolbar(viewPass, new string[] { "R", "G", "B", "A", "Gray" }, "sv_iconselector_back", GUILayout.Width(180));
                    EditorGUILayout.EndHorizontal();
                }
                else if (width > 40)
                {
                    viewPass = EditorGUILayout.Popup(viewPass, new string[] { "R", "G", "B", "A", "Gray" }, "IN MinMaxStateDropDown", GUILayout.Width(width));
                }
                passView = true;
                ViewPass = (TexViewPass)viewPass;
            }
            
            if (pre != viewPass)
                RefreshView();

            float ratio = 1;
            if (bindTexture != null)
                ratio = (float)bindTexture.height / bindTexture.width;
            if (ViewTexture != null)
                ratio = (float)ViewTexture.height / ViewTexture.width;
            if (ratio == 1)
            {
                if (PassView && ViewTexture != null)
                    GUILayout.Box(ViewTexture, GUILayout.Width(width), GUILayout.Height(width * ratio));
                else
                    GUILayout.Box(bindTexture, GUILayout.Width(width), GUILayout.Height(width * ratio));
            }
            else if (ratio < 1)
            {
                GUILayout.Space(width * (1 - ratio) / 2);
                if (PassView && ViewTexture != null)
                    GUILayout.Box(ViewTexture, GUILayout.Width(width), GUILayout.Height(width * ratio));
                else
                    GUILayout.Box(bindTexture, GUILayout.Width(width), GUILayout.Height(width * ratio));
                GUILayout.Space(width * (1 - ratio) / 2);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space((width - width / ratio) / 2);
                if (PassView && ViewTexture != null)
                    GUILayout.Box(ViewTexture, GUILayout.Width(width / ratio), GUILayout.Height(width));
                else
                    GUILayout.Box(bindTexture, GUILayout.Width(width / ratio), GUILayout.Height(width));
                GUILayout.Space((width - width / ratio) / 2);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
    }
}