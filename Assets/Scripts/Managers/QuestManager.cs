using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    // �� �ʿ����� �÷��̾� ���� ��ġ�� �����ϴ� ��ųʸ�
    public Dictionary<int, Vector3> playerStartPositions = new Dictionary<int, Vector3>()
    {
        //{ 1, new Vector3(28.5f, -2.5f, 0) },  // Map 1 ���� ��ġ
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
    /*
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Map1 ���� �ε�� �� QuestManager �� ������Ʈ �ʱ�ȭ
        if (scene.name == "Map 1")
        {
            ResetMap1();
        }
    }

    // Map1 �ʱ�ȭ �۾� ����
    private void ResetMap1()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            // �÷��̾ ������ ���� ����
            GameObject newPlayer = Instantiate(Resources.Load("Player")) as GameObject;
            newPlayer.transform.position = GetPlayerStartPosition(1);
        }
    }
    */
    // �� �̵��� �����ϴ� �Լ�
    public void CompleteQuest()
    {
        currentQuest++;
        //SpawnManager spawnManager = SpawnManager.instance;
        if (currentQuest > 3)
        {
            //SpawnManager.instance.DestroyAllPools();  // ��� ������Ʈ Ǯ ����
            SceneManager.LoadScene("Finish UI");
            Destroy(GameObject.FindWithTag("Player"));  // Player ������Ʈ �ı�
            Destroy(GameObject.FindWithTag("SpawnManager"));
            Destroy(GameObject.FindWithTag("SkillManager"));
            Destroy(GameObject.FindWithTag("UIManager"));
            Destroy(GameObject.FindWithTag("RealTimeManager"));
            Destroy(GameObject.FindWithTag("MainCamera"));
            SpawnManager.instance.DestroyAllPools();
        }
        else
        {
            //SpawnManager.instance.DeactivateActivePools();  // Ȱ��ȭ�� Ǯ ��Ȱ��ȭ
            SceneManager.LoadScene("Map " + currentQuest);
            SpawnManager.instance.DestroyActiveObjects();
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
