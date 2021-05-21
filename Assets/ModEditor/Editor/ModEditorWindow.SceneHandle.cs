using System;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public partial class ModEditorWindow
    {
        public event Action<Camera> onCameraChange;
        public event Action<SceneView> onSceneValidate;
        public event Action onVertexViewChange;

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

        bool toolView;
        public bool ToolView
        {
            get => toolView;
            set
            {
                if (toolView == value)
                    return;
                toolView = value;
                onVertexViewChange?.Invoke();
                Repaint();
            }
        }

        bool brushColorView;
        public bool BrushColorView
        {
            get => brushColorView;
            set
            {
                if (brushColorView == value)
                    return;
                brushColorView = value;
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

        public bool ZoneLock { get; set; }

        public bool BrushLock { get; set; }

        public bool OnSceneGUI { get; set; }

        MouseCursor mouseCursor;

        void registerEvent()
        {
            Key.ControlOrAltStateChange += Key_ControlOrAltStateChange;
        }

        void logoutEvent()
        {
            Key.ControlOrAltStateChange -= Key_ControlOrAltStateChange;
        }

        private void Key_ControlOrAltStateChange(bool obj)
        {
            SceneHandleType = obj ? SceneHandleType.Ready : SceneHandleType.None;
        }

        private void duringSceneGui(SceneView obj)
        {
            camera = obj.camera;
            ToolView = Tools.current == Tool.Custom;
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