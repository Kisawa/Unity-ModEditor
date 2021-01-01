using System;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public partial class ModEditorWindow
    {
        public event Action<Camera> onCameraChange;
        public event Action<SceneView> onSceneGUI;

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

        public SceneHandleType SceneHandleType { get; set; }

        MouseCursor mouseCursor;

        void registerEvent()
        {
            EditorEvent.Use.OnKey.Tab.Down += Tab_Down;
            EditorEvent.Use.OnKey.BackQuote.Down += BackQuote_Down;
            EditorEvent.Use.Control.OnMouse.DragLeft += Control_OnMouse_DragLeft;
            EditorEvent.Use.Control.OnScrollWheel.Roll += Control_OnScrollWheel_Roll;
            EditorEvent.Use.Alt.OnScrollWheel.Roll += Alt_OnScrollWheel_Roll;
        }

        void logoutEvent()
        {
            EditorEvent.Use.OnKey.Tab.Down -= Tab_Down;
            EditorEvent.Use.OnKey.BackQuote.Down -= BackQuote_Down;
            EditorEvent.Use.Control.OnMouse.DragLeft -= Control_OnMouse_DragLeft;
            EditorEvent.Use.Control.OnScrollWheel.Roll -= Control_OnScrollWheel_Roll;
            EditorEvent.Use.Alt.OnScrollWheel.Roll -= Alt_OnScrollWheel_Roll;
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

        private void Control_OnMouse_DragLeft()
        {
            if (TabType != ModEditorTabType.NormalEditor || !VertexView)
                return;
            Manager.BrushSize += Event.current.delta.x * 0.01f;
            SceneHandleType = SceneHandleType.BrushSize;
        }

        private void Control_OnScrollWheel_Roll(float obj)
        {
            if (TabType != ModEditorTabType.NormalEditor || !VertexView)
                return;
            Manager.BrushDepth -= Event.current.delta.y * 0.01f;
            SceneHandleType = SceneHandleType.BrushDepth;
        }

        private void Alt_OnScrollWheel_Roll(float obj)
        {
            if (TabType != ModEditorTabType.NormalEditor || !VertexView)
                return;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].SpreadSelects(1);
        }

        private void beforeSceneGui(SceneView obj)
        {
            camera = obj.camera;
            VertexView = Tools.current == Tool.Custom;
            EditorEvent.Update(Event.current, camera);
            viewHandle(obj);
            onSceneGUI?.Invoke(obj);
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
            if (Event.current.isMouse || (Event.current.isScrollWheel && !Event.current.alt))
            {
                for (int i = 0; i < CalcShaderDatas.Count; i++)
                    CalcShaderDatas[i].ClearSpread();
            }
            if (Event.current.control || Event.current.alt)
            {
                SceneHandleType = SceneHandleType.Ready;
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, scene.position.width, scene.position.height), mouseCursor);
            }
            else if (SceneHandleType == SceneHandleType.Ready)
            {
                SceneHandleType = SceneHandleType.None;
            }
            switch (SceneHandleType)
            {
                case SceneHandleType.None:
                case SceneHandleType.BrushDepth:
                    if (TabType == ModEditorTabType.NormalEditor)
                        mouseCursor = MouseCursor.CustomCursor;
                    else
                        mouseCursor = MouseCursor.Arrow;
                    break;
                case SceneHandleType.SceneGUI:
                    mouseCursor = MouseCursor.Arrow;
                    break;
                case SceneHandleType.BrushSize:
                    mouseCursor = MouseCursor.SlideArrow;
                    break;
            }
        }
    }
}