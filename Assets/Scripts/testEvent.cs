using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        e += TestEvent_e1;
        e += TestEvent_e;
        
    }

    private bool TestEvent_e()
    {
        return true;
    }

    private bool TestEvent_e1()
    {
        return false;
    }

    event Func<bool> e;

    private void OnGUI()
    {
        if (GUILayout.Button("asd"))
        {
            
        }
    }
}
