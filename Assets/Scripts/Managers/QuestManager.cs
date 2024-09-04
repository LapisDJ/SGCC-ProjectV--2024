using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    // 각 맵에서의 플레이어 시작 위치를 저장하는 딕셔너리
    public Dictionary<int, Vector3> playerStartPositions = new Dictionary<int, Vector3>()
    {
        { 1, new Vector3(29.5f, -3.5f, 0) },  // Map 1 시작 위치
        { 2, new Vector3(1.5f, -2f, 0) },    // Map 2 시작 위치
        { 3, new Vector3(2f, 24f, 0) },      // Map 3 시작 위치
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);  // 중복된 오브젝트 파괴
        }
    }

    public int currentQuest = 1;

    // 씬 이동을 관리하는 함수
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
                SceneManager.LoadScene("Finish UI");  // 모든 퀘스트 완료 후 메인 메뉴로 복귀
                break;
        }
    }

    // 퀘스트에 따른 플레이어 시작 위치 반환
    public Vector3 GetPlayerStartPosition(int currentQuest)
    {
        if (playerStartPositions.ContainsKey(currentQuest))
        {
            return playerStartPositions[currentQuest];
        }

        return Vector3.zero;  // 기본값
    }

    public int GetCurrentQuest()
    {
        return currentQuest;
    }
}
