using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ControlManager : MonoBehaviour {
    public Control[] controls;
    
    void Update()
    {
        if (Input.anyKey && Input.inputString != "")
        {
            Debug.Log(Input.inputString);
        }

        //if (Input.ax)
    }
}

public class Control
{
    public string controlString;
    public InputType type;

    public enum InputType
    {
        KeyOrMouseButton,
        MouseMovement,
        JoystickAxis,
    };
}
