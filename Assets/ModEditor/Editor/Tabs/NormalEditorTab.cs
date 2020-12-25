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

        Dictionary<Transform, Mesh> objInOperation;

        public override void OnFocus()
        {
            base.OnFocus();
            window.onSceneGUI += onSceneGUI;
            if (brushCursor == null)
                brushCursor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{ModEditorWindow.ModEditorPath}Textures/brushCursor.png");
        }

        public override void OnLostFocus()
        {
            base.OnLostFocus();
            window.onSceneGUI -= onSceneGUI;
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

        void updateMaterial()
        {
            window.Mat_viewUtil.SetVector("_MousePos", window.ScreenTexcoord);
            window.Mat_viewUtil.SetFloat("_SelcetDis", window.Manager.BrushSize);
            SceneView.RepaintAll();
        }

        Dictionary<Transform, Mesh> recordObjInOperation()
        {
            if (window.camera == null)
                return null;
            Dictionary<Transform, Mesh> _objInOperation = new Dictionary<Transform, Mesh>();
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
                    _objInOperation.Add(target.transform, meshFilter.sharedMesh);
                }
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    window.SetEditingMesh(target, skinnedMeshRenderer);
                    _objInOperation.Add(target.transform, skinnedMeshRenderer.sharedMesh);
                }
            }
            return _objInOperation;
        }
    }
}