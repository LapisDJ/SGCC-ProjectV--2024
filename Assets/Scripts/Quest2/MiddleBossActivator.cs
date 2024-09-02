using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


// ��ũ��Ʈ�� ������Ʈ�� �ڵ����� �ְԲ� �ڵ� �߰� : ����Ʈ 1 , ����Ʈ2 , ����Ʈ3 ���


public class MiddleBossActivator : PlayerController
{
    public GameObject middleBoss;   // �߰����� ������Ʈ                                    // tag ����ؼ� �ڵ����� �ҷ�����
    public UnityEngine.Transform player;    // �÷��̾� ��ġ                                // tag ����ؼ� �ڵ����� �ҷ�����
    private bool isRestore = false; // ��ȣŸ�� ���� ����
    private Vector3 finVector = new Vector3(1.5f, -2f, 0);  // ������ �̼� �̵� ��ġ
    private float hp_temp;                                                                  // Player_Controller.cs���� public ( ���� �̱��� )
    private float hp_cur;                                                                   // Player_Controller.cs���� public ( ���� �̱��� )
    private bool isBossAppear = false;  // �߰����� ���� ����
    private float interactionTime = 0f;                                                     // Player_Controller.cs���� public
    private Vector3 interactPlayerPosition;                                                 // Player_Controller.cs���� public
    private float requiredInteractionTime = 10.0f;                                          // Player_Controller.cs���� public
    private bool isBossDead = false;    // �߰������� óġ�Ǿ����� ����

    private void Start()
    {
        GameObject player = GameObject.Find("Player"); // Player ������Ʈ ã��
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.questPosition = new Vector3(1.5f, 35f, 0);
            }
        }
        middleBoss.SetActive(false);    // Map2 ���� �� �߰����� ( ��ŷ�� �ȵ���̵� ) ��Ȱ��ȭ 
        isInteractionStarted = false;   // ��ȣ�ۿ� �̼� �Ϸ� ����
        Debug.Log("����Ʈ2�� �����մϴ�!");
        Debug.Log("�̼� 1 : ��θ� ���󰡼� �߰������� óġ�ϼ���");
    }
    private void Update()
    {
        //hp_cur = playerstat.HPcurrent;
        bool isWithinXRange = player.position.x >= -0.5f && player.position.x <= 2.5f;  // �߰����� ��ȯ ���� X��ǥ ������ �ִ���
        bool isWithinYRange = player.position.y >= 34f && player.position.y <= 36f;     // �߰����� ��ȯ ���� y��ǥ ������ �ִ���

        if (isWithinXRange && isWithinYRange && !isBossAppear)  // �߰����� ��ȯ ������ ������ ������ ���� ��ȯ���� ���� ��� ���ο� �̼����� ������ ��ȯ�ؼ� óġ�ϴ� �̼��� ����
        {
            Debug.Log("�߰������� ��ȯ�Ǿ����ϴ�!!");
            middleBoss.SetActive(true); // �߰����� ��ȯ
            isBossAppear = true;    // �߰����� ��ȯ���� True�� ��ȯ : �߰������� 1ȸ�� ����
        }

        if (middleBoss != null && Input.GetKeyDown(KeyCode.O))  // �ӽ÷� ����Ʈ Ŭ���� ���� ���� ġƮŰ 42~45 Lines�� ���� ����
        {
            Destroy(middleBoss); // ���� �ı�
        }

        if(!isBossDead && middleBoss == null)
        {
            Debug.Log("�߰������� óġ�Ͽ����ϴ�!");
            if (player != null)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.questPosition = new Vector3(1.5f, 18f, 0);
                }
            }
            Debug.Log("��ȣŸ���� ������ �ּ���!");
            isBossDead = true;
        }


        if (middleBoss == null && Vector3.Distance(player.position, transform.position) <= 3f && !isRestore)    // �߰������� óġ�ǰ� ��ȣŸ���� ��ȣ�ۿ��� ������ ������ ��� ���� �̼� ����
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.F))   // FŰ�� ������ ��ȣ�ۿ� ����
            {
                //hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f; // ��ȣ�ۿ� �����ϸ� ��ȣ�ۿ� �ð� �ʱ�ȭ
                Debug.Log("���峭 ��ȣ Ÿ���� �����մϴ�");
                interactPlayerPosition = player.position;   // ��ȣ�ۿ� ���۽� �÷��̾� ��ġ�� ���
                isInteractionStarted = true;
            }

            if (interactionTime > 0f)
            {
                interactionTime += Time.deltaTime;
            }

            if (interactionTime >= requiredInteractionTime)
            {
                Debug.Log("��ȣ Ÿ�� ���� �Ϸ�");
                isRestore = true;
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
            else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // ���� �ߴ� ����
            {
                Debug.Log("��ȣ Ÿ�� ���� �ߴ�");
                isInteractionStarted = false;
                interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
            //else if ((interactPlayerPosition != player.position) && isInteractionStarted || ((hp_temp != hp_Cur) && isInteractionStarted))
            else if ((interactPlayerPosition != player.position) && isInteractionStarted)
            {
                // ��ȣ�ۿ��� �ߴܵǾ��� ��
                Debug.Log("��ȣ Ÿ�� ���� ����");
                isInteractionStarted = false;
                interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
        }

        if (isRestore && (Vector3.Distance(finVector, player.position)) < 3f)
        {
            Debug.Log("����Ʈ2 Ŭ����!");
            QuestManager.instance.CompleteQuest();
        }
    }
}