using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    // �� �ʿ����� �÷��̾� ���� ��ġ�� �����ϴ� ��ųʸ�
    public Dictionary<int, Vector3> playerStartPositions = new Dictionary<int, Vector3>()
    {
        { 1, new Vector3(29.5f, -3.5f, 0) },  // Map 1 ���� ��ġ
        { 2, new Vector3(1.5f, -2f, 0) },    // Map 2 ���� ��ġ
        { 3, new Vector3(2f, 24f, 0) },      // Map 3 ���� ��ġ
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // �� ��ȯ �� �ı����� ����
        }
        else
        {
            Destroy(gameObject);  // �ߺ��� ������Ʈ �ı�
        }
    }

    public int currentQuest = 1;

    // �� �̵��� �����ϴ� �Լ�
    public void CompleteQuest()
    {
        currentQuest++;

        switch (currentQuest)
        {
            
            case 1:
                SceneManager.LoadScene("Map 1");
                break;
            
            case 2:
                SceneManager.LoadScene("Map 2");
                break;
            
            case 3:
                SceneManager.LoadScene("Map 3");
                break;
           
            default:
                Debug.Log("All quests completed!");
                SceneManager.LoadScene("Finish UI");  // ��� ����Ʈ �Ϸ� �� ���� �޴��� ����
                break;
        }
    }

    // ����Ʈ�� ���� �÷��̾� ���� ��ġ ��ȯ
    public Vector3 GetPlayerStartPosition(int currentQuest)
    {
        if (playerStartPositions.ContainsKey(currentQuest))
        {
            return playerStartPositions[currentQuest];
        }

        return Vector3.zero;  // �⺻��
    }

    public int GetCurrentQuest()
    {
        return currentQuest;
    }
}
