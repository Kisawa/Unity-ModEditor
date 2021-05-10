﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public partial class VertexBrushTab : WindowTabBase
    {
        new ModEditorWindow window;

        public VertexBrushTab(EditorWindow window) : base(window)
        {
            this.window = window as ModEditorWindow;
            Type[] calcTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(y => typeof(VertexCalcUtilBase).IsAssignableFrom(y) && y.IsClass && !y.IsAbstract)).ToArray();
            calcUtilNames = new string[calcTypes.Length];
            calcUtilTipContents = new GUIContent[calcTypes.Length];
            calcUtilInstances = new VertexCalcUtilBase[calcTypes.Length];
            for (int i = 0; i < calcTypes.Length; i++)
            {
                VertexCalcUtilBase util = (VertexCalcUtilBase)Activator.CreateInstance(calcTypes[i]);
                calcUtilNames[i] = util.Name;
                calcUtilTipContents[i] = new GUIContent(util.Tip, util.Tip);
                calcUtilInstances[i] = util;
                util.window = this.window;
            }

            Type[] brushTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(y => typeof(VertexBrushUtilBase).IsAssignableFrom(y) && y.IsClass && !y.IsAbstract)).ToArray();
            brushUtilNames = new string[brushTypes.Length];
            brushUtilTipContents = new GUIContent[brushTypes.Length];
            brushUtilInstances = new VertexBrushUtilBase[brushTypes.Length];
            for (int i = 0; i < brushTypes.Length; i++)
            {
                VertexBrushUtilBase util = (VertexBrushUtilBase)Activator.CreateInstance(brushTypes[i]);
                brushUtilNames[i] = util.Name;
                brushUtilTipContents[i] = new GUIContent(util.Tip, util.Tip);
                brushUtilInstances[i] = util;
                util.window = this.window;
            }
        }

        string[] calcUtilNames;
        GUIContent[] calcUtilTipContents;
        VertexCalcUtilBase[] calcUtilInstances;
        string[] brushUtilNames;
        GUIContent[] brushUtilTipContents;
        [SerializeField]
        VertexBrushUtilBase[] brushUtilInstances;

        Vector2 scroll;

        public override void OnFocus()
        {
            base.OnFocus();
            window.onSceneValidate += onSceneValidate;
            EditorEvent.OnMouse.DownLeft += OnMouse_DownLeft;
            EditorEvent.ShiftAndControl.OnMouse.DownLeft += OnMouse_DownLeft;
            EditorEvent.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.Control.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.Alt.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.ControlAndAlt.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.Shift.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.ShiftAndAlt.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.ShiftAndControl.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.ShiftAndControlAndAlt.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.OnMouse.DragLeft += OnMouse_DragLeft;
            EditorEvent.ShiftAndControl.OnMouse.DragLeft += OnMouse_DragLeft;
            EditorEvent.Shift.OnMouse.DownLeft += Shift_OnMouse_Left;
            EditorEvent.Use.Shift.OnMouse.DragLeft += Shift_OnMouse_Left;
            EditorEvent.ShiftAndAlt.OnMouse.DownLeft += Shift_OnMouse_Left;
            EditorEvent.Use.ShiftAndAlt.OnMouse.DragLeft += Shift_OnMouse_Left;
            EditorEvent.Shift.OnMouse.DownRight += Shift_OnMouse_Right;
            EditorEvent.Use.Shift.OnMouse.DragRight += Shift_OnMouse_Right;
            EditorEvent.ShiftAndAlt.OnMouse.DownRight += Shift_OnMouse_Right;
            EditorEvent.Use.ShiftAndAlt.OnMouse.DragRight += Shift_OnMouse_Right;
            EditorEvent.Use.Alt.OnScrollWheel.Roll += Alt_OnScrollWheel_Roll;
            EditorEvent.Use.ShiftAndAlt.OnScrollWheel.Roll += Alt_OnScrollWheel_Roll;
            EditorEvent.Use.ShiftAndControlAndAlt.OnScrollWheel.Roll += Alt_OnScrollWheel_Roll;
            EditorEvent.Use.OnKey.Space.Down += Space_Down;
            EditorEvent.Use.Shift.OnKey.Space.Down += Space_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.Space.Down += Space_Down;
            EditorEvent.Use.OnKey.CapsLock.Down += CapsLock_Down;
            EditorEvent.Use.Alt.OnKey.CapsLock.Down += Alt_CapsLock_Down;
            EditorEvent.Use.Control.OnMouse.DragLeft += Control_OnMouse_DragLeft;
            EditorEvent.Use.Control.OnScrollWheel.Roll += Control_OnScrollWheel_Roll;
            EditorEvent.Use.OnKey.V.Down += V_Down;
            EditorEvent.Use.Control.OnKey.V.Down += V_Down;
            EditorEvent.Use.Shift.OnKey.V.Down += V_Down;
            EditorEvent.Use.Alt.OnKey.V.Down += V_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.V.Down += V_Down;
            EditorEvent.Use.ShiftAndAlt.OnKey.V.Down += V_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.V.Down += V_Down;
            EditorEvent.Use.OnKey.A.Down += A_Down;
            EditorEvent.Use.OnKey.D.Down += D_Down;
            EditorEvent.Use.OnKey.Z.Down += Z_Down;
            EditorEvent.Use.OnKey.C.Down += C_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.A.Down += A_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.D.Down += D_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.Z.Down += Z_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.C.Down += C_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.A.Down += A_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.D.Down += D_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.Z.Down += Z_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.C.Down += C_Down;
            EditorEvent.Use.OnKey.Enter.Down += Enter_Down;
            EditorEvent.Use.Control.OnKey.Enter.Down += Enter_Down;
            EditorEvent.Use.Alt.OnKey.Enter.Down += Enter_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.Enter.Down += Enter_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.Enter.Down += Enter_Down;
            Mouse.Update += Mouse_Update;
            ScrollWheel.Update += ScrollWheel_Update;
            if (window.Manager.CalcUtilIndex >= calcUtilTipContents.Length)
                window.Manager.calcUtilIndex = calcUtilTipContents.Length - 1;
            calcUtilInstances[window.Manager.CalcUtilIndex].OnFocus();
            if (window.Manager.WriteType == WriteType.OtherUtil)
            {
                if (window.Manager.BrushUtilIndex >= brushUtilTipContents.Length)
                    window.Manager.brushUtilIndex = brushUtilTipContents.Length - 1;
                brushUtilInstances[window.Manager.BrushUtilIndex].OnFocus();
            }
        }

        public override void OnLostFocus()
        {
            base.OnLostFocus();
            window.onSceneValidate -= onSceneValidate;
            EditorEvent.OnMouse.DownLeft -= OnMouse_DownLeft;
            EditorEvent.ShiftAndControl.OnMouse.DownLeft -= OnMouse_DownLeft;
            EditorEvent.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.Control.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.Alt.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.ControlAndAlt.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.Shift.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.ShiftAndAlt.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.ShiftAndControl.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.ShiftAndControlAndAlt.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.OnMouse.DragLeft -= OnMouse_DragLeft;
            EditorEvent.ShiftAndControl.OnMouse.DragLeft -= OnMouse_DragLeft;
            EditorEvent.Shift.OnMouse.DownLeft -= Shift_OnMouse_Left;
            EditorEvent.Use.Shift.OnMouse.DragLeft -= Shift_OnMouse_Left;
            EditorEvent.Shift.OnMouse.DownRight -= Shift_OnMouse_Right;
            EditorEvent.ShiftAndAlt.OnMouse.DownLeft -= Shift_OnMouse_Left;
            EditorEvent.Use.ShiftAndAlt.OnMouse.DragLeft -= Shift_OnMouse_Left;
            EditorEvent.Use.Shift.OnMouse.DragRight -= Shift_OnMouse_Right;
            EditorEvent.ShiftAndAlt.OnMouse.DownRight -= Shift_OnMouse_Right;
            EditorEvent.Use.ShiftAndAlt.OnMouse.DragRight -= Shift_OnMouse_Right;
            EditorEvent.Use.Alt.OnScrollWheel.Roll -= Alt_OnScrollWheel_Roll;
            EditorEvent.Use.ShiftAndAlt.OnScrollWheel.Roll -= Alt_OnScrollWheel_Roll;
            EditorEvent.Use.ShiftAndControlAndAlt.OnScrollWheel.Roll -= Alt_OnScrollWheel_Roll;
            EditorEvent.Use.OnKey.Space.Down -= Space_Down;
            EditorEvent.Use.Shift.OnKey.Space.Down -= Space_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.Space.Down -= Space_Down;
            EditorEvent.Use.OnKey.CapsLock.Down -= CapsLock_Down;
            EditorEvent.Use.Alt.OnKey.CapsLock.Down -= Alt_CapsLock_Down;
            EditorEvent.Use.Control.OnMouse.DragLeft -= Control_OnMouse_DragLeft;
            EditorEvent.Use.Control.OnScrollWheel.Roll -= Control_OnScrollWheel_Roll;
            EditorEvent.Use.OnKey.V.Down -= V_Down;
            EditorEvent.Use.Control.OnKey.V.Down -= V_Down;
            EditorEvent.Use.Shift.OnKey.V.Down -= V_Down;
            EditorEvent.Use.Alt.OnKey.V.Down -= V_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.V.Down -= V_Down;
            EditorEvent.Use.ShiftAndAlt.OnKey.V.Down -= V_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.V.Down -= V_Down;
            EditorEvent.Use.OnKey.A.Down -= A_Down;
            EditorEvent.Use.OnKey.D.Down -= D_Down;
            EditorEvent.Use.OnKey.Z.Down -= Z_Down;
            EditorEvent.Use.OnKey.C.Down -= C_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.A.Down -= A_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.D.Down -= D_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.Z.Down -= Z_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.C.Down -= C_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.A.Down -= A_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.D.Down -= D_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.Z.Down -= Z_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.C.Down -= C_Down;
            EditorEvent.Use.OnKey.Enter.Down -= Enter_Down;
            EditorEvent.Use.Control.OnKey.Enter.Down -= Enter_Down;
            EditorEvent.Use.Alt.OnKey.Enter.Down -= Enter_Down;
            EditorEvent.Use.ShiftAndControl.OnKey.Enter.Down -= Enter_Down;
            EditorEvent.Use.ShiftAndControlAndAlt.OnKey.Enter.Down -= Enter_Down;
            Mouse.Update -= Mouse_Update;
            ScrollWheel.Update -= ScrollWheel_Update;
            calcUtilInstances[window.Manager.CalcUtilIndex].OnLostFocus();
            if(window.Manager.WriteType == WriteType.OtherUtil)
                brushUtilInstances[window.Manager.BrushUtilIndex].OnLostFocus();
        }

        public override void Draw()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
            EditorGUILayout.BeginHorizontal("Badge");
            GUILayout.Space(15);
            EditorGUILayout.LabelField("Brush Scope View Color:", labelStyle, GUILayout.Width(150));
            window.Manager.BrushScopeViewColor = EditorGUILayout.ColorField(window.Manager.BrushScopeViewColor, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            drawBrush(labelStyle);
            EditorGUILayout.Space(10);
            drawCalcUtil(labelStyle);
            EditorGUILayout.Space(10);
            drawWrite(labelStyle);
            EditorGUILayout.EndScrollView();
        }

        void drawBrush(GUIStyle labelStyle)
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (GUILayout.Button("Brush", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.BrushUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.BrushUnfold = !window.Manager.BrushUnfold;
            EditorGUILayout.EndHorizontal();

            if (window.Manager.BrushUnfold)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Strength:", labelStyle, GUILayout.Width(80));
                window.Manager.BrushStrength = EditorGUILayout.Slider(window.Manager.BrushStrength, -10, 10, GUILayout.Width(window.position.width - 125));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Brush Type:", labelStyle, GUILayout.Width(100));
                window.Manager.VertexBrushType = (VertexBrushType)EditorGUILayout.EnumPopup(window.Manager.VertexBrushType, GUILayout.Width(window.position.width - 145));
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);
                EditorGUILayout.BeginVertical("SelectionRect", GUILayout.Width(window.position.width - 40));
                switch (window.Manager.VertexBrushType)
                {
                    case VertexBrushType.Color:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Brush Color:", labelStyle, GUILayout.Width(100));
                        window.Manager.BrushColor = EditorGUILayout.ColorField(window.Manager.BrushColor, GUILayout.Width(100));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(80);
                        EditorGUILayout.LabelField($"( {window.Manager.BrushColor.r * window.Manager.BrushStrength}, {window.Manager.BrushColor.g * window.Manager.BrushStrength}, {window.Manager.BrushColor.b * window.Manager.BrushStrength}, {window.Manager.BrushColor.a * window.Manager.BrushStrength} )", labelStyle);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case VertexBrushType.TwoColorGradient:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("From:", labelStyle, GUILayout.Width(80));
                        window.Manager.BrushColorFrom = EditorGUILayout.ColorField(window.Manager.BrushColorFrom, GUILayout.Width(100));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(80);
                        EditorGUILayout.LabelField($"( {window.Manager.BrushColorFrom.r * window.Manager.BrushStrength}, {window.Manager.BrushColorFrom.g * window.Manager.BrushStrength}, {window.Manager.BrushColorFrom.b * window.Manager.BrushStrength}, {window.Manager.BrushColorFrom.a * window.Manager.BrushStrength} )", labelStyle);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("From Step:", labelStyle, GUILayout.Width(100));
                        window.Manager.BrushColorFromStep = EditorGUILayout.Slider(window.Manager.BrushColorFromStep, 0, 1, GUILayout.Width(window.position.width - 160));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("To Step:", labelStyle, GUILayout.Width(100));
                        window.Manager.BrushColorToStep = EditorGUILayout.Slider(window.Manager.BrushColorToStep, 0, 1, GUILayout.Width(window.position.width - 160));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("To:", labelStyle, GUILayout.Width(80));
                        window.Manager.BrushColorTo = EditorGUILayout.ColorField(window.Manager.BrushColorTo, GUILayout.Width(100));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(80);
                        EditorGUILayout.LabelField($"( {window.Manager.BrushColorTo.r * window.Manager.BrushStrength}, {window.Manager.BrushColorTo.g * window.Manager.BrushStrength}, {window.Manager.BrushColorTo.b * window.Manager.BrushStrength}, {window.Manager.BrushColorTo.a * window.Manager.BrushStrength} )", labelStyle);
                        EditorGUILayout.EndHorizontal();
                        break;
                }
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawCalcUtil(GUIStyle labelStyle)
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (GUILayout.Button("Calc Util", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.CalcUtilUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.CalcUtilUnfold = !window.Manager.CalcUtilUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.CalcUtilUnfold)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Util Select:", labelStyle, GUILayout.Width(100));
                if (window.Manager.CalcUtilIndex >= calcUtilTipContents.Length)
                    window.Manager.calcUtilIndex = calcUtilTipContents.Length - 1;
                int preCalcUtilIndex = window.Manager.CalcUtilIndex;
                window.Manager.CalcUtilIndex = EditorGUILayout.Popup(window.Manager.CalcUtilIndex, calcUtilNames, GUILayout.Width(140));
                if (preCalcUtilIndex != window.Manager.CalcUtilIndex)
                {
                    calcUtilInstances[preCalcUtilIndex].OnLostFocus();
                    calcUtilInstances[window.Manager.CalcUtilIndex].OnFocus();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.LabelField("", GUI.skin.GetStyle("CN EntryInfoIconSmall"), GUILayout.Width(20));
                EditorGUILayout.LabelField(calcUtilTipContents[window.Manager.CalcUtilIndex], GUI.skin.GetStyle("MiniLabel"), GUILayout.Width(window.position.width - 70));
                EditorGUILayout.EndHorizontal();
                VertexCalcUtilBase util = calcUtilInstances[window.Manager.CalcUtilIndex];
                util.Draw(labelStyle, window.position.width - 40);
                GUILayout.Space(5);
                if (util.AllowSelect)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("With Select:", labelStyle, GUILayout.Width(130));
                    if (GUILayout.Button(util.WithSelect ? window.toggleOnContent : window.toggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
                        util.WithSelect = !util.WithSelect;
                    if (util.WithSelect)
                        EditorGUILayout.LabelField("/Enter", GUI.skin.GetStyle("LODSliderTextSelected"), GUILayout.Width(80));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.BeginDisabledGroup(util.AllowSelect && util.WithSelect);
                if (GUILayout.Button($"Execute Write - {passCountStr(util.PassCount)} Pass", "EditModeSingleButton", GUILayout.Width(window.position.width - 40)))
                    executeCalcUtil(util);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawWrite(GUIStyle labelStyle)
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (GUILayout.Button("Write", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.WriteUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.WriteUnfold = !window.Manager.WriteUnfold;
            EditorGUILayout.EndHorizontal();

            if (window.Manager.WriteUnfold)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Type:", labelStyle, GUILayout.Width(100));
                WriteType preWriteType = window.Manager.WriteType;
                window.Manager.WriteType = (WriteType)EditorGUILayout.EnumPopup(window.Manager.WriteType, GUILayout.Width(140));
                EditorGUILayout.EndHorizontal();
                bool presetTargetType = false;
                if (window.Manager.WriteType == WriteType.OtherUtil)
                {
                    EditorGUILayout.BeginVertical("SelectionRect", GUILayout.Width(window.position.width - 40));
                    EditorGUILayout.LabelField("Util Select:", GUI.skin.GetStyle("AnimationTimelineTick"));
                    if (window.Manager.BrushUtilIndex >= brushUtilTipContents.Length)
                        window.Manager.brushUtilIndex = brushUtilTipContents.Length - 1;
                    int preBrushUtilIndex = window.Manager.BrushUtilIndex;
                    window.Manager.BrushUtilIndex = EditorGUILayout.Popup(window.Manager.BrushUtilIndex, brushUtilNames, GUILayout.Width(window.position.width - 65));
                    if (preWriteType != WriteType.OtherUtil)
                        brushUtilInstances[window.Manager.BrushUtilIndex].OnFocus();
                    else if (preBrushUtilIndex != window.Manager.BrushUtilIndex)
                    {
                        brushUtilInstances[preBrushUtilIndex].OnLostFocus();
                        brushUtilInstances[window.Manager.BrushUtilIndex].OnFocus();
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("", GUI.skin.GetStyle("CN EntryInfoIconSmall"), GUILayout.Width(20));
                    EditorGUILayout.LabelField(brushUtilTipContents[window.Manager.BrushUtilIndex], GUI.skin.GetStyle("MiniLabel"), GUILayout.Width(window.position.width - 80));
                    EditorGUILayout.EndHorizontal();
                    brushUtilInstances[window.Manager.BrushUtilIndex].Draw(labelStyle, window.position.width - 80);
                    EditorGUILayout.EndVertical();
                    WriteTargetType utilTarget = brushUtilInstances[window.Manager.BrushUtilIndex].UtilTarget;
                    if (utilTarget != WriteTargetType.None)
                    {
                        window.Manager.WriteTargetType = utilTarget;
                        presetTargetType = true;
                    }
                }
                else if(preWriteType == WriteType.OtherUtil)
                    brushUtilInstances[window.Manager.BrushUtilIndex].OnLostFocus();
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Target Type:", labelStyle, GUILayout.Width(100));
                EditorGUI.BeginDisabledGroup(presetTargetType);
                window.Manager.WriteTargetType = (WriteTargetType)EditorGUILayout.EnumPopup(window.Manager.WriteTargetType, GUILayout.Width(140));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
                if (window.Manager.WriteTargetType == WriteTargetType.Custom)
                {
                    EditorGUI.indentLevel = 3;
                    customTargetDraw(labelStyle, 4);
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void customTargetDraw(GUIStyle labelStyle, int passCount)
        {
            if (passCount < 1)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(85);
                EditorGUILayout.LabelField("No Pass", labelStyle);
                EditorGUILayout.EndHorizontal();
                return;
            }
            string[] pass3Str = new string[] { "X", "Y", "Z" };
            if (passCount > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("X:", labelStyle, GUILayout.Width(60));
                window.Manager.CustomTargetType_X = (CustomTargetType)EditorGUILayout.EnumPopup(window.Manager.CustomTargetType_X, GUILayout.Width(140));
                if (window.Manager.CustomTargetType_X != CustomTargetType.None)
                {
                    if (window.Manager.CustomTargetType_X == CustomTargetType.Vertex || window.Manager.CustomTargetType_X == CustomTargetType.Normal)
                        window.Manager.CustomTargetPass_X = (TargetPassType)EditorGUILayout.Popup((int)window.Manager.CustomTargetPass_X, pass3Str, GUILayout.Width(80));
                    else
                        window.Manager.CustomTargetPass_X = (TargetPassType)EditorGUILayout.EnumPopup(window.Manager.CustomTargetPass_X, GUILayout.Width(80));
                }
                EditorGUILayout.EndHorizontal();
            }
            if (passCount > 1)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Y:", labelStyle, GUILayout.Width(60));
                window.Manager.CustomTargetType_Y = (CustomTargetType)EditorGUILayout.EnumPopup(window.Manager.CustomTargetType_Y, GUILayout.Width(140));
                if (window.Manager.CustomTargetType_Y != CustomTargetType.None)
                {
                    if (window.Manager.CustomTargetType_Y == CustomTargetType.Vertex || window.Manager.CustomTargetType_Y == CustomTargetType.Normal)
                        window.Manager.CustomTargetPass_Y = (TargetPassType)EditorGUILayout.Popup((int)window.Manager.CustomTargetPass_Y, pass3Str, GUILayout.Width(80));
                    else
                        window.Manager.CustomTargetPass_Y = (TargetPassType)EditorGUILayout.EnumPopup(window.Manager.CustomTargetPass_Y, GUILayout.Width(80));
                }
                EditorGUILayout.EndHorizontal();
            }
            if (passCount > 2)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Z:", labelStyle, GUILayout.Width(60));
                window.Manager.CustomTargetType_Z = (CustomTargetType)EditorGUILayout.EnumPopup(window.Manager.CustomTargetType_Z, GUILayout.Width(140));
                if (window.Manager.CustomTargetType_Z != CustomTargetType.None)
                {
                    if (window.Manager.CustomTargetType_Z == CustomTargetType.Vertex || window.Manager.CustomTargetType_Z == CustomTargetType.Normal)
                        window.Manager.CustomTargetPass_Z = (TargetPassType)EditorGUILayout.Popup((int)window.Manager.CustomTargetPass_Z, pass3Str, GUILayout.Width(80));
                    else
                        window.Manager.CustomTargetPass_Z = (TargetPassType)EditorGUILayout.EnumPopup(window.Manager.CustomTargetPass_Z, GUILayout.Width(80));
                }
                EditorGUILayout.EndHorizontal();
            }
            if (passCount > 3)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("W:", labelStyle, GUILayout.Width(60));
                window.Manager.CustomTargetType_W = (CustomTargetType)EditorGUILayout.EnumPopup(window.Manager.CustomTargetType_W, GUILayout.Width(140));
                if (window.Manager.CustomTargetType_W != CustomTargetType.None)
                {
                    if (window.Manager.CustomTargetType_W == CustomTargetType.Vertex || window.Manager.CustomTargetType_W == CustomTargetType.Normal)
                        window.Manager.CustomTargetPass_W = (TargetPassType)EditorGUILayout.Popup((int)window.Manager.CustomTargetPass_W, pass3Str, GUILayout.Width(80));
                    else
                        window.Manager.CustomTargetPass_W = (TargetPassType)EditorGUILayout.EnumPopup(window.Manager.CustomTargetPass_W, GUILayout.Width(80));
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void onSceneValidate(SceneView scene)
        {
            if (!window.VertexView)
                return;
            if (window.SceneHandleType == SceneHandleType.None)
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, scene.position.width, scene.position.height), MouseCursor.CustomCursor);
        }

        string passCountStr(PassCount pass)
        {
            switch (pass)
            {
                case PassCount.One:
                    return "One";
                case PassCount.Two:
                    return "Two";
                case PassCount.Three:
                    return "Three";
                case PassCount.Four:
                case PassCount.Color:
                    return "Four";
            }
            return "No";
        }
    }
}