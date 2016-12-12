using UnityEngine;
using System.Collections;
using System;

public class ControlConfig : MonoBehaviour {
    string currentControl;
    InputType currentType;
    private bool receivingInput = false;

    enum InputType
    {
        Button, Axis
    }

    void Update()
    {
        if (receivingInput)
        {
            if (currentType == InputType.Button)
            {
                //Look for any button presses
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (kcode.ToString().Contains("Mouse") && Input.GetKeyDown(kcode))
                    {
                        Debug.Log(kcode);
                        receivingInput = false;
                    }
                }
            }
            else if (currentType == InputType.Axis)
            {
                //Look for any axis input
                for (int x = 0; x < 4; x++)
                {
                    for (int i = 0; i < 28; i++)
                    {
                        if (Mathf.Abs(Input.GetAxisRaw("joy_" + x + "_axis_" + i)) > 0.5f)
                        {
                            Debug.Log("joystick " + x + " axis " + i);
                            receivingInput = false;
                        }
                    }
                }
            }
        }
    }

    public void ChangeControl(string controlString)
    {
        Debug.Log("Waiting for input!");
        receivingInput = true;
    }
}
