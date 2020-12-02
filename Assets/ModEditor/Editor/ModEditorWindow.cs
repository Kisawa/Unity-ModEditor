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

        static ExposedManagement exposedManagement;
        public static ExposedManagement ExposedManagement 
        {
            get
            {
                GameObject ExposedManagementObj = GameObject.Find("ExposedManagement");
                if (ExposedManagementObj == null)
                {
                    ExposedManagementObj = new GameObject("ExposedManagement");
                    ExposedManagementObj.hideFlags = HideFlags.HideInHierarchy;
                    exposedManagement = ExposedManagementObj.AddComponent<ExposedManagement>();
                }
                else
                {
                    exposedManagement = ExposedManagementObj.GetComponent<ExposedManagement>();
                }
                return exposedManagement;
            }
        }

        public ModEditorManager Manager { get; private set; }

        [MenuItem("Tools/Mod Editor")]
        static void Open()
        {
            ModEditorWindow window = GetWindow<ModEditorWindow>("Mod Editor");
            window.Manager.Target = Selection.activeGameObject;
            window.minSize = new Vector2(330, 700);
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
        public GUIContent dropdownContent { get; private set; }
        public GUIContent dropdownRightContent { get; private set; }

        private void OnEnable()
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
            if (dropdownContent == null)
                dropdownContent = EditorGUIUtility.IconContent("d_dropdown");
            if (dropdownRightContent == null)
                dropdownRightContent = EditorGUIUtility.IconContent("d_scrollright");
            refreshWindow();
            Selection.selectionChanged += selectionChanged;
            EditorApplication.hierarchyChanged += hierarchyChanged;
            Undo.undoRedoPerformed += undoRedoPerformed;
            EditorApplication.playModeStateChanged += playModeStateChanged;
            AssetModificationManagement.onWillSaveAssets += onWillSaveAssets;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= selectionChanged;
            EditorApplication.hierarchyChanged -= hierarchyChanged;
            Undo.undoRedoPerformed -= undoRedoPerformed;
            EditorApplication.playModeStateChanged -= playModeStateChanged;
            AssetModificationManagement.onWillSaveAssets -= onWillSaveAssets;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(Manager.LockTarget ? lockContent : unlockContent, "ObjectPickerTab"))
            {
                Manager.LockTarget = !Manager.LockTarget;
                if (!Manager.LockTarget)
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
            {
                EditorUtility.SetDirty(Manager);
            }
        }

        private void OnInspectorUpdate()
        {
            for (int i = 0; i < tabs.Count; i++)
                tabs[i].OnInspectorUpdate();
        }

        void selectionChanged()
        {
            Manager.Target = Selection.activeGameObject;
            Repaint();
        }

        void hierarchyChanged()
        {
            if (currentScene != SceneManager.GetActiveScene())
                refreshWindow();
            Manager.RefreshObjDic();
            Repaint();
        }

        void undoRedoPerformed()
        {
            Manager.RefreshObjDic();
            Repaint();
        }

        private void onWillSaveAssets(string[] path)
        {
            if (path.Contains(currentScene.path))
            {
                Manager.CheckAndClearExposed();
                ExposedManagement.CheckAndClearExposed();
            }
        }

        private void playModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    Manager.CheckAndClearExposed();
                    ExposedManagement.CheckAndClearExposed();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    break;
            }
        }

        void refreshWindow()
        {
            currentScene = SceneManager.GetActiveScene();
            Manager = AssetDatabase.LoadAssetAtPath<ModEditorManager>($"{managerPath}ModEditorManager-{currentScene.name}.asset");
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