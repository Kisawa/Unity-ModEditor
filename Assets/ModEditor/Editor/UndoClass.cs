using System;
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
            List<PropertyName> keyList;
            [SerializeField]
            List<bool> valList;

            public Dictionary_Obj_Bool()
            {
                keyList = new List<PropertyName>();
                valList = new List<bool>();
            }

            public Dictionary_Obj_Bool(UnityEngine.Object parent)
            {
                this.parent = parent;
                keyList = new List<PropertyName>();
                valList = new List<bool>();
            }

            public bool this[GameObject key]
            {
                get
                {
                    int index = keyList.IndexOf(ModEditorWindow.ExposedManagement.GetKey(key));
                    if (index >= 0)
                        return valList[index];
                    else
                        throw new Exception("UndoClass.Dictionary: This key does not exist.");
                }
                set
                {
                    int index = keyList.IndexOf(ModEditorWindow.ExposedManagement.GetKey(key));
                    if (index >= 0)
                    {
                        if (valList[index] == value)
                            return;
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
                if (keyList.Contains(ModEditorWindow.ExposedManagement.GetKey(key)))
                    throw new Exception("UndoClass.Dictionary: This key already exists.");
                keyList.Add(ModEditorWindow.ExposedManagement.GetKey(key));
                valList.Add(val);
                if (parent != null)
                    EditorUtility.SetDirty(parent);
            }

            public bool TryGetValue(GameObject key, out bool val)
            {
                val = default;
                int index = keyList.IndexOf(ModEditorWindow.ExposedManagement.GetKey(key));
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
                return keyList.Contains(ModEditorWindow.ExposedManagement.GetKey(key));
            }

            public void CheckAndClearExposed()
            {
                for (int i = 0; i < keyList.Count; i++)
                {
                    if (ModEditorWindow.ExposedManagement.CheckAndClearExposed(keyList[i]))
                    {
                        keyList.RemoveAt(i);
                        valList.RemoveAt(i);
                        i--;
                    }
                }
                if (parent != null)
                    EditorUtility.SetDirty(parent);
            }
        }

        [Serializable]
        public class Dictionary_Obj_Mesh
        {
            public UnityEngine.Object parent;
            [SerializeField]
            List<PropertyName> keyList;
            [SerializeField]
            List<Mesh> valList;

            public Dictionary_Obj_Mesh()
            {
                keyList = new List<PropertyName>();
                valList = new List<Mesh>();
            }

            public Dictionary_Obj_Mesh(UnityEngine.Object parent)
            {
                this.parent = parent;
                keyList = new List<PropertyName>();
                valList = new List<Mesh>();
            }

            public Mesh this[GameObject key]
            {
                get
                {
                    int index = keyList.IndexOf(ModEditorWindow.ExposedManagement.GetKey(key));
                    if (index >= 0)
                        return valList[index];
                    else
                        throw new Exception("UndoClass.Dictionary: This key does not exist.");
                }
                set
                {
                    int index = keyList.IndexOf(ModEditorWindow.ExposedManagement.GetKey(key));
                    if (index >= 0)
                    {
                        if (valList[index] == value)
                            return;
                        if (parent != null)
                            Undo.RecordObject(parent, "UndoDic set");
                        valList[index] = value;
                    }
                    else
                        throw new Exception("UndoClass.Dictionary: This key does not exist.");
                }
            }

            public void Add(GameObject key, Mesh val)
            {
                if (keyList.Contains(ModEditorWindow.ExposedManagement.GetKey(key)))
                    throw new Exception("UndoClass.Dictionary: This key already exists.");
                keyList.Add(ModEditorWindow.ExposedManagement.GetKey(key));
                valList.Add(val);
                if(parent != null)
                    EditorUtility.SetDirty(parent);
            }

            public bool TryGetValue(GameObject key, out bool val)
            {
                val = default;
                int index = keyList.IndexOf(ModEditorWindow.ExposedManagement.GetKey(key));
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
                return keyList.Contains(ModEditorWindow.ExposedManagement.GetKey(key));
            }

            public void CheckAndClearExposed()
            {
                for (int i = 0; i < keyList.Count; i++)
                {
                    if (ModEditorWindow.ExposedManagement.CheckAndClearExposed(keyList[i]))
                    {
                        keyList.RemoveAt(i);
                        Mesh mesh = valList[i];
                        AssetDatabase.RemoveObjectFromAsset(mesh);
                        valList.RemoveAt(i);
                        i--;
                    }
                }
                if (parent != null)
                    EditorUtility.SetDirty(parent);
            }
        }
    }
}