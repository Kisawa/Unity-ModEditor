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
            bool res = window.Manager.Target != null && window.Manager.TargetChildren.Contains(window.Manager.Target);
            if (res)
            {
                bool activeable = window.Manager.ActionableDic[window.Manager.Target];
                if (GUILayout.Button(activeable ? window.viewContent : window.hiddenContent, "ObjectPickerTab"))
                    window.Manager.ActionableDic[window.Manager.Target] = !activeable;
            }
            EditorGUI.BeginDisabledGroup(true);
            if (!res)
            {
                EditorGUI.indentLevel++;
                GUILayout.Button("", "ObjectPickerTab");
            }
            EditorGUILayout.ObjectField(window.Manager.Target, typeof(GameObject), true);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            if (!res)
                EditorGUI.indentLevel--;

            if (window.Manager.Target != null)
            {
                int originIndent = EditorGUI.indentLevel;
                List<GameObject> placeholdeGameObjectList = new List<GameObject>();
                for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
                {
                    GameObject obj = window.Manager.TargetChildren[i];
                    if (obj == window.Manager.Target)
                        continue;
                    EditorGUI.indentLevel = originIndent + 1;
                    Transform parent = obj.transform.parent;
                    while (parent != window.Manager.Target.transform)
                    {
                        EditorGUI.indentLevel++;
                        parent = parent.parent;
                    }
                    parent = obj.transform.parent;
                    if (parent.gameObject != window.Manager.Target && obj.transform.parent == parent && !window.Manager.TargetChildren.Contains(parent.gameObject) && !placeholdeGameObjectList.Contains(parent.gameObject))
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.Button("", "ObjectPickerTab");
                        EditorGUILayout.ObjectField(parent.gameObject, typeof(GameObject), true);
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                        placeholdeGameObjectList.Add(parent.gameObject);
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
                EditorGUI.indentLevel = originIndent;
            }

            EditorGUILayout.EndVertical();
        }
    }
}