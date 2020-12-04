﻿using System;
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
                if (value == lockTarget)
                    return;
                Undo.RecordObject(this, "ModEditor LockTarget");
                lockTarget = value;
            }
        }

        [SerializeField]
        ExposedReference<GameObject> target;
        public GameObject Target 
        {
            get => target.Resolve(ModEditorWindow.ExposedManagement);
            set
            {
                if (LockTarget)
                    return;
                target = ModEditorWindow.ExposedManagement.LinkExposedReference(value);
                EditorUtility.SetDirty(this);
            }
        }

        [SerializeField]
        UndoClass.Dictionary_Obj_Bool actionableDic;
        public UndoClass.Dictionary_Obj_Bool ActionableDic { get => actionableDic; }

        [SerializeField]
        UndoClass.Dictionary_Obj_Mesh meshDic;
        public UndoClass.Dictionary_Obj_Mesh MeshDic { get => meshDic; }

        public List<GameObject> TargetChildren { get; set; }

        #region Normal Shading
        [SerializeField]
        bool normalView;
        public bool NormalView 
        {
            get => normalView;
            set 
            {
                if (value == normalView)
                    return;
                Undo.RecordObject(this, "ModEditor NormalView");
                normalView = value;
            }
        }

        [SerializeField]
        bool normalViewUnfold = true;
        public bool NormalViewUnfold
        {
            get => normalViewUnfold;
            set
            {
                if (value == normalViewUnfold)
                    return;
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
                if (value == normalColor)
                    return;
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
                if (value == normalLength)
                    return;
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
                if (value == tangentView)
                    return;
                Undo.RecordObject(this, "ModEditor TangentView");
                tangentView = value;
            }
        }

        [SerializeField]
        bool tangentViewUnfold = true;
        public bool TangentViewUnfold
        {
            get => tangentViewUnfold;
            set
            {
                if (value == tangentViewUnfold)
                    return;
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
                if (value == tangentColor)
                    return;
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
                if (value == tangentLength)
                    return;
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
                if (value == arrowLength)
                    return;
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
                if (value == arrowSize)
                    return;
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
                if (value == gridView)
                    return;
                Undo.RecordObject(this, "ModEditor GridView");
                gridView = value;
            }
        }

        [SerializeField]
        bool gridViewUnfold = true;
        public bool GridViewUnfold
        {
            get => gridViewUnfold;
            set
            {
                if (value == gridViewUnfold)
                    return;
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
                if (value == gridColor)
                    return;
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
                if (value == uvView)
                    return;
                Undo.RecordObject(this, "ModEditor UVView");
                uvView = value;
            }
        }

        [SerializeField]
        bool uvViewUnfold = true;
        public bool UVViewUnfold
        {
            get => uvViewUnfold;
            set
            {
                if (value == uvViewUnfold)
                    return;
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
                if (value == uvAlpha)
                    return;
                Undo.RecordObject(this, "ModEditor UVAlpha");
                uvAlpha = value;
            }
        }
        #endregion

        private void Awake()
        {
            if (actionableDic == null)
                actionableDic = new UndoClass.Dictionary_Obj_Bool(this);
            if (meshDic == null)
                meshDic = new UndoClass.Dictionary_Obj_Mesh(this);
        }

        public void CheckAndClearExposed()
        {
            if (ModEditorWindow.ExposedManagement.CheckAndClearExposed(target.exposedName))
                target = default;
            ActionableDic.CheckAndClearExposed();
            MeshDic.CheckAndClearExposed();
            EditorUtility.SetDirty(this);
        }
    }
}