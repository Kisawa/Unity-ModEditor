using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModEditor
{
    public partial class ModEditorWindow : EditorWindow
    {
        public static string ModEditorPath = "Assets/ModEditor";

        public static ModEditorWindow Self;

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

        public event Action onTabChanged;
        public event Action onWindowFocus;
        public event Action onWindowLostFocus;
        public event Action onRefreshTargetDic;
        public event Action<GameObject> onTargetChanged;
        
        [MenuItem("Tools/Mod Editor %#E")]
        static void Open()
        {
            RefreshModEditorPath();

            ModEditorWindow window = GetWindow<ModEditorWindow>("Mod Editor");
            if (!window.Manager.LockTarget)
            {
                window.Manager.Target = Selection.activeGameObject;
                window.refreshObjDic();
            }
            window.minSize = new Vector2(330, 700);
        }

        static void RefreshModEditorPath()
        {
            ModEditorManager test = CreateInstance<ModEditorManager>();
            MonoScript monoScript = MonoScript.FromScriptableObject(test);
            string path = AssetDatabase.GetAssetPath(monoScript);
            int index = path.IndexOf("/Editor/ModEditorManager.cs");
            path = path.Substring(0, index);
            ModEditorPath = path;
            DestroyImmediate(test);
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
                if (value == 0)
                    TabType = ModEditorTabType.SceneCollection;
                else if (value == 1)
                    TabType = ModEditorTabType.VertexBrush;
                else
                    TabType = ModEditorTabType.TextureBrush;
                if (_tabIndex != value)
                {
                    if (_tabIndex < tabs.Count && _tabIndex >= 0)
                        tabs[_tabIndex].OnLostFocus();
                    if (value < tabs.Count && value >= 0)
                        tabs[value].OnFocus();
                    _tabIndex = value;
                    onTabChanged?.Invoke();
                }
            }
        }

        List<WindowTabBase> tabs;

        public SceneCollectionTab Tab_SceneCollection
        {
            get 
            {
                if (tabs == null || tabs.Count < 1)
                    return null;
                return tabs[0] as SceneCollectionTab;
            }
        }

        public VertexBrushTab Tab_VertexBrush
        {
            get
            {
                if (tabs == null || tabs.Count < 2)
                    return null;
                return tabs[1] as VertexBrushTab;
            }
        }

        public TextureBrushTab Tab_TextureBrush
        {
            get
            {
                if (tabs == null || tabs.Count < 3)
                    return null;
                return tabs[2] as TextureBrushTab;
            }
        }

        public ModEditorTabType TabType { get; set; }

        Material mat_Util;
        public Material Mat_Util
        {
            get
            {
                if (mat_Util == null)
                    mat_Util = new Material(Shader.Find("Hidden/ModEditorUtil"));
                return mat_Util;
            }
        }

        public GUIContent lockContent { get; private set; }
        public GUIContent unlockContent { get; private set; }
        public GUIContent hiddenContent { get; private set; }
        public GUIContent viewContent { get; private set; }
        public GUIContent dropdownContent { get; private set; }
        public GUIContent dropdownRightContent { get; private set; }
        public GUIContent olToggleContent { get; private set; }
        public GUIContent olToggleOnContent { get; private set; }
        public GUIContent toggleContent { get; private set; }
        public GUIContent toggleOnContent { get; private set; }

        private void OnEnable()
        {
            RefreshModEditorPath();
            Self = this;
            tabs = new List<WindowTabBase>();
            tabs.Add(new SceneCollectionTab(this));
            tabs.Add(new VertexBrushTab(this));
            tabs.Add(new TextureBrushTab(this));
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
            if (olToggleContent == null)
                olToggleContent = EditorGUIUtility.IconContent("ol toggle");
            if (olToggleOnContent == null)
                olToggleOnContent = EditorGUIUtility.IconContent("ol toggle on");
            if (toggleContent == null)
                toggleContent = EditorGUIUtility.IconContent("ShurikenToggleHover");
            if (toggleOnContent == null)
                toggleOnContent = EditorGUIUtility.IconContent("ShurikenToggleFocusedOn");
            refreshWindow();
            enable_serializableData();
            targetChanged_serializableData();

            for (int i = 0; i < tabs.Count; i++)
                tabs[i].OnEnable();
            Selection.selectionChanged += selectionChanged;
            EditorApplication.hierarchyChanged += hierarchyChanged;
            Undo.undoRedoPerformed += undoRedoPerformed;
            EditorApplication.playModeStateChanged += playModeStateChanged;
            AssetModificationManagement.onWillSaveAssets += onWillSaveAssets;
            SceneView.duringSceneGui += duringSceneGui;
            registerEvent();
        }

        private void OnDisable()
        {
            Self = null;
            tabIndex = -1;
            for (int i = 0; i < tabs.Count; i++)
                tabs[i].OnDiable();
            Selection.selectionChanged -= selectionChanged;
            EditorApplication.hierarchyChanged -= hierarchyChanged;
            Undo.undoRedoPerformed -= undoRedoPerformed;
            EditorApplication.playModeStateChanged -= playModeStateChanged;
            AssetModificationManagement.onWillSaveAssets -= onWillSaveAssets;
            SceneView.duringSceneGui -= duringSceneGui;
            logoutEvent();
        }

        private void OnDestroy()
        {
            Manager.CheckAndClearExposed();
            ExposedManagement.CheckAndClearExposed();
        }

        private void OnFocus()
        {
            onWindowFocus?.Invoke();
        }

        private void OnLostFocus()
        {
            onWindowLostFocus?.Invoke();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Manager.LockTarget ? lockContent : unlockContent, "ObjectPickerTab"))
            {
                Manager.LockTarget = !Manager.LockTarget;
                if (!Manager.LockTarget)
                {
                    GameObject pre = Manager.Target;
                    Manager.Target = Selection.activeGameObject;
                    refreshObjDic();
                    if (pre != Manager.Target)
                    {
                        targetChanged_serializableData();
                        onTargetChanged?.Invoke(Manager.Target);
                    }
                }
            }
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(Manager.Target, typeof(GameObject), true);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Game Camera Follow", Manager.GameCameraFollow ? "WarningOverlay" : "ProjectBrowserHeaderBgTop"))
                Manager.GameCameraFollow = !Manager.GameCameraFollow;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            tabIndex = GUILayout.Toolbar(tabIndex, new string[] { "Scene Collection", "Vertex Brush", "Texture Brush" }, "SearchModeFilter");
            EditorGUILayout.Space(10);

            if (tabIndex < tabs.Count)
                tabs[tabIndex].Draw();
            if (GUI.changed)
                Validate();
        }

        private void OnInspectorUpdate()
        {
            for (int i = 0; i < tabs.Count; i++)
                tabs[i].OnInspectorUpdate();
        }

        public void Validate()
        {
            for (int i = 0; i < tabs.Count; i++)
                tabs[i].OnValidate();
            EditorUtility.SetDirty(Manager);
        }

        void selectionChanged()
        {
            if (!Manager.LockTarget)
            {
                GameObject pre = Manager.Target;
                Manager.Target = Selection.activeGameObject;
                refreshObjDic();
                if (pre != Manager.Target)
                {
                    targetChanged_serializableData();
                    onTargetChanged?.Invoke(Manager.Target);
                }
            }
            Repaint();
        }

        void hierarchyChanged()
        {
            if (currentScene != SceneManager.GetActiveScene())
                refreshWindow();
            else
                refreshObjDic();
            Repaint();
        }

        void undoRedoPerformed()
        {
            refreshObjDic();
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
                    applyPlayModeEditing();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    Manager.CheckAndClearExposed();
                    ExposedManagement.CheckAndClearExposed();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                default:
                    break;
            }
        }

        void refreshWindow()
        {
            currentScene = SceneManager.GetActiveScene();
            string path = $"{ModEditorPath}/ModEditorManager-{currentScene.name}.asset";
            Manager = AssetDatabase.LoadAssetAtPath<ModEditorManager>(path);
            if (Manager == null)
            {
                Manager = CreateInstance<ModEditorManager>();
                AssetDatabase.CreateAsset(Manager, path);
                AssetDatabase.SetMainObject(Manager, path);
                AssetDatabase.ImportAsset(path);
            }
            refreshObjDic();
        }
        
        void refreshObjDic()
        {
            if (Manager.Target == null)
            {
                if (Manager.TargetChildren != null)
                    Manager.TargetChildren.Clear();
            }
            else
            {
                Manager.TargetChildren = Manager.Target.GetComponentsInChildren<Renderer>(true).Select(x => x.gameObject).ToList();
                for (int i = 0; i < Manager.TargetChildren.Count; i++)
                {
                    GameObject obj = Manager.TargetChildren[i];
                    if (!Manager.ActionableDic.ContainsKey(obj))
                        Manager.ActionableDic.Add(obj, true);
                }
            }
            refreshMeshDic();
            onRefreshTargetDic?.Invoke();
        }

        void refreshMeshDic()
        {
            for (int i = 0; i < Manager.MeshDic.count; i++)
            {
                GameObject obj = Manager.MeshDic.Resolve(i);
                if (obj == null)
                    continue;
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter != null)
                    Manager.MeshDic.Add(obj, meshFilter.sharedMesh);
                SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                    Manager.MeshDic.Add(obj, skinnedMeshRenderer.sharedMesh);
            }
        }

        public Mesh SetEditingMesh(GameObject target, MeshFilter meshFilter)
        {
            if (meshFilter.sharedMesh == null)
                return null;
            Mesh mesh = Instantiate(meshFilter.sharedMesh);
            mesh.name = target.name + "-Editing";
            if (Manager.MeshDic.ContainsKey(target))
                Manager.MeshDic.Add(target, mesh);
            else
                Manager.MeshDic.Add(target, mesh, meshFilter.sharedMesh);
            Undo.RecordObject(meshFilter, "ModEditor MeshEditing");
            meshFilter.sharedMesh = mesh;
            EditorUtility.SetDirty(target);
            return mesh;
        }
        
        public Mesh SetEditingMesh(GameObject target, SkinnedMeshRenderer skinnedMeshRenderer)
        {
            if (skinnedMeshRenderer.sharedMesh == null)
                return null;
            Mesh mesh = Instantiate(skinnedMeshRenderer.sharedMesh);
            mesh.name = target.name + "-Editing";
            if (Manager.MeshDic.ContainsKey(target))
                Manager.MeshDic.Add(target, mesh);
            else
                Manager.MeshDic.Add(target, mesh, skinnedMeshRenderer.sharedMesh);
            Undo.RecordObject(skinnedMeshRenderer, "ModEditor MeshEditing");
            skinnedMeshRenderer.sharedMesh = mesh;
            EditorUtility.SetDirty(target);
            return mesh;
        }

        void applyPlayModeEditing()
        {
            for (int i = 0; i < Manager.MeshDic.count; i++)
            {
                GameObject obj = Manager.MeshDic.Resolve(i);
                if (obj == null)
                    continue;
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter != null)
                    meshFilter.sharedMesh = Manager.MeshDic[obj, true];
                SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                    skinnedMeshRenderer.sharedMesh = Manager.MeshDic[obj, true];
                EditorUtility.SetDirty(obj);
            }
        }
    }
}