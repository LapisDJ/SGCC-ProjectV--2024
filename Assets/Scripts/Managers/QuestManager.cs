using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int currentQuest = 1;

    public void CompleteQuest()
    {
        currentQuest++;

        switch (currentQuest)
        {
            case 2:
                SceneManager.LoadScene("Map 2");
                break;
            case 3:
                SceneManager.LoadScene("Map 3");
                break;
            case 4:
                SceneManager.LoadScene("Finish UI");
                break;
            default:
                Debug.Log("All quests completed!");
                break;
        }
    }

    public int GetCurrentQuest()
    {
        return currentQuest;
    }
}
