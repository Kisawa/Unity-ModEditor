using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Rendering;

namespace ModEditor
{
    [EditorTool("ModEditor Tool")]
    public class ModEditorTool : EditorTool
    {
        static ModEditorWindow ModEditor => ModEditorWindow.Self;

        static List<CalcShaderData.CalcVertexsData> calcShaderDatas;
        public static List<CalcShaderData.CalcVertexsData> CalcShaderDatas
        {
            get
            {
                if (calcShaderDatas == null)
                    calcShaderDatas = new List<CalcShaderData.CalcVertexsData>();
                return calcShaderDatas;
            }
        }

        static bool changed;

        GUIContent icon;
        public override GUIContent toolbarIcon => icon;

        Color vertexColor = Color.black;
        Color VertexColor
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    vertexColor = ModEditor.Manager.VertexColor;
                return vertexColor;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                vertexColor = value;
                ModEditor.Manager.VertexColor = vertexColor;
            }
        }

        float vertexScale = 0.5f;
        float VertexScale
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    vertexScale = ModEditor.Manager.VertexScale;
                return vertexScale;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                vertexScale = value;
                ModEditor.Manager.VertexScale = vertexScale;
            }
        }

        Vector3 screenTexcoord;
        Vector3 ScreenTexcoord
        {
            get
            {
                if (ModEditor != null)
                    screenTexcoord = ModEditor.ScreenTexcoord;
                return screenTexcoord;
            }
        }

        float brushSize = 0.05f;
        float BrushSize
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushSize = ModEditor.Manager.BrushSize;
                return brushSize;
            }
        }

        Camera camera;
        CommandBuffer buffer;
        CameraEvent cameraEvent = CameraEvent.AfterForwardAlpha;

        private void OnEnable()
        {
            icon = EditorGUIUtility.IconContent("d_Mesh Icon");
        }

        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(window.position.width - 220, window.position.height - 90, 210, 60));
            EditorGUILayout.BeginVertical("dragtabdropwindow");
            GUILayout.Label("Vertex View", "LODRenderersText");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vertex's Color", "AboutWIndowLicenseLabel", GUILayout.Width(75));
            VertexColor = EditorGUILayout.ColorField(new GUIContent(), VertexColor, true, false, false, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vertex Scale", "AboutWIndowLicenseLabel", GUILayout.Width(75));
            VertexScale = EditorGUILayout.Slider(VertexScale, 0.001f, 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
            if (GUI.changed)
                if (ModEditor != null && ModEditor.Manager != null)
                    ModEditor.Validate();
            Handles.EndGUI();
            if (ModEditor != null)
                GUIUpdate((window as SceneView).camera);
        }

        public override bool IsAvailable()
        {
            if (ModEditor != null && ModEditor.Manager.Target != null && (ModEditor.Manager.LockTarget || ModEditor.Manager.Target == Selection.activeGameObject))
                return true;
            return false;
        }

        void GUIUpdate(Camera cam)
        {
            if (camera != cam)
            {
                if (camera != null && buffer != null)
                    camera.RemoveCommandBuffer(cameraEvent, buffer);
                if (buffer == null)
                    buffer = new CommandBuffer();
                else
                    buffer.Clear();
                camera = cam;
                camera.AddCommandBuffer(cameraEvent, buffer);
            }
            updateBuffer();
        }

        void updateBuffer()
        {
            if (changed)
                buffer.Clear();
            bool brushOn = ModEditor.TabType == ModEditorTabType.NormalEditor;
            for (int i = 0; i < CalcShaderDatas.Count; i++)
            {
                CalcShaderData.CalcVertexsData data = CalcShaderDatas[i];
                if (brushOn)
                    data.Update(camera, ScreenTexcoord, BrushSize);
                if (data.IsAvailable)
                {
                    data.material.SetInt("_BrushOn", brushOn ? 1 : 0);
                    data.material.SetColor("_VertexColor", VertexColor);
                    data.material.SetFloat("_VertexScale", VertexScale);
                    if (changed)
                        buffer.DrawRenderer(data.renderer, data.material, 0, 0);
                }
            }
            changed = false;
        }

        static void addCalcShaderRender(Renderer renderer, MeshFilter meshFilter)
        {
            if (renderer == null || meshFilter == null || meshFilter.sharedMesh == null)
                return;
            Material material = new Material(Shader.Find("Hidden/ModEditorVertexView"));
            CalcShaderData.CalcVertexsData data;
            switch (ModEditor.Manager.BrushType)
            {
                case BrushType.ScreenScope:
                    data = new CalcShaderData.CalcMeshVertexsData_ScreenScope(renderer, meshFilter);
                    data.BindMaterial(material);
                    CalcShaderDatas.Add(data);
                    break;
            }
        }

        static void addCalcShaderRender(SkinnedMeshRenderer skinnedMesh)
        {
            if (skinnedMesh == null || skinnedMesh.sharedMesh == null)
                return;
            Material material = new Material(Shader.Find("Hidden/ModEditorVertexView"));
            CalcShaderData.CalcVertexsData data;
            switch (ModEditor.Manager.BrushType)
            {
                case BrushType.ScreenScope:
                    data = new CalcShaderData.CalcSkinnedMeshVertexsData_ScreenScope(skinnedMesh);
                    data.BindMaterial(material);
                    CalcShaderDatas.Add(data);
                    break;
            }
        }

        public static void RefreshCalcBuffer()
        {
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].Clear();
            CalcShaderDatas.Clear();
            for (int i = 0; i < ModEditor.Manager.TargetChildren.Count; i++)
            {
                GameObject target = ModEditor.Manager.TargetChildren[i];
                if (target == null)
                    continue;
                if (!ModEditor.Manager.ActionableDic[target])
                    continue;
                Renderer renderer = target.GetComponent<Renderer>();
                MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                addCalcShaderRender(renderer, meshFilter);
                addCalcShaderRender(renderer as SkinnedMeshRenderer);
            }
            changed = true;
        }

        public static void RemoveCalcBuffer()
        {
            for (int i = 0; i < CalcShaderDatas.Count; i++)
                CalcShaderDatas[i].Clear();
            CalcShaderDatas.Clear();
            changed = true;
        }
    }
}