using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class ExposedManagement : MonoBehaviour, IExposedPropertyTable
    {
        public int growthId;
        public List<PropertyName> keys = new List<PropertyName>();
        public List<Object> vals = new List<Object>();

        public void ClearReferenceValue(PropertyName id)
        {
            int index = keys.IndexOf(id);
            if (index > -1)
            {
                keys.RemoveAt(index);
                vals.RemoveAt(index);
#if UNITY_EDITOR
                EditorUtility.SetDirty(gameObject);
#endif
            }
        }

        public Object GetReferenceValue(PropertyName id, out bool idValid)
        {
            int index = keys.IndexOf(id);
            if (index > -1)
            {
                idValid = true;
                return vals[index];
            }
            idValid = false;
            return null;
        }

        public void SetReferenceValue(PropertyName id, Object value)
        {
            if (PropertyName.IsNullOrEmpty(id))
                return;
            int index = keys.IndexOf(id);
            if (index > -1)
            {
                keys[index] = id;
                vals[index] = value;
            }
            else
            {
                keys.Add(id);
                vals.Add(value);
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
        }

        public PropertyName GetKey<T>(T obj) where T : Object
        {
            if (obj == null)
                return default;
            int index = vals.IndexOf(obj);
            if (index > -1)
                return keys[index];
            else
            {
#if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                    return 0;
#endif
                PropertyName property = new PropertyName(++growthId);
                SetReferenceValue(property, obj);
                return property;
            }
        }

        public ExposedReference<T> LinkExposedReference<T>(T obj) where T : Object
        {
            if (obj == null)
                return default;
            ExposedReference<T> exposedReference = new ExposedReference<T>();
            int index = vals.IndexOf(obj);
            if (index > -1)
                exposedReference.exposedName = keys[index];
            else
            {
#if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                    return exposedReference;
#endif
                PropertyName property = new PropertyName(++growthId);
                exposedReference.exposedName = property;
                SetReferenceValue(property, obj);
            }
            return exposedReference;
        }

        public bool CheckAndClearExposed(PropertyName key)
        {
            int index = keys.IndexOf(key);
            if (index > -1)
            {
                if (vals[index] == null)
                {
                    ClearReferenceValue(key);
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }

        public void CheckAndClearExposed()
        {
            for (int i = 0; i < vals.Count; i++)
            {
                if (vals[i] == null)
                {
                    keys.RemoveAt(i);
                    vals.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}