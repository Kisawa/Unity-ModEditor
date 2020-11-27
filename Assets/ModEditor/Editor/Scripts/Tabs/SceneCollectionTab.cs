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
                    if (obj == window.Manager.Target)
                        continue;
                    int indent = 1;
                    Transform parent = obj.transform.parent;
                    while (parent != window.Manager.Target.transform)
                    {
                        indent++;
                        parent = parent.parent;
                    }
                    EditorGUI.indentLevel = indent;
                    EditorGUILayout.BeginHorizontal();
                    bool actionable = window.Manager.ActionableDic[obj];
                    if (GUILayout.Button(actionable ? window.viewContent : window.hiddenContent, "ObjectPickerTab"))
                        window.Manager.ActionableDic[obj] = !actionable;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}