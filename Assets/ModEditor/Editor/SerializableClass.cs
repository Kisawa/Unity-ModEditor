using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class SerializableClass
    {
        [Serializable]
        public class Dictionary_Obj_Bool
        {
            public UnityEngine.Object parent;
            [SerializeField]
            List<PropertyName> keyList;
            [SerializeField]
            List<bool> valList;

            public int count => keyList.Count;

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
                        throw new Exception("SerializableClass.Dictionary_Obj_Bool: This key does not exist.");
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
                        throw new Exception("SerializableClass.Dictionary_Obj_Bool: This key does not exist.");
                }
            }

            public bool this[int index]
            {
                get
                {
                    if (index >= 0)
                        return valList[index];
                    else
                        throw new Exception("SerializableClass.Dictionary_Obj_Bool: This key does not exist.");
                }
                set
                {
                    if (index >= 0)
                    {
                        if (valList[index] == value)
                            return;
                        if (parent != null)
                            Undo.RecordObject(parent, "UndoDic set");
                        valList[index] = value;
                    }
                    else
                        throw new Exception("SerializableClass.Dictionary_Obj_Bool: This key does not exist.");
                }
            }

            public GameObject Resolve(int index)
            {
                if (index < count)
                {
                    GameObject obj = ModEditorWindow.ExposedManagement.GetReferenceValue(keyList[index], out bool idValid) as GameObject;
                    if (idValid)
                        return obj;
                }
                return null;
            }

            public void Add(GameObject key, bool val)
            {
                if (keyList.Contains(ModEditorWindow.ExposedManagement.GetKey(key)))
                    throw new Exception("SerializableClass.Dictionary_Obj_Bool: This key already exists.");
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
                for (int i = 0; i < count; i++)
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
            [SerializeField]
            List<Mesh> originList;
            List<Mesh> recycleBin;

            public int count => keyList.Count;

            public Dictionary_Obj_Mesh()
            {
                keyList = new List<PropertyName>();
                valList = new List<Mesh>();
                originList = new List<Mesh>();
                recycleBin = new List<Mesh>();
            }

            public Dictionary_Obj_Mesh(UnityEngine.Object parent)
            {
                this.parent = parent;
                keyList = new List<PropertyName>();
                valList = new List<Mesh>();
                originList = new List<Mesh>();
                recycleBin = new List<Mesh>();
            }

            public Mesh this[GameObject key, bool origin = false]
            {
                get
                {
                    int index = keyList.IndexOf(ModEditorWindow.ExposedManagement.GetKey(key));
                    if (index >= 0)
                    {
                        return origin ? originList[index] : valList[index];
                    }
                    else
                        throw new Exception("SerializableClass.Dictionary_Obj_Mesh: This key does not exist.");
                }
                set
                {
                    int index = keyList.IndexOf(ModEditorWindow.ExposedManagement.GetKey(key));
                    if (index >= 0)
                    {
                        if (origin)
                        {
                            if (originList[index] == value)
                                return;
                            originList[index] = value;
                        }
                        else
                        {
                            if (valList[index] == value)
                                return;
                            valList[index] = value;
                        }
                    }
                    else
                        throw new Exception("SerializableClass.Dictionary_Obj_Mesh: This key does not exist.");
                }
            }

            public void Add(GameObject key, Mesh val, Mesh origin)
            {
                int index = keyList.IndexOf(ModEditorWindow.ExposedManagement.GetKey(key));
                if (index >= 0)
                {
                    Mesh mesh = valList[index];
                    if (mesh != null && mesh.name.EndsWith("-Editing"))
                    {
                        recycleBin.Add(mesh);
                        AssetDatabase.RemoveObjectFromAsset(mesh);
                    }
                    if (val != null && parent != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(val)))
                        AssetDatabase.AddObjectToAsset(val, parent);
                    valList[index] = val;
                    originList[index] = origin;
                }
                else
                {
                    keyList.Add(ModEditorWindow.ExposedManagement.GetKey(key));
                    valList.Add(val);
                    originList.Add(origin);
                }
                if (parent != null)
                    EditorUtility.SetDirty(parent);
            }

            public void Add(GameObject key, Mesh val)
            {
                int index = keyList.IndexOf(ModEditorWindow.ExposedManagement.GetKey(key));
                if (index >= 0)
                {
                    Mesh mesh = valList[index];
                    if (mesh != null && mesh.name.EndsWith("-Editing"))
                    {
                        recycleBin.Add(mesh);
                        AssetDatabase.RemoveObjectFromAsset(mesh);
                    }
                    if (val != null && parent != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(val)))
                        AssetDatabase.AddObjectToAsset(val, parent);
                    valList[index] = val;
                }
                else
                {
                    throw new Exception("SerializableClass.Dictionary_Obj_Mesh: This key does not exist. Please set the origin first than go on.");
                }
                if (parent != null)
                    EditorUtility.SetDirty(parent);
            }

            public bool TryGetValue(GameObject key, out Mesh val)
            {
                val = null;
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

            public GameObject Resolve(int index)
            {
                if (index < count)
                {
                    GameObject obj = ModEditorWindow.ExposedManagement.GetReferenceValue(keyList[index], out bool idValid) as GameObject;
                    if (idValid)
                        return obj;
                }
                return null;
            }

            public void CheckAndClearExposed()
            {
                for (int i = 0; i < count; i++)
                {
                    if (ModEditorWindow.ExposedManagement.CheckAndClearExposed(keyList[i]))
                    {
                        keyList.RemoveAt(i);
                        Mesh mesh = valList[i];
                        if (mesh != null)
                            AssetDatabase.RemoveObjectFromAsset(mesh);
                        valList.RemoveAt(i);
                        originList.RemoveAt(i);
                        i--;
                    }
                }
                if (parent != null)
                    EditorUtility.SetDirty(parent);
            }
        }
    }
}