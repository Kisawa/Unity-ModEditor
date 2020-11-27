using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class ModEditorWindow : EditorWindow
    {
        const string ManagerPath = "Assets/ModEditor/ModEditorDefaultManager.asset";
        public ModEditorManager Manager { get; private set; }

        [MenuItem("Tools/Mod Editor")]
        static void Open()
        {
            GetWindow<ModEditorWindow>("Mod Editor");
        }

        int _tabIndex = -1;
        int tabIndex
        {
            get
            {
                if (_tabIndex < 0)
                    return 0;
                return _tabIndex;
            }
            set
            {
                if (_tabIndex != value)
                {
                    if (_tabIndex < tabs.Count && _tabIndex >= 0)
                        tabs[_tabIndex].OnLostFocus();
                    if (value < tabs.Count && value >= 0)
                        tabs[value].OnFocus();
                }
                _tabIndex = value;
            }
        }
        List<WindowTabBase> tabs;

        public GUIContent lockContent { get; private set; }
        public GUIContent unlockContent { get; private set; }
        public GUIContent hiddenContent { get; private set; }
        public GUIContent viewContent { get; private set; }

        private void Awake()
        {
            tabs = new List<WindowTabBase>();
            tabs.Add(new SceneCollectionTab(this));
            tabs.Add(new NormalEditorTab(this));
            if (lockContent == null)
                lockContent = EditorGUIUtility.IconContent("IN LockButton on act");
            if (unlockContent == null)
                unlockContent = EditorGUIUtility.IconContent("IN LockButton");
            if (hiddenContent == null)
                hiddenContent = EditorGUIUtility.IconContent("d_scenevis_hidden-mixed");
            if (viewContent == null)
                viewContent = EditorGUIUtility.IconContent("d_scenevis_visible-mixed");
            if (Manager == null)
            {
                Manager = AssetDatabase.LoadAssetAtPath<ModEditorManager>(ManagerPath);
                if (Manager == null)
                {
                    Manager = CreateInstance<ModEditorManager>();
                    AssetDatabase.CreateAsset(Manager, ManagerPath);
                    AssetDatabase.ImportAsset(ManagerPath);
                }
            }
        }

        private void OnEnable()
        {
            Manager.Target = Selection.activeGameObject;
            EditorApplication.hierarchyChanged += Repaint;
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= Repaint;
        }

        private void OnFocus()
        {
            Manager.Target = Selection.activeGameObject;
        }

        public ExposedReference<Transform> exposedReference;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(Manager.LockTarget ? lockContent : unlockContent, "ObjectPickerTab"))
            {
                Manager.LockTarget = !Manager.LockTarget;
                Manager.Target = Selection.activeGameObject;
            }
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(Manager.Target, typeof(GameObject), true);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            tabIndex = GUILayout.Toolbar(tabIndex, new string[] { "Scene Collection", "Normal Editor", "Other" }, "SearchModeFilter");
            EditorGUILayout.Space(10);
            if (tabIndex < tabs.Count)
            {
                tabs[tabIndex].Draw();
            }
        }

        private void OnSelectionChange()
        {
            Manager.Target = Selection.activeGameObject;
            Repaint();
        }
    }
}