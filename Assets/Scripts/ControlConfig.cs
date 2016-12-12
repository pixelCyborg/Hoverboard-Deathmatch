using UnityEngine;
using System.Collections;
using System;
using TeamUtility.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ControlConfig : MonoBehaviour {
    public int playerNum;
    string currentControl;
    InputType currentType;
    public bool receivingInput = false;

    int selectIndex = 0;
    public List<Image> controlButtons;
    private bool axisActive = false;
    private bool initialized = false;

    private Color origColor = Color.clear;
    private Color highlightColor = Color.white;
    private GameObject receivingMessage;

    public AudioClip moveSound;
    public AudioClip selectSound;
    private AudioSource source;

    void SelectUp()
    {
        controlButtons[selectIndex].color = origColor;
        selectIndex++;
        if (selectIndex == controlButtons.Count) selectIndex = 0;
        controlButtons[selectIndex].color = highlightColor;
        PlayMove();
    }

    void SelectDown()
    {
        controlButtons[selectIndex].color = origColor;
        selectIndex--;
        if (selectIndex < 0) selectIndex = controlButtons.Count - 1;
        controlButtons[selectIndex].color = highlightColor;
        PlayMove();
    }

    void PlayMove()
    {
        source.clip = moveSound;
        source.Play();
    }

    void PlaySelect()
    {
        source.clip = selectSound;
        source.Play();
    }

    void OnEnable()
    {
        CheckNames();
        if (playerNum < MenuManager.colors.Count)
        {
            highlightColor = MenuManager.colors[playerNum];
        }

        if (!initialized)
        {
            source = GetComponent<AudioSource>();
            controlButtons = new List<Image>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Button button = transform.GetChild(i).GetComponent<Button>();
                if (button != null) controlButtons.Add(button.GetComponent<Image>());
                receivingMessage = transform.FindChild("Receiving").gameObject;
                receivingMessage.SetActive(false);
            }
            if (origColor == Color.clear)
            {
                origColor = controlButtons[0].color;
            }

            selectIndex = 0;
            if (highlightColor != Color.white)
            {
                initialized = true;
            }
        }

        controlButtons[selectIndex].color = highlightColor;

    }

    void OnDisable()
    {
        controlButtons[selectIndex].color = origColor;
    }

    void CheckNames()
    {
        foreach (Transform child in transform)
        {
            Text text = child.GetComponentInChildren<Text>();
            if (child.GetComponent<Button>() && text != null)
            {
                if (child.name.Contains("Horizontal") || child.name.Contains("Vertical"))
                {
                    text.text = child.name + ": Axis " + InputManager.GetAxisConfiguration("Unity-Imported", child.name + " PLAYER_" + (playerNum + 1)).axis;
                }
                else
                {
                    string buttonString = InputManager.GetAxisConfiguration("Unity-Imported", child.name + " PLAYER_" + (playerNum + 1)).positive.ToString();
                    if (buttonString.Length > 6)
                    {
                        int buttonLocation = buttonString.IndexOf("Button") + 6;
                        buttonString = buttonString.Substring(buttonLocation, buttonString.Length - buttonLocation);
                        text.text = child.name + ": Button " + buttonString;
                    }
                }
            }
        }
    }

    enum InputType
    {
        Button, Axis
    }

    void Update()
    {
        if (!receivingInput)
        {
            if (InputManager.GetButtonDown("Fire PLAYER_" + (playerNum + 1)))
            {
                EventSystem.current.SetSelectedGameObject(controlButtons[selectIndex].gameObject);
                controlButtons[selectIndex].GetComponent<Button>().onClick.Invoke();
            }
            else if(InputManager.GetButtonDown("Drop PLAYER_" + (playerNum + 1)))
            {
                FindObjectOfType<MenuManager>().GoToMain();
            }
            else
            {
                float vertValue = InputManager.GetAxis("Vertical PLAYER_" + (playerNum + 1));
                if (vertValue > 0.9f)
                {
                    if (!axisActive)
                    {
                        axisActive = true;
                        SelectDown();
                    }
                }
                else if (vertValue < -0.9f)
                {
                    if (!axisActive)
                    {
                        axisActive = true;
                        SelectUp();
                    }
                }
                else
                {
                    axisActive = false;
                }
            }
        }
    }

    public void ChangeControl()
    {
        if (!receivingInput)
        {
            PlaySelect();
            receivingInput = true;
            receivingMessage.SetActive(true);
            //Get info on the currently pressed button
            currentControl = EventSystem.current.currentSelectedGameObject.name + " PLAYER_" + (playerNum + 1);
            if (currentControl.Contains("Horizontal") || currentControl.Contains("Vertical"))
                currentType = InputType.Axis;
            else
                currentType = InputType.Button;

            //Implement the KeyScan
            if (currentType == InputType.Button)
            {
                //BUTTON KEYSCAN
                InputManager.StartJoystickButtonScan((key, arg) =>
                {
                    AxisConfiguration axisConfig = InputManager.GetAxisConfiguration("Unity-Imported", currentControl);
                    axisConfig.positive = (key == KeyCode.Backspace || key == KeyCode.Escape) ? KeyCode.None : key;
                //Debug.Log("Set " + axisConfig.name + " to " + axisConfig.positive);
                StartCoroutine(StopReceiving());
                    CheckNames();
                    return true;
                }, playerNum, 10.0f, null);
            }
            else if (currentType == InputType.Axis)
            {
                //AXIS KEYSCAN
                InputManager.StartJoystickAxisScan((axis, arg) =>
                {
                    AxisConfiguration axisConfig = InputManager.GetAxisConfiguration("Unity-Imported", currentControl);
                    axisConfig.SetAnalogAxis(playerNum, axis);
                //Debug.Log("Set " + axisConfig.name + " to " + axisConfig.axis);
                StartCoroutine(StopReceiving());
                    CheckNames();
                    return true;
                }, playerNum, 10.0f, null);
            }
        }
    }

    IEnumerator StopReceiving()
    {
        PlaySelect();
        receivingMessage.SetActive(false);
        for (int i = 0; i < 6; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        receivingInput = false;
    }
}
