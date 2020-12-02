using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class SceneCollectionTab : WindowTabBase
    {
        new ModEditorWindow window;

        public SceneCollectionTab(EditorWindow window) : base(window)
        {
            this.window = window as ModEditorWindow;
        }

        public override void Draw()
        {
            drawCollection();
            EditorGUILayout.Space(20);
            drawNormalViewUtil();
            EditorGUILayout.Space(10);
            drawTangentViewUtil();
            EditorGUILayout.Space(10);
            drawGridViewUtil();
            EditorGUILayout.Space(10);
            drawUVViewUtil();
        }

        public override void OnInspectorUpdate()
        {
            base.OnInspectorUpdate();
            
        }

        void drawCollection()
        {
            EditorGUILayout.BeginVertical("flow background");
            EditorGUILayout.BeginHorizontal();
            if (window.Manager.Target != null && window.Manager.TargetChildren.Contains(window.Manager.Target))
            {
                bool activeable = window.Manager.ActionableDic[window.Manager.Target];
                if (GUILayout.Button(activeable ? window.viewContent : window.hiddenContent, "ObjectPickerTab"))
                    window.Manager.ActionableDic[window.Manager.Target] = !activeable;
            }
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(window.Manager.Target, typeof(GameObject), true);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            if (window.Manager.Target != null)
            {
                for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
                {
                    GameObject obj = window.Manager.TargetChildren[i];
                    if (obj == window.Manager.Target || obj.gameObject == null)
                        continue;
                    EditorGUI.indentLevel = 1;
                    Transform parent = obj.transform.parent;
                    while (parent != window.Manager.Target.transform && parent != null)
                    {
                        EditorGUI.indentLevel++;
                        parent = parent.parent;
                    }
                    EditorGUILayout.BeginHorizontal();
                    bool actionable = window.Manager.ActionableDic[obj];
                    if (GUILayout.Button(actionable ? window.viewContent : window.hiddenContent, "ObjectPickerTab"))
                        window.Manager.ActionableDic[obj] = !actionable;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel = 0;
            }
            EditorGUILayout.EndVertical();
        }

        void drawNormalViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            window.Manager.NormalView = EditorGUILayout.Toggle(window.Manager.NormalView, "OL ToggleWhite", GUILayout.Width(20));
            if (GUILayout.Button("Normal View", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.NormalViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 195)))
                window.Manager.NormalViewUnfold = !window.Manager.NormalViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.NormalViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.NormalView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Normal Color", labelStyle, GUILayout.Width(120));
                window.Manager.NormalColor = EditorGUILayout.ColorField(window.Manager.NormalColor, GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Normal Length", labelStyle, GUILayout.Width(120));
                window.Manager.NormalLength = EditorGUILayout.Slider(window.Manager.NormalLength, 0.01f, 1, GUILayout.Width(170));
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawTangentViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            window.Manager.TangentView = EditorGUILayout.Toggle(window.Manager.TangentView, "OL ToggleWhite", GUILayout.Width(20));
            if (GUILayout.Button("Tangent View", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.TangentViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 195)))
                window.Manager.TangentViewUnfold = !window.Manager.TangentViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.TangentViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.TangentView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tangent Color", labelStyle, GUILayout.Width(120));
                window.Manager.TangentColor = EditorGUILayout.ColorField(window.Manager.TangentColor, GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tangent Length", labelStyle, GUILayout.Width(120));
                window.Manager.TangentLength = EditorGUILayout.Slider(window.Manager.TangentLength, 0.01f, 1, GUILayout.Width(170));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Binormal", GUI.skin.GetStyle("AnimationTimelineTick"));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Arrow Length", labelStyle, GUILayout.Width(120));
                window.Manager.ArrowLength = EditorGUILayout.Slider(window.Manager.ArrowLength, 0, 1, GUILayout.Width(170));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Arrow Size", labelStyle, GUILayout.Width(120));
                window.Manager.ArrowSize = EditorGUILayout.Slider(window.Manager.ArrowSize, 0, 1, GUILayout.Width(170));
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawGridViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            window.Manager.GridView = EditorGUILayout.Toggle(window.Manager.GridView, "OL ToggleWhite", GUILayout.Width(20));
            if (GUILayout.Button("Grid View", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.GridViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 195)))
                window.Manager.GridViewUnfold = !window.Manager.GridViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.GridViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.GridView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Grid Color", labelStyle, GUILayout.Width(120));
                window.Manager.GridColor = EditorGUILayout.ColorField(window.Manager.GridColor, GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawUVViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            window.Manager.UVView = EditorGUILayout.Toggle(window.Manager.UVView, "OL ToggleWhite", GUILayout.Width(20));
            if (GUILayout.Button("UV View", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.UVViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 195)))
                window.Manager.UVViewUnfold = !window.Manager.UVViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.UVViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.UVView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Alpha", labelStyle, GUILayout.Width(120));
                window.Manager.UVAlpha = EditorGUILayout.Slider(window.Manager.UVAlpha, 0, 1, GUILayout.Width(170));
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }
    }
}