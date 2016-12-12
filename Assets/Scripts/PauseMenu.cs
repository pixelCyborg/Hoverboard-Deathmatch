using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    int selectIndex = 0;
    List<Button> pauseButtons = new List<Button>();
    Transform selection;
    public bool paused = false;

	// Use this for initialization
	void Start () {
        selection = transform.FindChild("Selection");
	    for(int i = 0; i < transform.childCount; i++)
        {
            Button button = transform.GetChild(i).GetComponent<Button>();
            if(button != null)
            {
                pauseButtons.Add(button);
            }
        }

        selection.localPosition = pauseButtons[0].transform.localPosition;
        paused = false;
        gameObject.SetActive(false);
	}

    public void Pause()
    {
        paused = true;
        gameObject.SetActive(true);
        selectIndex = 0;
        StartCoroutine(LoadPauseMenu());
    }

    IEnumerator LoadPauseMenu()
    {
        yield return new WaitForEndOfFrame();
        selection.localPosition = pauseButtons[selectIndex].transform.localPosition;
    }

    public void Resume()
    {
        StartCoroutine(Unpause());
    }

    IEnumerator Unpause()
    {
        for(int i = 0; i < 6; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        paused = false;
        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        transform.parent.parent.GetComponentInChildren<CombatController>().Die();
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
	
    public void Up()
    {
        selectIndex--;
        if (selectIndex < 0) selectIndex = pauseButtons.Count - 1;
        selection.localPosition = pauseButtons[selectIndex].transform.localPosition;
    }

    public void Down()
    {
        selectIndex++;
        if (selectIndex == pauseButtons.Count) selectIndex = 0;
        selection.localPosition = pauseButtons[selectIndex].transform.localPosition;
    }

    public void Select()
    {
        pauseButtons[selectIndex].onClick.Invoke();
    }
}
