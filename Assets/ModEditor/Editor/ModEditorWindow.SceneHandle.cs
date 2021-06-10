using System;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public partial class ModEditorWindow
    {
        public event Action<Camera> onCameraChange;
        public event Action<SceneView> onSceneValidate;
        public event Action onSceneToolChange;

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

        ModEditorToolType toolType;
        public ModEditorToolType ToolType
        {
            get => toolType;
            set
            {
                if (toolType == value)
                    return;
                toolType = value;
                onSceneToolChange?.Invoke();
                Repaint();
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
            set => sceneHandleType = value;
        }

        public bool OnSceneGUI { get; set; }

        MouseCursor mouseCursor;

        void registerEvent()
        {
            Key.ControlOrAltStateChange += Key_ControlOrAltStateChange;
            EditorEvent.Use.OnKey.BackQuote.Down += BackQuote_Down;
        }

        void logoutEvent()
        {
            Key.ControlOrAltStateChange -= Key_ControlOrAltStateChange;
            EditorEvent.Use.OnKey.BackQuote.Down -= BackQuote_Down;
        }

        private void BackQuote_Down()
        {
            Tools.current = Tool.Custom;
        }

        private void Key_ControlOrAltStateChange(bool obj)
        {
            SceneHandleType = obj ? SceneHandleType.Ready : SceneHandleType.None;
        }

        private void duringSceneGui(SceneView obj)
        {
            camera = obj.camera;
            EditorEvent.Update(Event.current, camera);
            if (Tools.current == Tool.Custom)
            {
                switch (TabType)
                {
                    case ModEditorTabType.VertexBrush:
                        ToolType = ModEditorToolType.VertexBrush;
                        break;
                    case ModEditorTabType.TextureBrush:
                        ToolType = ModEditorToolType.TextureBrush;
                        break;
                    default:
                        ToolType = ModEditorToolType.None;
                        break;
                }
            }
            else
                ToolType = ModEditorToolType.None;
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