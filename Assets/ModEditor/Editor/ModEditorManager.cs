using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class ModEditorManager : ScriptableObject
    {
        [SerializeField]
        bool lockTarget;
        public bool LockTarget 
        {
            get => lockTarget;
            set {
                Undo.RecordObject(this, "ModEditor Change LockTarget");
                lockTarget = value;
            }
        }

        [SerializeField]
        ExposedReference<GameObject> target;
        public GameObject Target
        {
            get
            {
                return target.Resolve(ModEditorWindow.ExposedManagement);
            }
            set
            {
                if (LockTarget)
                    return;
                Undo.RecordObject(this, "ModEditor Change Target");
                target = ModEditorWindow.ExposedManagement.LinkExposedReference(value);
                RefreshObjDic();
            }
        }

        [SerializeField]
        UndoClass.Dictionary_Obj_Bool actionableDic;
        public UndoClass.Dictionary_Obj_Bool ActionableDic { get => actionableDic; }

        public List<GameObject> TargetChildren { get; private set; }

        private void Awake()
        {
            if (actionableDic == null)
                actionableDic = new UndoClass.Dictionary_Obj_Bool(this);
        }

        private void OnEnable()
        {
            RefreshObjDic();
            EditorApplication.hierarchyChanged += RefreshObjDic;
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= RefreshObjDic;
        }

        public void RefreshObjDic()
        {
            if (Target == null)
            {
                if(TargetChildren != null)
                    TargetChildren.Clear();
                return;
            }
            TargetChildren = Target.GetComponentsInChildren<Renderer>().Select(x => x.gameObject).ToList();
            for (int i = 0; i < TargetChildren.Count; i++)
            {
                GameObject obj = TargetChildren[i];
                if (!ActionableDic.ContainsKey(obj))
                    ActionableDic.Add(obj, true);
            }
        }
    }
}