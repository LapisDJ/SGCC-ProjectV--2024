using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SurvAndBoss : PlayerController
{
    //private UnityEngine.Transform player;    // Player ��ġ ����
    public GameObject boss; // ��
    public GameObject targetObject;

    private static int count = 0;   // ����� ������ �� ( 3���� ���� ������ ���� �Ϸᰡ true�� bull ���·ε� Ȱ�� )
    private Vector3 finVector = new Vector3(2f, 24f, 0);    // ����Ʈ3 ������ �̼ǿ��� �����ؾ� �� ��ġ
    private bool isBossAppear = false;
    private float hp_temp;
    private float hp_cur;
    private float interactionTime = 0f;
    private Vector3 interactPlayerPosition;
    private float requiredInteractionTime = 10.0f;
    private bool isBossDead = false;    // �������� óġ����
    private Vector3 interactPosition;   // ��ȣ�ۿ��� questPosition�� �ĺ���
    private void Start()
    {
        interactPosition = targetObject.transform.position;
        interactPosition = new Vector3(interactPosition.x + 3f, interactPosition.y , interactPosition.z);

        boss.transform.position = finVector;    // ���� ���� ���� ��ġ ����
        boss.SetActive(false);  // ���� ���� �� ���� ��Ȱ��ȭ
        isInteractionStarted = false;   // ������ ���⿡ ����ϴ� ���� ( ��� : ... )

        GameObject player = GameObject.Find("Player"); // Player ������Ʈ ã��
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.questPosition = new Vector3 (27f , 14f , 0);
            }
        }

    }

    private void Update()
    {
        if (count < 3)  // �����ڰ� ��� ������� ���� ���
        {
            if (questPosition != interactPosition)
            {
                if (Vector3.Distance(player_T.transform.position, interactPosition) < Vector3.Distance(player_T.transform.position, questPosition))
                {
                    GameObject player = GameObject.Find("Player"); // Player ������Ʈ ã��
                    if (player != null)
                    {
                        PlayerController playerController = player.GetComponent<PlayerController>();
                        if (playerController != null)
                        {
                            playerController.questPosition = interactPosition;
                        }
                    }
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
            }

            // �������� óġ ġƮŰ
            if (boss != null && Input.GetKeyDown(KeyCode.O))
            {
                Destroy(boss); // ���� �ı�
            }

            if (!isBossDead && boss == null)    // ���� ���͸� ó���� ���
            {
                Debug.Log("�߰������� óġ�Ͽ����ϴ�!");
                GameObject player = GameObject.Find("Player"); // Player ������Ʈ ã��
                if (player != null)
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.questPosition = finVector;
                    }
                }
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

    private void CheckSurvivorInteraction()
    {
        if (Vector3.Distance(targetObject.transform.position, player_T.position) <= 3f)
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.F))
            {
                //hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f;
                Debug.Log("������ ������ �����մϴ�");
                interactPlayerPosition = player_T.position;
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
                isInteractionStarted = false;
                if (count == 3)
                {
                    GameObject player = GameObject.Find("Player"); // Player ������Ʈ ã��
                    if (player != null)
                    {
                        PlayerController playerController = player.GetComponent<PlayerController>();
                        if (playerController != null)
                        {
                            playerController.questPosition = finVector;
                        }
                    }
                    Debug.Log("��� �������� ���ư�����");
                }
            }
            else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // ���� �ߴ� ����
            {
                Debug.Log("������ ���� �ߴ�");
                isInteractionStarted = false;
                interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
            //else if ((interactPlayerPosition != player.position) && isInteractionStarted || ((hp_temp != hp_Cur) && isInteractionStarted))
            else if ((interactPlayerPosition != player_T.position) && isInteractionStarted)
            {
                // ��ȣ�ۿ��� �ߴܵǾ��� ��
                Debug.Log("������ ���� ����");
                isInteractionStarted = false;
                interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
        }
    }
}