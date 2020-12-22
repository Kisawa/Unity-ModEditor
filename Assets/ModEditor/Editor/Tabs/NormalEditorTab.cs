using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ModEditor
{
    public partial class NormalEditorTab : WindowTabBase
    {
        new ModEditorWindow window;

        public NormalEditorTab(EditorWindow window) : base(window)
        {
            this.window = window as ModEditorWindow;
        }

        Texture2D defaultCursor;
        Texture2D brushCursor;

        RenderTexture texture;
        CommandBuffer buffer;
        CameraEvent cameraEvent = CameraEvent.BeforeForwardOpaque;
        Vector3 screenTexcoord;

        List<(Transform, Mesh, SkinnedMeshRenderer)> objInOperation;

        public override void OnEnable()
        {
            base.OnEnable();
            test = new int[5] { 0, 1, 2, 3, 4 };
        }
        int[] test;
        public override void OnFocus()
        {
            base.OnFocus();
            window.onCameraChange += onCameraChange;
            window.onRefreshTargetDic += refreshBuffer;
            window.onVertexViewChange += refreshBuffer;
            window.onSceneGUI += onSceneGUI;
            onCameraChange(null);
            if (brushCursor == null)
                brushCursor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{ModEditorWindow.ModEditorPath}Textures/brushCursor.png");
        }

        public override void OnLostFocus()
        {
            base.OnLostFocus();
            window.onCameraChange -= onCameraChange;
            window.onRefreshTargetDic -= refreshBuffer;
            window.onVertexViewChange -= refreshBuffer;
            window.onSceneGUI -= onSceneGUI;
            if (window.camera != null && buffer != null)
                window.camera.RemoveCommandBuffer(cameraEvent, buffer);
            if (texture != null)
            {
                RenderTexture.ReleaseTemporary(texture);
                texture = null;
            }
            if (_expandEdgeTexture != null)
            {
                RenderTexture.ReleaseTemporary(_expandEdgeTexture);
                _expandEdgeTexture = null;
            }
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
            EditorGUILayout.ObjectField(texture, typeof(RenderTexture), true);
            EditorGUILayout.ObjectField(_expandEdgeTexture, typeof(RenderTexture), true);

            if(GUILayout.Button("test"))
            {
                int[] _test = test.Skip(0).Take(2).ToArray();
                for (int i = 0; i < _test.Length; i++)
                {
                    Debug.LogError(_test[i]);
                }
            }
        }

        private void onSceneGUI(SceneView scene)
        {
            if (!window.VertexView)
                return;
            if (PlayerSettings.defaultCursor != brushCursor)
            {
                defaultCursor = PlayerSettings.defaultCursor;
                PlayerSettings.defaultCursor = brushCursor;
                Cursor.SetCursor(brushCursor, new Vector2(brushCursor.width / 2f, brushCursor.height / 2f), CursorMode.Auto);
            }
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, scene.position.width, scene.position.height), MouseCursor.CustomCursor);
            updateMaterial();
            if (window.sceneHandleType != SceneHandleType.None)
                return;
            if (Event.current.type == EventType.MouseUp)
            {
                objInOperation = null;
            }
            if (Event.current.alt)
                return;
            if (Event.current.button == 0)
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    objInOperation = recordObjInOperation();
                    write();
                }
                if (Event.current.type == EventType.MouseDrag)
                {
                    write();
                }
            }
        }

        private void onCameraChange(Camera obj)
        {
            if (obj != null && buffer != null)
                obj.RemoveCommandBuffer(cameraEvent, buffer);
            if (window.camera != null)
            {
                if (texture == null)
                {
                    texture = RenderTexture.GetTemporary(4096, 4096, 24);
                    texture.format = RenderTextureFormat.RFloat;
                    texture.hideFlags = HideFlags.HideAndDontSave;
                }
                if (buffer == null)
                {
                    buffer = new CommandBuffer();
                    buffer.name = "ModEditor NormalEditor";
                }
                window.camera.AddCommandBuffer(cameraEvent, buffer);
                refreshBuffer();
            }
        }

        void refreshBuffer()
        {
            if (buffer != null)
            {
                buffer.Clear();
            }
            if (texture == null || window.camera == null || window.Manager.Target == null || window.Manager.TargetChildren.Count == 0)
                return;
            buffer.SetRenderTarget(texture);
            buffer.ClearRenderTarget(true, true, Color.black);
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject target = window.Manager.TargetChildren[i];
                if (target == null)
                    continue;
                if (!window.Manager.ActionableDic[target])
                    continue;
                Renderer renderer = target.GetComponent<Renderer>();
                if (renderer == null)
                    continue;
                buffer.DrawRenderer(renderer, window.Mat_viewUtil, 0, 9);
            }
        }

        void updateMaterial()
        {
            screenTexcoord = window.camera.ScreenToViewportPoint(Event.current.mousePosition);
            screenTexcoord.y = 1 - screenTexcoord.y;
            screenTexcoord.z = (float)Screen.width / Screen.height;
            window.Mat_viewUtil.SetVector("_MousePos", screenTexcoord);
            window.Mat_viewUtil.SetFloat("_SelcetDis", window.Manager.BrushSize);
            SceneView.RepaintAll();
        }

        List<(Transform, Mesh, SkinnedMeshRenderer)> recordObjInOperation()
        {
            if (window.camera == null || texture == null)
                return null;
            List<(Transform, Mesh, SkinnedMeshRenderer)> _objInOperation = new List<(Transform, Mesh, SkinnedMeshRenderer)>();
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
                    _objInOperation.Add((target.transform, meshFilter.sharedMesh, null));
                }
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    window.SetEditingMesh(target, skinnedMeshRenderer);
                    _objInOperation.Add((target.transform, skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer));
                }
            }
            return _objInOperation;
        }
    }
}