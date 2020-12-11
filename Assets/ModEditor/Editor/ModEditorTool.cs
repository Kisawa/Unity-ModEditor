using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace ModEditor
{
    [EditorTool("ModEditor Tool")]
    public class ModEditorTool : EditorTool
    {
        GUIContent icon;
        public override GUIContent toolbarIcon => icon;

        Color vertexColor = Color.black;
        Color VertexColor 
        {
            get 
            {
                if (ModEditorWindow.Self != null && ModEditorWindow.Self.Manager != null)
                    vertexColor = ModEditorWindow.Self.Manager.VertexColor;
                return vertexColor;
            }
            set 
            {
                if (ModEditorWindow.Self == null || ModEditorWindow.Self.Manager == null)
                    return;
                vertexColor = value;
                ModEditorWindow.Self.Manager.VertexColor = vertexColor;
            }
        }

        float vertexScale = 0.5f;
        float VertexScale
        {
            get 
            {
                if (ModEditorWindow.Self != null && ModEditorWindow.Self.Manager != null)
                    vertexScale = ModEditorWindow.Self.Manager.VertexScale;
                return vertexScale;
            }
            set
            {
                if (ModEditorWindow.Self == null || ModEditorWindow.Self.Manager == null)
                    return;
                vertexScale = value;
                ModEditorWindow.Self.Manager.VertexScale = vertexScale;
            }
        }

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
                if (ModEditorWindow.Self != null && ModEditorWindow.Self.Manager != null)
                    ModEditorWindow.Self.Validate();
            Handles.EndGUI();
        }

        public override bool IsAvailable()
        {
            if (ModEditorWindow.Self != null && ModEditorWindow.Self.Manager.Target != null && ModEditorWindow.Self.Manager.Target == Selection.activeGameObject)
                return true;
            return false;
        }
    }
}