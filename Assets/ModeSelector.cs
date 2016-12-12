using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ModeSelector : MonoBehaviour {
    int currentMode = 0;
    public Text modeText;
    Goal.GameMode[] modes = new Goal.GameMode[] { Goal.GameMode.Deathmatch, Goal.GameMode.Oddball };
    string[] modeStrings = new string[] { "Deathmatch", "Oddball" };

    public static Goal.GameMode gameMode = Goal.GameMode.Deathmatch;

    void Update()
    {
        for(int i = 1; i < 5; i++)
        {
            if (Input.GetButtonDown("Drift PLAYER_" + i))
            {
                Next();
            }
            else if(Input.GetButtonDown("Fire PLAYER_" + i))
            {
                Previous();
            }
        }
    }

	// Update is called once per frame
	void Next ()
    {
        currentMode++;
        if (currentMode >= modes.Length - 1)
        {
            currentMode = 0;
        }

        SetMode();
	}

    void Previous()
    {
        currentMode--;
        if (currentMode < 0)
        {
            currentMode = modes.Length - 1;
        }

        SetMode();
    }

    void SetMode()
    {
        modeText.text = modeStrings[currentMode];
        gameMode = modes[currentMode];
    }
}
