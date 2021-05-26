using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        Vector2 scroll;

        public event Action onCurrentDrawBoardChanged;

        List<Transform> drawBoards;
        Transform currentDrawBoard {
            get => window.TextureBrushTabCurrentDrawBoard;
            set
            {
                if (window.TextureBrushTabCurrentDrawBoard == value)
                    return;
                TextureManager.Clear();
                if (value != null)
                {
                    TextureManager.Init(value, window.Manager.TextureBaseColor);
                    onCurrentDrawBoardChanged?.Invoke();
                }
                window.TextureBrushTabCurrentDrawBoard = value;
            }
        }
        bool textureView { get => window.TextureBrushTabTextureView; set => window.TextureBrushTabTextureView = value; }
        public TextureManager TextureManager { get; } = new TextureManager();

        public override void OnFocus()
        {
            base.OnFocus();
            window.onRefreshTargetDic += refreshDrawBoards;
            EditorEvent.Use.OnMouse.DownLeft += OnMouse_DownLeft;

            refreshDrawBoards();
        }

        public override void OnLostFocus()
        {
            base.OnLostFocus();
            window.onRefreshTargetDic -= refreshDrawBoards;
            EditorEvent.Use.OnMouse.DownLeft -= OnMouse_DownLeft;
        }

        public override void OnDiable()
        {
            base.OnDiable();
            DrawUtil.Self.ClearCache();
        }

        public override void Draw()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
            EditorGUILayout.BeginVertical("flow background");
            if (drawBoards == null || drawBoards.Count == 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUI.skin.GetStyle("CN EntryWarnIconSmall"), GUILayout.Width(20));
                EditorGUILayout.LabelField("No Drawing Board", labelStyle);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                for (int i = 0; i < drawBoards.Count; i++)
                {
                    Transform trans = drawBoards[i];
                    EditorGUILayout.BeginHorizontal();
                    bool isCurrentDrawBoard = trans == currentDrawBoard;
                    if (GUILayout.Button(isCurrentDrawBoard ? window.toggleOnContent : window.toggleContent, "ObjectPickerTab"))
                    {
                        if (isCurrentDrawBoard)
                            currentDrawBoard = null;
                        else
                            currentDrawBoard = trans;
                    }
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(trans, typeof(GameObject), true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base Color:", labelStyle, GUILayout.Width(100));
            window.Manager.TextureBaseColor = EditorGUILayout.ColorField(window.Manager.TextureBaseColor, GUILayout.Width(80));
            if (GUILayout.Button("Refresh"))
                TextureManager.ChangeBase(window.Manager.TextureBaseColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(textureView ? window.viewContent : window.hiddenContent, "ObjectPickerTab"))
                textureView = !textureView;
            RenderTexture texture = null;
            if (TextureManager.IsAvailable)
                texture = TextureManager.Cache.Texture;
            EditorGUILayout.ObjectField(texture, typeof(RenderTexture), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Brush Color:", labelStyle, GUILayout.Width(100));
            window.Manager.TextureBrushColor = EditorGUILayout.ColorField(window.Manager.TextureBrushColor, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Save"))
                TextureManager.Save(window.Manager.Target.name);

            if (textureView && TextureManager.IsAvailable)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(17);
                GUILayout.Box(TextureManager.Cache.Texture, GUILayout.Width(window.position.width - 34), GUILayout.Height(window.position.width - 34));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        void refreshDrawBoards()
        {
            if (drawBoards == null)
                drawBoards = new List<Transform>();
            else
                drawBoards.Clear();
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject obj = window.Manager.TargetChildren[i];
                if (!window.Manager.ActionableDic[obj])
                    continue;
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer == null)
                    continue;
                Collider collider = obj.GetComponent<Collider>();
                if (collider == null || !collider.enabled)
                    continue;
                drawBoards.Add(obj.transform);
            }
        }

        private void OnMouse_DownLeft()
        {
            if (window.OnSceneGUI)
                return;
            if (Physics.Raycast(EditorEvent.Camera.ScreenPointToRay(Mouse.ScreenPos), out RaycastHit hit))
            {
                if (drawBoards.Contains(hit.transform))
                {
                    Debug.LogError(hit.transform.name);
                }
            }
        }
    }
}