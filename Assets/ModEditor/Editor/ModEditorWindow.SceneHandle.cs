using System;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public partial class ModEditorWindow
    {
        Vector3 screenTexcoord;
        public Vector3 ScreenTexcoord { get => screenTexcoord; }

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
                if (vertexView)
                    ModEditorTool.RefreshCalcBuffer();
                else
                    ModEditorTool.RemoveCalcBuffer();
            }
        }

        public SceneHandleType sceneHandleType { get; private set; }

        MouseCursor mouseCursor;

        private void beforeSceneGui(SceneView obj)
        {
            camera = obj.camera;
            VertexView = Tools.current == Tool.Custom;
            viewHandle(obj);
            screenTexcoord = camera.ScreenToViewportPoint(Event.current.mousePosition);
            screenTexcoord.y = 1 - ScreenTexcoord.y;
            screenTexcoord.z = (float)Screen.width / Screen.height;
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
            if (Event.current.control)
            {
                sceneHandleType = SceneHandleType.Ready;
                if (Event.current.isScrollWheel)
                {
                    Vector3 pos = scene.pivot;
                    pos -= scene.camera.transform.forward * Event.current.delta.y * 0.002f;
                    scene.pivot = pos;
                    Event.current.Use();
                    sceneHandleType = SceneHandleType.ViewZoom;
                }
                if (Event.current.isMouse)
                {
                    if (Event.current.button == 0 && !Event.current.alt)
                    {
                        if (TabType == ModEditorTabType.NormalEditor && VertexView && Event.current.type == EventType.MouseDrag)
                        {
                            Manager.BrushSize += Event.current.delta.x * 0.01f;
                            Event.current.Use();
                            sceneHandleType = SceneHandleType.BrushSize;
                        }
                    }
                }
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, scene.position.width, scene.position.height), mouseCursor);
            }
            else
            {
                sceneHandleType = SceneHandleType.None;
            }
            if (Event.current.Equals(Event.KeyboardEvent("BackQuote")))
            {
                Tools.current = Tool.Custom;
                Event.current.Use();
            }
            switch (sceneHandleType)
            {
                case SceneHandleType.None:
                    mouseCursor = MouseCursor.Arrow;
                    break;
                case SceneHandleType.ViewZoom:
                    mouseCursor = MouseCursor.ScaleArrow;
                    break;
                case SceneHandleType.BrushSize:
                    mouseCursor = MouseCursor.SlideArrow;
                    break;
            }
        }
    }
}