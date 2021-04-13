using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ModEditor
{
    public partial class VertexBrushTab : WindowTabBase
    {
        new ModEditorWindow window;

        public VertexBrushTab(EditorWindow window) : base(window)
        {
            this.window = window as ModEditorWindow;
        }

        Texture2D defaultCursor;
        Texture2D brushCursor;

        List<(Transform, Mesh)> objInOperation;

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
            EditorEvent.Use.OnMouse.DownScroll += OnMouse_Scroll;
            EditorEvent.Use.OnMouse.UpScroll += OnMouse_Scroll;
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
            if (brushCursor == null)
                brushCursor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{ModEditorWindow.ModEditorPath}/Textures/brushCursor.png");
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
            EditorEvent.Use.OnMouse.DownScroll -= OnMouse_Scroll;
            EditorEvent.Use.OnMouse.UpScroll -= OnMouse_Scroll;
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
            PlayerSettings.defaultCursor = defaultCursor;
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }

        public override void Draw()
        {
            if (GUILayout.Button("Write avg normals to model's tangent"))
            {
                writeAcgNormalToTangent();
            }
            window.Manager.BrushType = (BrushType)EditorGUILayout.EnumPopup(window.Manager.BrushType);
            window.Manager.BrushColor = EditorGUILayout.ColorField("Brush Color", window.Manager.BrushColor);
            window.Manager.BrushViewColor = EditorGUILayout.ColorField(new GUIContent("Brush View Color"), window.Manager.BrushViewColor, true, true, false);

            if (GUILayout.Button("asd"))
                func();
            index = EditorGUILayout.IntSlider(index, 0, 3);
        }

        int index = 0;
        void func()
        {

        }

        private void OnMouse_DownLeft()
        {
            if (window.OnSceneGUI)
                return;
            if (window.VertexView)
            {
                objInOperation = recordObjInOperation();
                write();
            }
        }

        private void OnMouse_UpLeft()
        {
            objInOperation = null;
        }

        private void OnMouse_DragLeft()
        {
            if (window.OnSceneGUI)
                return;
            if (window.VertexView)
                write();
        }

        private void OnMouse_Scroll() { }

        private void Alt_OnScrollWheel_Roll(float obj)
        {
            if (window.OnSceneGUI || !window.VertexView)
                return;
            for (int i = 0; i < window.CalcShaderDatas.Count; i++)
                window.CalcShaderDatas[i].SpreadSelects(obj < 0);
        }

        private void Shift_OnMouse_Left()
        {
            if (window.OnSceneGUI || !window.BrushLock)
                return;
            addZone();
        }

        private void Shift_OnMouse_Right()
        {
            if (window.OnSceneGUI || !window.BrushLock)
                return;
            subZone();
        }

        private void onSceneValidate(SceneView scene)
        {
            if (!window.VertexView)
                return;
            if (PlayerSettings.defaultCursor != brushCursor)
            {
                defaultCursor = PlayerSettings.defaultCursor;
                PlayerSettings.defaultCursor = brushCursor;
                Cursor.SetCursor(brushCursor, new Vector2(brushCursor.width / 2f, brushCursor.height / 2f), CursorMode.Auto);
            }
            if (window.SceneHandleType == SceneHandleType.None)
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, scene.position.width, scene.position.height), MouseCursor.CustomCursor);
        }

        List<(Transform, Mesh)> recordObjInOperation()
        {
            if (window.camera == null)
                return null;
            List<(Transform, Mesh)> _objInOperation = new List<(Transform, Mesh)>();
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject target = window.Manager.TargetChildren[i];
                if (target == null)
                    continue;
                if (!window.Manager.ActionableDic[target])
                    continue;
                MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    window.SetEditingMesh(target, meshFilter);
                    _objInOperation.Add((target.transform, meshFilter.sharedMesh));
                }
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    window.SetEditingMesh(target, skinnedMeshRenderer);
                    _objInOperation.Add((target.transform, skinnedMeshRenderer.sharedMesh));
                }
            }
            return _objInOperation;
        }
    }
}