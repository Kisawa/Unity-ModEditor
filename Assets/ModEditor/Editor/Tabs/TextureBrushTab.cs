using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public partial class TextureBrushTab : WindowTabBase
    {
        new ModEditorWindow window;

        public TextureBrushTab(EditorWindow window) : base(window)
        {
            this.window = window as ModEditorWindow;
        }

        Vector2 scroll;

        public override void OnFocus()
        {
            base.OnFocus();
            Undo.undoRedoPerformed += undoRedoPerformed;
            window.onRefreshTargetDic += refreshDrawBoards;

            EditorEvent.OnMouse.Move += OnMouse_Move;
            EditorEvent.OnMouse.DownLeft += OnMouse_DrawStart;
            EditorEvent.OnMouse.DragLeft += OnMouse_Draw;
            EditorEvent.OnMouse.DragRight += OnMouse_Move;
            EditorEvent.Control.OnMouse.Move += OnMouse_Move;
            EditorEvent.Control.OnMouse.DragLeft += OnMouse_Move;
            EditorEvent.Control.OnMouse.DragRight += OnMouse_Move;
            EditorEvent.Alt.OnMouse.Move += OnMouse_Move;
            EditorEvent.Alt.OnMouse.DragLeft += OnMouse_Move;
            EditorEvent.Alt.OnMouse.DragRight += OnMouse_Move;
            
            EditorEvent.Use.Control.OnMouse.DragLeft += Control_OnMouse_DragLeft;
            EditorEvent.Use.Control.OnMouse.DragRight += Control_OnMouse_DragRight;
            EditorEvent.Use.Control.OnScrollWheel.Roll += OnScrollWheel_Roll;
            EditorEvent.Use.OnKey.V.Down += V_Down;

            refreshDrawBoards();
            currentDrawBoard = currentDrawBoard;
        }

        public override void OnLostFocus()
        {
            base.OnLostFocus();
            Undo.undoRedoPerformed -= undoRedoPerformed;
            window.onRefreshTargetDic -= refreshDrawBoards;

            EditorEvent.OnMouse.Move -= OnMouse_Move;
            EditorEvent.OnMouse.DownLeft -= OnMouse_DrawStart;
            EditorEvent.OnMouse.DragLeft -= OnMouse_Draw;
            EditorEvent.OnMouse.DragRight -= OnMouse_Move;
            EditorEvent.Control.OnMouse.Move -= OnMouse_Move;
            EditorEvent.Control.OnMouse.DragLeft -= OnMouse_Move;
            EditorEvent.Control.OnMouse.DragRight -= OnMouse_Move;
            EditorEvent.Alt.OnMouse.Move -= OnMouse_Move;
            EditorEvent.Alt.OnMouse.DragLeft -= OnMouse_Move;
            EditorEvent.Alt.OnMouse.DragRight -= OnMouse_Move;
            EditorEvent.Use.Control.OnMouse.DragLeft -= Control_OnMouse_DragLeft;
            EditorEvent.Use.Control.OnMouse.DragRight -= Control_OnMouse_DragRight;
            EditorEvent.Use.Control.OnScrollWheel.Roll -= OnScrollWheel_Roll;
            EditorEvent.Use.OnKey.V.Down -= V_Down;
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

            EditorGUILayout.Space(10);
            drawBrush(labelStyle);
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(textureView ? window.viewContent : window.hiddenContent, "ObjectPickerTab"))
                textureView = !textureView;
            RenderTexture texture = null;
            if (TextureManager.IsAvailable)
                texture = TextureManager.Cache.Texture;
            EditorGUILayout.ObjectField(texture, typeof(RenderTexture), false);
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
        
        void drawBrush(GUIStyle labelStyle)
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (GUILayout.Button("Background", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.TexBrushUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.TexBrushUnfold = !window.Manager.TexBrushUnfold;
            EditorGUILayout.EndHorizontal();

            if (window.Manager.TexBrushUnfold)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Base Color:", labelStyle, GUILayout.Width(100));
                window.Manager.TextureBaseColor = EditorGUILayout.ColorField(window.Manager.TextureBaseColor, GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15);
                if (GUILayout.Button("Refresh With Color", "EditModeSingleButton", GUILayout.Width(window.position.width - 60)))
                    TextureManager.ChangeBase(window.Manager.TextureBaseColor);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Base Tex:", labelStyle, GUILayout.Width(100));
                baseTex = (Texture)EditorGUILayout.ObjectField(baseTex, typeof(Texture), false, GUILayout.Width(window.position.width - 150));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15);
                if (GUILayout.Button("Refresh With Texture", "EditModeSingleButton", GUILayout.Width(window.position.width - 60)))
                    TextureManager.ChangeBase(baseTex);
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (GUILayout.Button("Foreground", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.TexBrushForegroundUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.TexBrushForegroundUnfold = !window.Manager.TexBrushForegroundUnfold;
            EditorGUILayout.EndHorizontal();

            if (window.Manager.TexBrushForegroundUnfold)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Brush Color:", labelStyle, GUILayout.Width(125));
                window.Manager.TextureBrushColor = EditorGUILayout.ColorField(window.Manager.TextureBrushColor, GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);

                Vector3 texBrushRange = window.Manager.TexBrushRange;
                EditorGUILayout.LabelField("BurshRange Width:", labelStyle, GUILayout.Width(125));
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30);
                texBrushRange.x = EditorGUILayout.Slider(texBrushRange.x, 0, 0.5f, GUILayout.Width(window.position.width - 100));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("BurshRange Height:", labelStyle, GUILayout.Width(125));
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30);
                texBrushRange.y = EditorGUILayout.Slider(texBrushRange.y, 0, 0.5f, GUILayout.Width(window.position.width - 100));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("BrushRange Inner:", labelStyle, GUILayout.Width(125));
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30);
                texBrushRange.z = EditorGUILayout.Slider(texBrushRange.z, 0, 1, GUILayout.Width(window.position.width - 100));
                EditorGUILayout.EndHorizontal();
                window.Manager.TexBrushRange = texBrushRange;
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }
    }
}