using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButton : MonoBehaviour
{
    public void OnClickNewgame()
    {
        SceneManager.LoadScene("Character Choose");
    }
    public void OnClickBringup()
    {
        SceneManager.LoadScene("?");
    }
    public void OnClickSettings()
    {
        SceneManager.LoadScene("Settings");
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
}
