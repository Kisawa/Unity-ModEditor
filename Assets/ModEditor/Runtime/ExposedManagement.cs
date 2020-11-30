using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ExposedManagement : MonoBehaviour, IExposedPropertyTable
{
    public List<PropertyName> keys = new List<PropertyName>();
    public List<Object> vals = new List<Object>();

    public void ClearReferenceValue(PropertyName id)
    {
        int index = keys.IndexOf(id);
        if (index > -1)
        {
            keys.RemoveAt(index);
            vals.RemoveAt(index);
            EditorUtility.SetDirty(gameObject);
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
        EditorUtility.SetDirty(gameObject);
    }

    public PropertyName GetKey<T>(T obj) where T: Object 
    {
        if (obj == null)
            return default;
        int index = vals.IndexOf(obj);
        if (index > -1)
            return keys[index];
        else
        {
            PropertyName property = new PropertyName(keys.Count + 1);
            SetReferenceValue(property, obj);
            return property;
        }
    }

    public ExposedReference<T> LinkExposedReference<T>(T obj) where T: Object
    {
        if (obj == null)
            return default;
        ExposedReference<T> exposedReference = new ExposedReference<T>();
        int index = vals.IndexOf(obj);
        if (index > -1)
            exposedReference.exposedName = keys[index];
        else
        {
            PropertyName property = new PropertyName(keys.Count + 1);
            exposedReference.exposedName = property;
            SetReferenceValue(property, obj);
        }
        return exposedReference;
    }
}