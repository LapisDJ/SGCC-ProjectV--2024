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
        SceneManager.LoadScene("Finish UI");
    }

    public int GetCurrentQuest()
    {
        return currentQuest;
    }
}