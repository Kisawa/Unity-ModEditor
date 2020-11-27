using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testMono : MonoBehaviour, IExposedPropertyTable
{
    public test t;

    public List<PropertyName> prop = new List<PropertyName>();
    public List<Object> obj = new List<Object>();

    public void ClearReferenceValue(PropertyName id)
    {
        Debug.LogError("ClearReferenceValue");
        int index = -1;
        index = prop.IndexOf(id);
        if (index > -1)
        {
            prop.RemoveAt(index);
            obj.RemoveAt(index);
        }
    }

    public Object GetReferenceValue(PropertyName id, out bool idValid)
    {
        Object res;
        idValid = false;
        int index = -1;
        index = prop.IndexOf(id);
        if (index > -1)
        {
            idValid = true;
            return obj[index];
        }
        return null;
    }

    public void SetReferenceValue(PropertyName id, Object value)
    {
        Debug.LogError("SetReferenceValue");
        int index = -1;
        if (PropertyName.IsNullOrEmpty(id))
            return;
        index = prop.IndexOf(id);
        if (index > -1)
        {
            prop[index] = id;
            obj[index] = value;
        }
        else
        {
            prop.Add(id);
            obj.Add(value);
        }
    }

    private void Start()
    {
        if (t != null)
        {
            //Camera camera = t.sceneCamera.Resolve(this);
            //Debug.LogError(camera.name);
        }
    }
}