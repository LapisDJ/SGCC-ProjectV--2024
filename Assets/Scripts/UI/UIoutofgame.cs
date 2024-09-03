using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIoutofgame : MonoBehaviour
{
    [SerializeField] GameObject recheckwindow;
    void Start()
    {
        recheckwindow.SetActive(false);
    }
    public void GotoMainmenuButton()
    {
        recheckwindow.SetActive(true);
    }
    public void Recheckyes()
    {
        LoadingSceneController.Loadscene("Main Menu");
    }
    public void Recheckno()
    {
        recheckwindow.SetActive(false);
    }
    public void Retry()
    {
        QuestManager.instance.currentQuest--;
        switch(QuestManager.instance.currentQuest)
        {
            case 1 : 
                LoadingSceneController.Loadscene("Map 1");
                break;
            case 2 : 
                LoadingSceneController.Loadscene("Map 2");
                break;
            case 3 : 
                LoadingSceneController.Loadscene("Map 3");
                break;
            default:
                break;
        }
    }
    public void Nextchapter()
    {
        switch(QuestManager.instance.currentQuest)
        {
            case 1 : 
                LoadingSceneController.Loadscene("Map 1");
                break;
            case 2 : 
                LoadingSceneController.Loadscene("Map 2");
                break;
            case 3 : 
                LoadingSceneController.Loadscene("Map 3");
                break;
            default:
                break;
        }
    }
    //캐릭터 선택창
    int characternum;
    public void FirstCharacterButton()
    {
        characternum = 1;
    }
    public void Confirmbutton()
    {
        LoadingSceneController.Loadscene("Map 1");
    }
    public void Previousbutton()
    {
        SceneManager.LoadScene("Main Menu");
    }

    //메인메뉴
    public void OnClickNewgame()
    {
        SceneManager.LoadScene("Character choose");
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
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
