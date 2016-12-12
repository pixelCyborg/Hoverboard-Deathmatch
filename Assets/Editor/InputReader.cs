using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ControlManager))]
public class ControlManagerEditor : Editor {
    private ControlManager manager;

    void OnEnable()
    {
        manager = (ControlManager)target;
    }

    public static void ReadAxes()
    {
        var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];

        SerializedObject obj = new SerializedObject(inputManager);

        SerializedProperty axisArray = obj.FindProperty("m_Axes");

        if (axisArray.arraySize == 0)
            Debug.Log("No Axes");

        for (int i = 0; i < axisArray.arraySize; ++i)
        {
            var axis = axisArray.GetArrayElementAtIndex(i);

            var name = axis.FindPropertyRelative("m_Name").stringValue;
            var axisVal = axis.FindPropertyRelative("axis").intValue;
            var inputType = (InputType)axis.FindPropertyRelative("type").intValue;

            Debug.Log(name);
            Debug.Log(axisVal);
            Debug.Log(inputType);
        }
    }

    public enum InputType
    {
        KeyOrMouseButton,
        MouseMovement,
        JoystickAxis,
    };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Read Axes"))
        {
            ReadAxes();
        }
    }

}
