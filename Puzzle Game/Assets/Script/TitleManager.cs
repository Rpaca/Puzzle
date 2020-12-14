using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(1920, 1080, true);
    }


    public void gameStart()
    {
        SceneManager.LoadScene("Level1");
        return;
    }

    public void goTutorial()
    {
        GameObject.Find("UI").transform.GetChild(6).gameObject.SetActive(true);
        GameObject.Find("UI").transform.GetChild(5).gameObject.SetActive(true);
        return;
    }

    public void goBackTitle()
    {
        SceneManager.LoadScene("Title");
        return;
    }

    public void goNextLevel()
    {
        if (GameManager.instance.stageLevel == 1)
        {
            SceneManager.LoadScene("Level2");
        }
        else if (GameManager.instance.stageLevel == 2)
        {
            SceneManager.LoadScene("Level3");
        }
        return;
    }

    public void exitUI1()
    {
        GameObject.Find("UI").transform.GetChild(6).gameObject.SetActive(false);
    }


    public void exitUI2()
    {
        GameObject.Find("UI").transform.GetChild(5).gameObject.SetActive(false);
    }

    public void exit()
    {
        Application.Quit();
        return;
    }
}
