using System;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public partial class ModEditorWindow
    {
        public event Action<Camera> onCameraChange;
        public event Action<SceneView> onSceneValidate;

        Camera _camera;
        public Camera camera
        {
            get => _camera;
            private set
            {
                if (_camera != value)
                {
                    Camera old = _camera;
                    _camera = value;
                    onCameraChange?.Invoke(old);
                }
            }
        }

        bool vertexView;
        public bool VertexView
        {
            get => vertexView;
            set
            {
                if (vertexView == value)
                    return;
                vertexView = value;
                onVertexViewChange?.Invoke();
            }
        }

        SceneHandleType sceneHandleType;
        public SceneHandleType SceneHandleType
        {
            get 
            {
                if (OnSceneGUI)
                    return SceneHandleType.OnSceneGUI;
                return sceneHandleType;
            }
            private set => sceneHandleType = value;
        }

        public bool BrushLock { get; private set; }

        public bool OnSceneGUI { get; set; }

        MouseCursor mouseCursor;

        void registerEvent()
        {
            Key.ControlOrAltStateChange += Key_ControlOrAltStateChange;
            EditorEvent.Use.OnKey.Tab.Down += Tab_Down;
            EditorEvent.Use.OnKey.BackQuote.Down += BackQuote_Down;
            EditorEvent.Use.OnKey.Space.Down += Space_Down;
            EditorEvent.Use.Shift.OnKey.Space.Down += Space_Down;
            EditorEvent.Use.OnKey.CapsLock.Down += CapsLock_Down;
            EditorEvent.Use.Control.OnMouse.DragLeft += Control_OnMouse_DragLeft;
            EditorEvent.Use.Control.OnScrollWheel.Roll += Control_OnScrollWheel_Roll;
            EditorEvent.Use.Alt.OnScrollWheel.Roll += Alt_OnScrollWheel_Roll;
            EditorEvent.Use.ShiftAndAlt.OnScrollWheel.Roll += Alt_OnScrollWheel_Roll;
            Mouse.Update += Mouse_Update;
            ScrollWheel.Update += ScrollWheel_Update;
        }

        void logoutEvent()
        {
            Key.ControlOrAltStateChange -= Key_ControlOrAltStateChange;
            EditorEvent.Use.OnKey.Tab.Down -= Tab_Down;
            EditorEvent.Use.OnKey.BackQuote.Down -= BackQuote_Down;
            EditorEvent.Use.OnKey.Space.Down -= Space_Down;
            EditorEvent.Use.Shift.OnKey.Space.Down -= Space_Down;
            EditorEvent.Use.OnKey.CapsLock.Down -= CapsLock_Down;
            EditorEvent.Use.Control.OnMouse.DragLeft -= Control_OnMouse_DragLeft;
            EditorEvent.Use.Control.OnScrollWheel.Roll -= Control_OnScrollWheel_Roll;
            EditorEvent.Use.Alt.OnScrollWheel.Roll -= Alt_OnScrollWheel_Roll;
            EditorEvent.Use.ShiftAndAlt.OnScrollWheel.Roll -= Alt_OnScrollWheel_Roll;
            Mouse.Update -= Mouse_Update;
            ScrollWheel.Update -= ScrollWheel_Update;
        }

        private void Key_ControlOrAltStateChange(bool obj)
        {
            SceneHandleType = obj ? SceneHandleType.Ready : SceneHandleType.None;
        }

        private void BackQuote_Down()
        {
            Tools.current = Tool.Custom;
        }

        private void Tab_Down()
        {
            if(VertexView)
                Manager.VertexWithZTest = !Manager.VertexWithZTest;
        }

        private void Space_Down()
        {
            if (!VertexView || TabType != ModEditorTabType.VertexBrush)
                return;
            float depth = float.MaxValue;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
            {
                float _depth = CalcShaderDatas[i].GetMinDepth(camera, Mouse.ScreenTexcoord, Manager.BrushSize);
                if (depth > _depth)
                    depth = _depth;
            }
            Manager.BrushDepth = depth + 0.0001f;
            Mouse_Update();
        }

        private void CapsLock_Down()
        {
            BrushLock = !BrushLock;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].LockZoneFromSelect(BrushLock);
        }

        private void Control_OnMouse_DragLeft()
        {
            if (TabType != ModEditorTabType.VertexBrush || !VertexView)
                return;
            Manager.BrushSize += Event.current.delta.x * 0.01f;
            SceneHandleType = SceneHandleType.BrushSize;
        }

        private void Control_OnScrollWheel_Roll(float obj)
        {
            if (TabType != ModEditorTabType.VertexBrush || !VertexView)
                return;
            Manager.BrushDepth -= Event.current.delta.y * 0.01f;
            SceneHandleType = SceneHandleType.BrushDepth;
        }

        private void Alt_OnScrollWheel_Roll(float obj)
        {
            if (TabType != ModEditorTabType.VertexBrush || !VertexView)
                return;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].SpreadSelects(obj < 0);
        }

        private void Mouse_Update()
        {
            if (Mouse.IsButton)
                return;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].ClearSpread();
        }

        private void ScrollWheel_Update()
        {
            if (Key.Alt)
                return;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].ClearSpread();
        }

        private void beforeSceneGui(SceneView obj)
        {
            camera = obj.camera;
            VertexView = Tools.current == Tool.Custom;
            EditorEvent.Update(Event.current, camera);
            viewHandle(obj);
            onSceneValidate?.Invoke(obj);
            if (Manager.GameCameraFollow)
                gameCameraFollow(camera.transform.position, camera.transform.rotation);
        }

        private void gameCameraFollow(Vector3 pos, Quaternion rotate)
        {
            Camera cam = Camera.main;
            if (cam == null)
                return;
            cam.transform.position = pos;
            cam.transform.rotation = rotate;
        }

        private void viewHandle(SceneView scene)
        {
            if (Key.Control && !Key.Alt)
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, scene.position.width, scene.position.height), mouseCursor);
            switch (SceneHandleType)
            {
                case SceneHandleType.Ready:
                    mouseCursor = MouseCursor.Arrow;
                    break;
                case SceneHandleType.BrushDepth:
                    mouseCursor = MouseCursor.ScaleArrow;
                    break;
                case SceneHandleType.BrushSize:
                    mouseCursor = MouseCursor.SlideArrow;
                    break;
            }
            if (OnSceneGUI)
                mouseCursor = MouseCursor.Arrow;
        }
    }
}