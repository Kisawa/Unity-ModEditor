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
        GameObject target;
        public GameObject Target
        {
            get => target;
            set
            {
                if (LockTarget)
                    return;
                Undo.RecordObject(this, "ModEditor Change Target");
                target = value;
                RefreshObjDic();
            }
        }

        public List<GameObject> TargetChildren;

        [SerializeField]
        UndoClass.Dictionary_Obj_Bool actionableDic;
        public UndoClass.Dictionary_Obj_Bool ActionableDic 
        {
            get 
            {
                if (actionableDic == null)
                    actionableDic = new UndoClass.Dictionary_Obj_Bool(this);
                return actionableDic;
            }
            private set
            {
                actionableDic = value;
            }
        }

        private void Awake()
        {
            actionableDic = new UndoClass.Dictionary_Obj_Bool(this);
        }

        private void OnEnable()
        {
            EditorApplication.hierarchyChanged += hierarchyChanged;
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= hierarchyChanged;
        }

        void hierarchyChanged()
        {
            RefreshObjDic(false);
        }

        public void RefreshObjDic(bool targetChange = true)
        {
            if (Target == null)
                return;
            TargetChildren = Target.GetComponentsInChildren<Renderer>().Select(x => x.gameObject).ToList();
            if (targetChange)
            {
                UndoClass.Dictionary_Obj_Bool _actionableDic = new UndoClass.Dictionary_Obj_Bool();
                for (int i = 0; i < TargetChildren.Count; i++)
                {
                    GameObject obj = TargetChildren[i];
                    if (ActionableDic.TryGetValue(obj, out bool actionable))
                        _actionableDic.Add(obj, actionable);
                    else
                        _actionableDic.Add(obj, true);
                }
                _actionableDic.parent = this;
                Undo.RecordObject(this, "ModEditor Change ActionableDic");
                ActionableDic = _actionableDic;
            }
            else
            {
                for (int i = 0; i < TargetChildren.Count; i++)
                {
                    GameObject obj = TargetChildren[i];
                    if (!ActionableDic.ContainsKey(obj))
                        ActionableDic.Add(obj, true);
                }
            }
        }
    }
}