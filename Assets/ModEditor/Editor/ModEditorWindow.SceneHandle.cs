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
        public event Action onBrushColorViewChange;

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

        bool brushColorView;
        public bool BrushColorView
        {
            get => brushColorView;
            set
            {
                if (brushColorView == value)
                    return;
                brushColorView = value;
                onBrushColorViewChange?.Invoke();
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

        public bool BrushLock { get; set; }

        public bool OnSceneGUI { get; set; }

        MouseCursor mouseCursor;

        void registerEvent()
        {
            Key.ControlOrAltStateChange += Key_ControlOrAltStateChange;
            EditorEvent.Use.OnKey.Tab.Down += Tab_Down;
            EditorEvent.Use.OnKey.BackQuote.Down += BackQuote_Down;
        }

        void logoutEvent()
        {
            Key.ControlOrAltStateChange -= Key_ControlOrAltStateChange;
            EditorEvent.Use.OnKey.Tab.Down -= Tab_Down;
            EditorEvent.Use.OnKey.BackQuote.Down -= BackQuote_Down;
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

        private void duringSceneGui(SceneView obj)
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