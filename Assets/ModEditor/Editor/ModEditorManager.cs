using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class ModEditorManager : ScriptableObject
    {
        public event Action onRefreshTargetDic;
        public event Action onNormalViewChanged;
        public event Action onTangentViewChanged;
        public event Action onGridViewChanged;
        public event Action onUVViewChanged;

        [SerializeField]
        bool lockTarget;
        public bool LockTarget 
        {
            get => lockTarget;
            set {
                Undo.RecordObject(this, "ModEditor LockTarget");
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
                target = ModEditorWindow.ExposedManagement.LinkExposedReference(value);
                RefreshObjDic();
            }
        }

        [SerializeField]
        UndoClass.Dictionary_Obj_Bool actionableDic;
        public UndoClass.Dictionary_Obj_Bool ActionableDic { get => actionableDic; }

        public List<GameObject> TargetChildren { get; private set; }

        #region Normal Shading
        [SerializeField]
        bool normalView;
        public bool NormalView 
        {
            get => normalView;
            set 
            {
                Undo.RecordObject(this, "ModEditor NormalView");
                normalView = value;
                onNormalViewChanged?.Invoke();
            }
        }

        [SerializeField]
        bool normalViewUnfold = true;
        public bool NormalViewUnfold
        {
            get => normalViewUnfold;
            set
            {
                Undo.RecordObject(this, "ModEditor NormalViewUnfold");
                normalViewUnfold = value;
            }
        }

        [SerializeField]
        Color normalColor = Color.red;
        public Color NormalColor
        {
            get => normalColor;
            set
            {
                Undo.RecordObject(this, "ModEditor NormalColor");
                normalColor = value;
            }
        }

        [SerializeField]
        float normalLength = 0.5f;
        public float NormalLength
        {
            get => normalLength;
            set
            {
                Undo.RecordObject(this, "ModEditor NormalLength");
                normalLength = value;
            }
        }
        #endregion

        #region Tangent Shading
        [SerializeField]
        bool tangentView;
        public bool TangentView
        {
            get => tangentView;
            set
            {
                Undo.RecordObject(this, "ModEditor TangentView");
                tangentView = value;
                onTangentViewChanged?.Invoke();
            }
        }

        [SerializeField]
        bool tangentViewUnfold = true;
        public bool TangentViewUnfold
        {
            get => tangentViewUnfold;
            set
            {
                Undo.RecordObject(this, "ModEditor TangentViewUnfold");
                tangentViewUnfold = value;
            }
        }

        [SerializeField]
        Color tangentColor = Color.green;
        public Color TangentColor
        {
            get => tangentColor;
            set
            {
                Undo.RecordObject(this, "ModEditor TangentColor");
                tangentColor = value;
            }
        }

        [SerializeField]
        float tangentLength = 0.5f;
        public float TangentLength
        {
            get => tangentLength;
            set
            {
                Undo.RecordObject(this, "ModEditor TangentLength");
                tangentLength = value;
            }
        }

        [SerializeField]
        float arrowLength = 0.2f;
        public float ArrowLength
        {
            get => arrowLength;
            set
            {
                Undo.RecordObject(this, "ModEditor ArrowLength");
                arrowLength = value;
            }
        }

        [SerializeField]
        float arrowSize = 0.1f;
        public float ArrowSize
        {
            get => arrowSize;
            set
            {
                Undo.RecordObject(this, "ModEditor ArrowSize");
                arrowSize = value;
            }
        }
        #endregion

        #region Grid Shading
        [SerializeField]
        bool gridView;
        public bool GridView
        {
            get => gridView;
            set
            {
                Undo.RecordObject(this, "ModEditor GridView");
                gridView = value;
                onGridViewChanged?.Invoke();
            }
        }

        [SerializeField]
        bool gridViewUnfold = true;
        public bool GridViewUnfold
        {
            get => gridViewUnfold;
            set
            {
                Undo.RecordObject(this, "ModEditor GridViewUnfold");
                gridViewUnfold = value;
            }
        }

        [SerializeField]
        Color gridColor = Color.white;
        public Color GridColor
        {
            get => gridColor;
            set
            {
                Undo.RecordObject(this, "ModEditor GridColor");
                gridColor = value;
            }
        }
        #endregion

        #region UV Shading
        [SerializeField]
        bool uvView;
        public bool UVView
        {
            get => uvView;
            set
            {
                Undo.RecordObject(this, "ModEditor UVView");
                uvView = value;
                onUVViewChanged?.Invoke();
            }
        }

        [SerializeField]
        bool uvViewUnfold = true;
        public bool UVViewUnfold
        {
            get => uvViewUnfold;
            set
            {
                Undo.RecordObject(this, "ModEditor UVViewUnfold");
                uvViewUnfold = value;
            }
        }

        [SerializeField]
        float uvAlpha = 1;
        public float UVAlpha
        {
            get => uvAlpha;
            set
            {
                Undo.RecordObject(this, "ModEditor UVAlpha");
                uvAlpha = value;
            }
        }
        #endregion

        private void Awake()
        {
            if (actionableDic == null)
                actionableDic = new UndoClass.Dictionary_Obj_Bool(this);
        }

        public void RefreshObjDic()
        {
            if (Target == null)
            {
                if (TargetChildren != null)
                    TargetChildren.Clear();
            }
            else
            {
                TargetChildren = Target.GetComponentsInChildren<Renderer>().Select(x => x.gameObject).ToList();
                for (int i = 0; i < TargetChildren.Count; i++)
                {
                    GameObject obj = TargetChildren[i];
                    if (!ActionableDic.ContainsKey(obj))
                        ActionableDic.Add(obj, true);
                }
            }
            onRefreshTargetDic?.Invoke();
        }

        public void CheckAndClearExposed()
        {
            if (ModEditorWindow.ExposedManagement.CheckAndClearExposed(target.exposedName))
                target = default;
            ActionableDic.CheckAndClearExposed();
            EditorUtility.SetDirty(this);
        }
    }
}