using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    // 각 맵에서의 플레이어 시작 위치를 저장하는 딕셔너리
    public Dictionary<int, Vector3> playerStartPositions = new Dictionary<int, Vector3>()
    {
        //{ 1, new Vector3(28.5f, -2.5f, 0) },  // Map 1 시작 위치
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
    /*
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Map1 씬이 로드될 때 QuestManager 및 오브젝트 초기화
        if (scene.name == "Map 1")
        {
            ResetMap1();
        }
    }

    // Map1 초기화 작업 수행
    private void ResetMap1()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            // 플레이어가 없으면 새로 생성
            GameObject newPlayer = Instantiate(Resources.Load("Player")) as GameObject;
            newPlayer.transform.position = GetPlayerStartPosition(1);
        }
    }
    */
    // 씬 이동을 관리하는 함수
    public void CompleteQuest()
    {
        currentQuest++;
        //SpawnManager spawnManager = SpawnManager.instance;
        if (currentQuest > 3)
        {
            //SpawnManager.instance.DestroyAllPools();  // 모든 오브젝트 풀 삭제
            SceneManager.LoadScene("Finish UI");
            Destroy(GameObject.FindWithTag("Player"));  // Player 오브젝트 파괴
            Destroy(GameObject.FindWithTag("SpawnManager"));
            Destroy(GameObject.FindWithTag("SkillManager"));
            Destroy(GameObject.FindWithTag("UIManager"));
            Destroy(GameObject.FindWithTag("RealTimeManager"));
            Destroy(GameObject.FindWithTag("MainCamera"));
            SpawnManager.instance.DestroyAllPools();
        }
        else
        {
            //SpawnManager.instance.DeactivateActivePools();  // 활성화된 풀 비활성화
            SceneManager.LoadScene("Map " + currentQuest);
            SpawnManager.instance.DestroyActiveObjects();
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
