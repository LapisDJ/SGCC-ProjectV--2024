using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SurvAndBoss : PlayerController
{
    public GameObject boss; // ��
    public GameObject targetObject;
    private static int count = 0;   // ����� ������ �� ( 3���� ���� ������ ���� �Ϸᰡ true�� bull ���·ε� Ȱ�� )
    private Vector3 finVector = new Vector3(2f, 24f, 0);    // ����Ʈ3 ������ �̼ǿ��� �����ؾ� �� ��ġ
    private bool isBossAppear = false;
    private float hp_prev;
    private float hp_cur;
    private float interactionTime = 0f;
    private Vector3 interactPlayerPosition;
    private float requiredInteractionTime = 10.0f;
    private bool isBossDead = false;    // �������� óġ����
    private Vector3 interactPosition;   // ��ȣ�ۿ��� questPosition�� �ĺ���
    private Vector3 bossPosition;
    private bool bossAlive = false;
    private float interactReach = 3.0f;
    private new void Start()
    {
        GameObject surv1 = GameObject.Find("Surv1");
        if (surv1 != null)
        {
            surv1.transform.position = new Vector3(-4f, 49f, 0f);
        }

        GameObject surv2 = GameObject.Find("Surv2");
        if (surv2 != null)
        {
            surv2.transform.position = new Vector3(-15f, 12f, 0f);
        }

        GameObject surv3 = GameObject.Find("Surv3");
        if (surv3 != null)
        {
            surv3.transform.position = new Vector3(25f, 14f, 0f);
        }


        targetObject = this.gameObject;
        player_T = GameObject.Find("Player").transform;
        /*
         * backgroundTilemap = GameObject.Find("Background").GetComponent<Tilemap>();
        obstacleTilemaps = new Tilemap[6]
            {
                    GameObject.Find("Left").GetComponent<Tilemap>(),
                    GameObject.Find("Right").GetComponent<Tilemap>(),
                    GameObject.Find("Top").GetComponent<Tilemap>(),
                    GameObject.Find("Bottom").GetComponent<Tilemap>(),
                    GameObject.Find("Wreck").GetComponent<Tilemap>(),
                    GameObject.Find("Building").GetComponent<Tilemap>()
            };
        */
        interactPosition = targetObject.transform.position;
        interactPosition = new Vector3(interactPosition.x + 3f, interactPosition.y, interactPosition.z);
        Debug.Log(interactPosition);
        boss.transform.position = finVector;    // ���� ���� ���� ��ġ ����
        boss.SetActive(false);  // ���� ���� �� ���� ��Ȱ��ȭ
        isInteractionStarted = false;   // ������ ���⿡ ����ϴ� ���� ( ��� : ... )

        GameObject player = GameObject.Find("Player"); // Player ������Ʈ ã��
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.questPosition = new Vector3(27f, 14f, 0);
            }
        }
        questPosition = new Vector3(27f, 14f, 0);
    }

    private void Update()
    {
        GameObject player = GameObject.Find("Player"); // Player ������Ʈ ã��
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                hp_cur = Player_Stat.instance.HPcurrent;

                if (count < 3)  // �����ڰ� ��� ������� ���� ���
                {
                    if (questPosition != interactPosition)
                    {
                        if (Vector3.Distance(player_T.position, interactPosition) < Vector3.Distance(player_T.position, questPosition))
                        {
                            playerController.questPosition = interactPosition;
                            questPosition = interactPosition;
                            Debug.Log(playerController.questPosition);
                            Debug.Log(questPosition);
                        }
                    }
                    CheckSurvivorInteraction(); // ������ �����ϴ� �̼��� ������
                }
                else            // ������ ��� ������ ��� ������ �̼��� ������
                {
                    if (!isBossAppear && count == 3 && Vector3.Distance(player_T.position, finVector) <= 10f) // �÷��̾ ���� ��ġ ��ó�� �� ��� �������Ͱ� ��ȯ��
                    {
                        Debug.Log("�������� ����!");
                        boss.SetActive(true);   // �������� ��ȯ
                        if (boss.activeSelf)    // ������ ��ȯ�Ǹ�
                        {
                            isBossAppear = true;    // ������ 1ȸ�� ��ȯ�ǵ��� ��
                        }
                        bossAlive = true;
                    }

                    if (bossAlive)
                    {
                        bossPosition = boss.transform.position;
                        Debug.Log(bossPosition);
                        playerController.questPosition = bossPosition;
                    }
                    // �������� óġ ġƮŰ
                    if (boss != null && Input.GetKeyDown(KeyCode.O))
                    {
                        Destroy(boss); // ���� �ı�
                        bossAlive = false;
                    }

                    if (!isBossDead && boss == null)    // ���� ���͸� ó���� ���
                    {
                        Debug.Log("�߰������� óġ�Ͽ����ϴ�!");
                        playerController.questPosition = finVector;
                        Debug.Log("��� �������� ���ư�����");
                        isBossDead = true;
                    }

                    if (boss == null && Vector3.Distance(player_T.position, finVector) <= 3f) // ���� ���͸� óġ�� �� ���� ��ġ�� �����ϸ� ����Ʈ3 ������ �� ������ ����
                    {
                        Debug.Log("����Ʈ3 Ŭ����!");
                        QuestManager.instance.CompleteQuest();  // ������ ������ �̵�
                    }
                }

            }
        }
        hp_prev = hp_cur;
    }

    private void CheckSurvivorInteraction() // ������ ���� �Լ�
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                if (Vector3.Distance(targetObject.transform.position, player_T.position) <= interactReach)
                {
                    if (!isInteractionStarted && Input.GetKey(KeyCode.F))
                    {
                        interactionTime = 0.1f;
                        Debug.Log("������ ������ �����մϴ�");
                        playerController.isInteractionStarted = true;
                        isInteractionStarted = true;
                    }
                    if (interactionTime > 0f)
                    {
                        interactionTime += Time.deltaTime;
                    }
                    if (interactionTime >= requiredInteractionTime)
                    {
                        count++;
                        Debug.Log("������ " + count + "�� ���� �Ϸ�");

                        Destroy(gameObject);
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        if (count == 3)
                        {
                            playerController.questPosition = finVector;
                            Debug.Log("��� �������� ���ư�����");
                        }
                    }
                    else if (isInteractionStarted && Input.GetKey(KeyCode.G))
                    {
                        Debug.Log("������ ���� �ߴ�");
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        interactionTime = 0f;
                    }
                    else if (hp_cur < hp_prev)
                    {
                        Debug.Log("������ ���� ����");
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        interactionTime = 0f; 
                    }
                }
            }
        }
    }
}

