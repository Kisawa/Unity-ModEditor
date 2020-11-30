using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModEditor
{
    public class ModEditorWindow : EditorWindow
    {
        const string managerPath = "Assets/ModEditor/";
        public static ExposedManagement ExposedManagement { get; private set; }
        public ModEditorManager Manager { get; private set; }

        [MenuItem("Tools/Mod Editor")]
        static void Open()
        {
            GetWindow<ModEditorWindow>("Mod Editor");
        }

        Scene currentScene;

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
            refreshWindow();
            Undo.undoRedoPerformed += Manager.RefreshObjDic;
        }

        private void OnEnable()
        {
            Manager.Target = Selection.activeGameObject;
            EditorApplication.hierarchyChanged += hierarchyChanged;
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= hierarchyChanged;
        }

        private void OnDestroy()
        {
            Undo.undoRedoPerformed -= Manager.RefreshObjDic;
        }

        private void OnFocus()
        {
            Manager.Target = Selection.activeGameObject;
        }

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
            if (GUI.changed)
                EditorUtility.SetDirty(Manager);
        }

        private void OnSelectionChange()
        {
            Manager.Target = Selection.activeGameObject;
            Repaint();
        }

        void hierarchyChanged()
        {
            if (currentScene != SceneManager.GetActiveScene())
                refreshWindow();
            Repaint();
        }

        void refreshWindow()
        {
            currentScene = SceneManager.GetActiveScene();
            
            GameObject ExposedManagementObj = GameObject.Find("ExposedManagement");
            if (ExposedManagementObj == null)
            {
                ExposedManagementObj = new GameObject("ExposedManagement");
                ExposedManagementObj.hideFlags = HideFlags.HideInHierarchy;
                ExposedManagement = ExposedManagementObj.AddComponent<ExposedManagement>();
                AssetDatabase.DeleteAsset($"{managerPath}ModEditorManager-{currentScene.name}.asset");
            }
            else
            {
                ExposedManagement = ExposedManagementObj.GetComponent<ExposedManagement>();
                Manager = AssetDatabase.LoadAssetAtPath<ModEditorManager>($"{managerPath}ModEditorManager-{currentScene.name}.asset");
            }
            if (Manager == null)
            {
                Manager = CreateInstance<ModEditorManager>();
                AssetDatabase.CreateAsset(Manager, $"{managerPath}ModEditorManager-{currentScene.name}.asset");
                AssetDatabase.ImportAsset($"{managerPath}ModEditorManager-{currentScene.name}.asset");
            }

            Manager.RefreshObjDic();
        }
    }
}