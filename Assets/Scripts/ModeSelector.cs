using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ModeSelector : MonoBehaviour {
    int currentMode = 0;
    public Text modeText;
    Goal.GameMode[] modes = new Goal.GameMode[] { Goal.GameMode.Deathmatch, Goal.GameMode.Oddball };
    string[] modeStrings = new string[] { "Deathmatch", "Oddball" };

    public static Goal.GameMode gameMode = Goal.GameMode.Deathmatch;

    // Update is called once per frame
    public void Next ()
    {
        currentMode++;
        if (currentMode > modes.Length - 1)
        {
            currentMode = 0;
        }
        SetMode();
	}

    public void Previous()
    {
        currentMode--;
        if (currentMode < 0)
        {
            currentMode = modes.Length - 1;
        }

        SetMode();
    }

    public void SetMode()
    {
        modeText.text = modeStrings[currentMode];
        gameMode = modes[currentMode];
    }
}
