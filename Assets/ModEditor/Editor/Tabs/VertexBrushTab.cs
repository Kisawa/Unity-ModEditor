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
            EditorEvent.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.Control.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.Alt.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.ControlAndAlt.OnMouse.UpLeft += OnMouse_UpLeft;
            EditorEvent.OnMouse.DragLeft += OnMouse_DragLeft;
            EditorEvent.Use.OnMouse.DownScroll += OnMouse_Scroll;
            EditorEvent.Use.OnMouse.UpScroll += OnMouse_Scroll;
            if (brushCursor == null)
                brushCursor = AssetDatabase.LoadAssetAtPath<Texture2D>($"{ModEditorWindow.ModEditorPath}/Textures/brushCursor.png");
        }

        public override void OnLostFocus()
        {
            base.OnLostFocus();
            window.onSceneValidate -= onSceneValidate;
            EditorEvent.OnMouse.DownLeft -= OnMouse_DownLeft;
            EditorEvent.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.Control.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.Alt.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.ControlAndAlt.OnMouse.UpLeft -= OnMouse_UpLeft;
            EditorEvent.OnMouse.DragLeft -= OnMouse_DragLeft;
            EditorEvent.Use.OnMouse.DownScroll -= OnMouse_Scroll;
            EditorEvent.Use.OnMouse.UpScroll -= OnMouse_Scroll;
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

            if (GUILayout.Button("asd"))
                func();
            index = EditorGUILayout.IntSlider(index, 0, 3);
        }

        int index = 0;
        void func()
        {
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject target = window.Manager.TargetChildren[i];
                MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    Mesh mesh = meshFilter.sharedMesh;
                    CalcUtil.Self.CalcVertexShader.SetInt("_Index", index);
                    ComputeBuffer rw_selects = new ComputeBuffer(mesh.vertexCount, sizeof(float));
                    float[] selects = Enumerable.Repeat(0f, mesh.vertexCount).ToArray();
                    selects[index] = 1;
                    selects[1] = -1;
                    rw_selects.SetData(selects);
                    CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SpreadSelectInTirangle, "RW_Selects", rw_selects);
                    ComputeBuffer _triangles = new ComputeBuffer(mesh.triangles.Length, sizeof(int));
                    _triangles.SetData(mesh.triangles);
                    CalcUtil.Self.CalcVertexShader.SetBuffer(CalcUtil.Self.kernel_SpreadSelectInTirangle, "_Triangles", _triangles);
                    CalcUtil.Self.CalcVertexShader.Dispatch(CalcUtil.Self.kernel_SpreadSelectInTirangle, Mathf.CeilToInt((float)mesh.triangles.Length / 1024), 1, 1);
                    float[] res = new float[mesh.vertexCount];
                    rw_selects.GetData(res);
                    for (int j = 0; j < res.Length; j++)
                    {
                        Debug.LogError(res[j]);
                    }
                    rw_selects.Dispose();
                    _triangles.Dispose();
                }
            }
        }

        private void OnMouse_DownLeft()
        {
            if (window.OnSceneGUI)
                return;
            objInOperation = recordObjInOperation();
            write();
        }

        private void OnMouse_UpLeft()
        {
            objInOperation = null;
        }

        private void OnMouse_DragLeft()
        {
            write();
        }

        private void OnMouse_Scroll() { }

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