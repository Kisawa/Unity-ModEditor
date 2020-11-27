﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class UndoClass
    {
        [Serializable]
        public class Dictionary_Obj_Bool
        {
            public UnityEngine.Object parent;
            [SerializeField]
            List<GameObject> keyList;
            [SerializeField]
            List<bool> valList;

            public Dictionary_Obj_Bool()
            {
                keyList = new List<GameObject>();
                valList = new List<bool>();
            }

            public Dictionary_Obj_Bool(UnityEngine.Object parent)
            {
                this.parent = parent;
                keyList = new List<GameObject>();
                valList = new List<bool>();
            }

            public bool this[GameObject key]
            {
                get 
                {
                    int index = keyList.IndexOf(key);
                    if (index >= 0)
                        return valList[index];
                    else
                        throw new Exception("UndoClass.Dictionary: This key does not exist.");
                }
                set
                {
                    int index = keyList.IndexOf(key);
                    if (index >= 0)
                    {
                        if(parent != null)
                            Undo.RecordObject(parent, "UndoDic set");
                        valList[index] = value;
                    }
                    else
                        throw new Exception("UndoClass.Dictionary: This key does not exist.");
                }
            }

            public void Add(GameObject key, bool val)
            {
                if (keyList.Contains(key))
                    throw new Exception("UndoClass.Dictionary: This key already exists.");
                if (parent != null)
                    Undo.RecordObject(parent, "UndoDic Add");
                keyList.Add(key);
                valList.Add(val);
            }

            public bool TryGetValue(GameObject key, out bool val)
            {
                val = default;
                int index = keyList.IndexOf(key);
                if (index >= 0)
                {
                    val = valList[index];
                    return true;
                }
                else
                    return false;
            }

            public bool ContainsKey(GameObject key)
            {
                return keyList.Contains(key);
            }
        }
    }
}