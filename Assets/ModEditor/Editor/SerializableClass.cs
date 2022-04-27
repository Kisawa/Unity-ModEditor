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
            [SerializeField]
            List<PropertyName> keyList = new List<PropertyName>();
            [SerializeField]
            List<Mesh> originList = new List<Mesh>();
            Dictionary<PropertyName, Mesh> valDic = new Dictionary<PropertyName, Mesh>();

            public int count => keyList.Count;

            public Mesh this[GameObject key, bool origin = false]
            {
                get
                {
                    PropertyName _key = ModEditorWindow.ExposedManagement.GetKey(key);
                    int index = keyList.IndexOf(_key);
                    if (index >= 0)
                        return origin ? originList[index] : valDic[_key];
                    else
                        throw new Exception("SerializableClass.Dictionary_Obj_Mesh: This key does not exist.");
                }
                set
                {
                    PropertyName _key = ModEditorWindow.ExposedManagement.GetKey(key);
                    int index = keyList.IndexOf(_key);
                    if (index >= 0)
                    {
                        if (origin)
                            originList[index] = value;
                        else
                            valDic[_key] = value;
                    }
                    else
                        throw new Exception("SerializableClass.Dictionary_Obj_Mesh: This key does not exist.");
                }
            }

            public void Add(GameObject key, Mesh val, Mesh origin)
            {
                if(string.IsNullOrEmpty(AssetDatabase.GetAssetPath(origin)))
                    throw new Exception("SerializableClass.Dictionary_Obj_Mesh: origin is a instance.");
                PropertyName _key = ModEditorWindow.ExposedManagement.GetKey(key);
                int index = keyList.IndexOf(_key);
                if (index >= 0)
                    originList[index] = origin;
                else
                {
                    keyList.Add(ModEditorWindow.ExposedManagement.GetKey(key));
                    originList.Add(origin);
                }
                valDic[_key] = val;
            }

            public void Add(GameObject key, Mesh val)
            {
                PropertyName _key = ModEditorWindow.ExposedManagement.GetKey(key);
                int index = keyList.IndexOf(_key);
                if (index >= 0)
                {
                    if (val == originList[index])
                        valDic.Remove(_key);
                    else
                        valDic[_key] = val;
                }
                else
                    throw new Exception("SerializableClass.Dictionary_Obj_Mesh: This key does not exist. Please set the origin first than go on.");
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
                checkFolder();
                for (int i = 0; i < keyList.Count; i++)
                {
                    PropertyName _key = keyList[i];
                    if (ModEditorWindow.ExposedManagement.CheckAndClearExposed(_key))
                    {
                        keyList.RemoveAt(i);
                        valDic.Remove(_key);
                        originList.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        if (valDic.TryGetValue(_key, out Mesh mesh) && mesh != null)
                        {
                            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mesh)))
                            {
                                string path = $"{ModEditorWindow.ModEditorPath}/Meshs/{mesh.name}.mesh";
                                path = path.Replace(':', '-');
                                int checkIndex = 1;
                                while (true)
                                {
                                    Mesh _m = AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;
                                    if (_m == null)
                                        break;
                                    if (_m.vertexCount == mesh.vertexCount)
                                    {
                                        AssetDatabase.DeleteAsset(path);
                                        break;
                                    }
                                    path = $"{ModEditorWindow.ModEditorPath}/Meshs/{mesh.name} {checkIndex++}.mesh";
                                    path = path.Replace(':', '-');
                                }
                                AssetDatabase.CreateAsset(mesh, path);
                            }
                            valDic.Remove(_key);
                            originList[i] = mesh;
                        }
                    }
                }
                AssetDatabase.ImportAsset($"{ModEditorWindow.ModEditorPath}/Meshs");
            }

            void checkFolder()
            {
                if (!AssetDatabase.IsValidFolder($"{ModEditorWindow.ModEditorPath}/Meshs"))
                    AssetDatabase.CreateFolder(ModEditorWindow.ModEditorPath, "Meshs");
            }
        }
    }
}