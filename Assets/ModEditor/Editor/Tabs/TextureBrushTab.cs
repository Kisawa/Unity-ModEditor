using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            Type[] calcTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(y => typeof(TextureUtilBase).IsAssignableFrom(y) && y.IsClass && !y.IsAbstract)).ToArray();
            utilNames = new string[calcTypes.Length];
            utilTipContents = new GUIContent[calcTypes.Length];
            utilInstances = new TextureUtilBase[calcTypes.Length];
            for (int i = 0; i < calcTypes.Length; i++)
            {
                TextureUtilBase util = (TextureUtilBase)Activator.CreateInstance(calcTypes[i]);
                utilNames[i] = util.Name;
                utilTipContents[i] = new GUIContent(util.Tip, util.Tip);
                utilInstances[i] = util;
                util.window = this.window;
            }
        }

        Vector2 scroll;
        string[] utilNames;
        GUIContent[] utilTipContents;
        TextureUtilBase[] utilInstances;

        public override void OnFocus()
        {
            base.OnFocus();
            Undo.undoRedoPerformed += undoRedoPerformed;
            window.onRefreshTargetDic += refreshDrawBoards;

            EditorEvent.OnMouse.Move += OnMouse_Move;
            EditorEvent.OnMouse.DownLeft += OnMouse_DrawStart;
            EditorEvent.OnMouse.UpLeft += OnMouse_DrawEnd;
            EditorEvent.OnMouse.DragLeft += OnMouse_Draw;
            EditorEvent.OnMouse.DragRight += OnMouse_Move;
            EditorEvent.Control.OnMouse.Move += OnMouse_Move;
            EditorEvent.Control.OnMouse.DragLeft += OnMouse_Move;
            EditorEvent.Control.OnMouse.DragRight += OnMouse_Move;
            EditorEvent.Alt.OnMouse.Move += OnMouse_Move;
            EditorEvent.Alt.OnMouse.DragLeft += OnMouse_Move;
            EditorEvent.Alt.OnMouse.DragRight += OnMouse_Move;
            EditorEvent.OnScrollWheel.Roll += OnScrollWheel_Move;
            
            EditorEvent.Use.Control.OnMouse.DragLeft += Control_OnMouse_DragLeft;
            EditorEvent.Use.Control.OnMouse.DragRight += Control_OnMouse_DragRight;
            EditorEvent.Use.Control.OnScrollWheel.Roll += OnScrollWheel_Roll;
            EditorEvent.Use.OnKey.V.Down += V_Down;

            refreshDrawBoards();
            currentDrawBoard = currentDrawBoard;
            if (window.Manager.TexUtilIndex >= utilTipContents.Length)
                window.Manager.texUtilIndex = utilTipContents.Length - 1;
            utilInstances[window.Manager.TexUtilIndex].OnFocus();
        }

        public override void OnLostFocus()
        {
            base.OnLostFocus();
            Undo.undoRedoPerformed -= undoRedoPerformed;
            window.onRefreshTargetDic -= refreshDrawBoards;

            EditorEvent.OnMouse.Move -= OnMouse_Move;
            EditorEvent.OnMouse.DownLeft -= OnMouse_DrawStart;
            EditorEvent.OnMouse.UpLeft -= OnMouse_DrawEnd;
            EditorEvent.OnMouse.DragLeft -= OnMouse_Draw;
            EditorEvent.OnMouse.DragRight -= OnMouse_Move;
            EditorEvent.Control.OnMouse.Move -= OnMouse_Move;
            EditorEvent.Control.OnMouse.DragLeft -= OnMouse_Move;
            EditorEvent.Control.OnMouse.DragRight -= OnMouse_Move;
            EditorEvent.Alt.OnMouse.Move -= OnMouse_Move;
            EditorEvent.Alt.OnMouse.DragLeft -= OnMouse_Move;
            EditorEvent.Alt.OnMouse.DragRight -= OnMouse_Move;
            EditorEvent.OnScrollWheel.Roll -= OnScrollWheel_Move;

            EditorEvent.Use.Control.OnMouse.DragLeft -= Control_OnMouse_DragLeft;
            EditorEvent.Use.Control.OnMouse.DragRight -= Control_OnMouse_DragRight;
            EditorEvent.Use.Control.OnScrollWheel.Roll -= OnScrollWheel_Roll;
            EditorEvent.Use.OnKey.V.Down -= V_Down;

            utilInstances[window.Manager.TexUtilIndex].OnLostFocus();
        }

        public override void OnDiable()
        {
            base.OnDiable();
            for (int i = 0; i < utilInstances.Length; i++)
                utilInstances[i].OnDisable();
            DrawUtil.Self.ClearCache();
        }

        public override void Draw()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");

            EditorGUILayout.BeginHorizontal("Badge");
            GUILayout.Space(15);
            EditorGUILayout.LabelField("Texel Size:", labelStyle, GUILayout.Width(100));
            window.Manager.TexBrushTexelSize = EditorGUILayout.Vector2IntField("", window.Manager.TexBrushTexelSize, GUILayout.Width(window.position.width - 170));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
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
                    int subCount = drawBoardSubCount[i];
                    bool unfold = drawBoardSubUnfold[i];
                    if (subCount == 1)
                    {
                        bool isCurrentDrawBoard = trans == currentDrawBoard;
                        if (GUILayout.Button(isCurrentDrawBoard ? window.toggleOnContent : window.toggleContent, "ObjectPickerTab"))
                        {
                            subNum = 0;
                            if (isCurrentDrawBoard)
                                currentDrawBoard = null;
                            else
                                currentDrawBoard = trans;
                        }
                    }
                    else
                    {
                        GUILayout.Space(5);
                        if (GUILayout.Button(unfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
                            unfold = !unfold;
                        drawBoardSubUnfold[i] = unfold;
                    }
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(trans, typeof(GameObject), true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                    if (subCount > 1)
                    {
                        if (unfold)
                        {
                            for (int j = 0; j < subCount; j++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Space(20);
                                bool isCurrentDrawBoardSubNum = trans == currentDrawBoard && subNum == j;
                                if (GUILayout.Button(isCurrentDrawBoardSubNum ? window.toggleOnContent : window.toggleContent, "ObjectPickerTab"))
                                {
                                    if (isCurrentDrawBoardSubNum)
                                    {
                                        subNum = 0;
                                        currentDrawBoard = null;
                                    }
                                    else
                                    {
                                        subNum = j;
                                        currentDrawBoard = trans;
                                    }
                                }
                                EditorGUILayout.LabelField($"Sub - {j}", labelStyle);
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        else if(trans == currentDrawBoard && subNum >= 0)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            if (GUILayout.Button(window.toggleOnContent, "ObjectPickerTab"))
                            {
                                subNum = 0;
                                currentDrawBoard = null;
                            }
                            EditorGUILayout.LabelField($"Sub - {subNum}", labelStyle);
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
            drawBrush(labelStyle);
            EditorGUILayout.Space(10);
            drawUtil(labelStyle);
            EditorGUILayout.Space(10);
            drawColorMask(labelStyle);
            EditorGUILayout.Space(10);

            if (TextureManager.IsAvailable)
            {
                int viewPass = textureView ? window.Manager.TexturePassView ? (int)window.Manager.TextureViewPass + 2 : 1 : 0;
                int pre = viewPass;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(window.position.width - 272);
                viewPass = GUILayout.Toolbar(viewPass, new string[] { "None", "RGBA", "R", "G", "B", "A", "Gray" }, "sv_iconselector_back", GUILayout.Width(250), GUILayout.Height(15));
                EditorGUILayout.EndHorizontal();
                if (viewPass == 0)
                {
                    textureView = false;
                    window.Manager.TexturePassView = false;
                }
                else
                {
                    textureView = true;
                    if (viewPass == 1)
                        window.Manager.TexturePassView = false;
                    else
                    {
                        window.Manager.TexturePassView = true;
                        window.Manager.TextureViewPass = (TexViewPass)(viewPass - 2);
                    }
                    if (pre != viewPass)
                        TextureManager.RefreshView();
                }
                if (textureView)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(17);
                    if (window.Manager.TexturePassView)
                        GUILayout.Box(TextureManager.Cache.ViewTexture, GUILayout.Width(window.position.width - 34), GUILayout.Height(window.position.width - 34));
                    else
                        GUILayout.Box(TextureManager.Cache.Texture, GUILayout.Width(window.position.width - 34), GUILayout.Height(window.position.width - 34));
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Save"))
                {
                    string name = drawBoardSubCount[drawBoards.IndexOf(currentDrawBoard)] > 1 ? $"{currentDrawBoard.name}-Sub{subNum}" : currentDrawBoard.name;
                    TextureManager.Save(name);
                }
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
                EditorGUILayout.LabelField("Brush Color:", labelStyle, GUILayout.Width(100));
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
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15);
                if (GUILayout.Button("Clear", "EditModeSingleButton", GUILayout.Width(window.position.width - 60)))
                    TextureManager.ClearDraw();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawUtil(GUIStyle labelStyle)
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (GUILayout.Button("Util", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.TexUtilUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.TexUtilUnfold = !window.Manager.TexUtilUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.TexUtilUnfold)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Util Select:", labelStyle, GUILayout.Width(100));
                if (window.Manager.TexUtilIndex >= utilTipContents.Length)
                    window.Manager.texUtilIndex = utilTipContents.Length - 1;
                int preUtilIndex = window.Manager.TexUtilIndex;
                window.Manager.TexUtilIndex = EditorGUILayout.Popup(window.Manager.TexUtilIndex, utilNames, GUILayout.Width(165));
                if (preUtilIndex != window.Manager.TexUtilIndex)
                {
                    utilInstances[preUtilIndex].OnLostFocus();
                    utilInstances[window.Manager.TexUtilIndex].OnFocus();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.LabelField("", GUI.skin.GetStyle("CN EntryInfoIconSmall"), GUILayout.Width(20));
                EditorGUILayout.LabelField(utilTipContents[window.Manager.TexUtilIndex], GUI.skin.GetStyle("MiniLabel"), GUILayout.Width(window.position.width - 70));
                EditorGUILayout.EndHorizontal();

                TextureUtilBase util = utilInstances[window.Manager.TexUtilIndex];
                if (util.OnlyCustom)
                    utilTargetTextureType = TargetTextureType.Custom;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Texture Select:", labelStyle, GUILayout.Width(125));
                EditorGUI.BeginDisabledGroup(util.OnlyCustom);
                utilTargetTextureType = (TargetTextureType)EditorGUILayout.EnumPopup(utilTargetTextureType, GUILayout.Width(140));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();

                if (utilTargetTextureType == TargetTextureType.Custom)
                {
                    EditorGUILayout.BeginVertical("SelectionRect", GUILayout.Width(window.position.width - 40));

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Origin Tex:", labelStyle, GUILayout.Width(90));
                    utilCustomOriginTex = (Texture)EditorGUILayout.ObjectField(utilCustomOriginTex, typeof(Texture), false, GUILayout.Width(window.position.width - 150));
                    EditorGUILayout.EndHorizontal();

                    if (utilCustomOriginTex != null || utilCustomResultTex != null)
                    {
                        utilCustomViewTexPanel.Draw(window.position.width - 55);

                        EditorGUI.BeginDisabledGroup(utilCustomResultTex == null);
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(32);
                        if (GUILayout.Button($"Save", "EditModeSingleButton", GUILayout.Width((window.position.width - 90) / 2)))
                            DrawUtil.Self.Save(utilCustomResultTex, utilCustomOriginTex == null ? $"{util.Name}-Editor" : utilCustomOriginTex.name);
                        GUILayout.Space(3);
                        if (GUILayout.Button($"Clear", "EditModeSingleButton", GUILayout.Width((window.position.width - 90) / 2)))
                        {
                            utilCustomResultTex = null;
                            utilCustomViewTexPanel.BindTexture(utilCustomOriginTex);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUILayout.EndVertical();
                }

                util.Draw(labelStyle, window.position.width - 45);
                GUILayout.Space(5);
                EditorGUI.BeginDisabledGroup((utilTargetTextureType != TargetTextureType.Custom && !TextureManager.IsAvailable) || !util.IsAvailable);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15);
                if (GUILayout.Button($"Execute", "EditModeSingleButton", GUILayout.Width(window.position.width - 60)))
                    excuteUtil(util);
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawColorMask(GUIStyle labelStyle)
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (GUILayout.Button("Color Mask", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.ColorMaskUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.ColorMaskUnfold = !window.Manager.ColorMaskUnfold;
            EditorGUILayout.EndHorizontal();

            if (window.Manager.ColorMaskUnfold)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                Color mask = window.Manager.ColorMask;
                EditorGUILayout.LabelField("R:", labelStyle, GUILayout.Width(45));
                if (GUILayout.Button(mask.r == 1 ? window.toggleOnContent : window.toggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(15)))
                    mask.r = Math.Abs(mask.r - 1);

                EditorGUILayout.LabelField("G:", labelStyle, GUILayout.Width(45));
                if (GUILayout.Button(mask.g == 1 ? window.toggleOnContent : window.toggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(15)))
                    mask.g = Math.Abs(mask.g - 1);

                EditorGUILayout.LabelField("B:", labelStyle, GUILayout.Width(45));
                if (GUILayout.Button(mask.b == 1 ? window.toggleOnContent : window.toggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(15)))
                    mask.b = Math.Abs(mask.b - 1);

                EditorGUILayout.LabelField("A:", labelStyle, GUILayout.Width(45));
                if (GUILayout.Button(mask.a == 1 ? window.toggleOnContent : window.toggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(15)))
                    mask.a = Math.Abs(mask.a - 1);
                window.Manager.ColorMask = mask;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }
    }
}