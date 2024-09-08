using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class MiddleBossActivator : PlayerController
{
    public GameObject middleBoss;   // �߰����� ������Ʈ                                    // tag ����ؼ� �ڵ����� �ҷ�����
    public UnityEngine.Transform player;    // �÷��̾� ��ġ                                // tag ����ؼ� �ڵ����� �ҷ�����
    private bool isRestore = false; // ��ȣŸ�� ���� ����
    private Vector3 finVector = new Vector3(1.5f, -2f, 0);  // ������ �̼� �̵� ��ġ
    private float hp_prev;
    private float hp_cur;                                                              // Player_Controller.cs���� public ( ���� �̱��� )
    private bool isBossAppear = false;  // �߰����� ���� ����
    private float interactionTime = 0f;                                                     // Player_Controller.cs���� public
    private Vector3 interactPlayerPosition;                                                 // Player_Controller.cs���� public
    private float requiredInteractionTime = 10.0f;                                          // Player_Controller.cs���� public
    private bool isBossDead = false;    // �߰������� óġ�Ǿ����� ����
    private bool bossAlive = false;
    private Vector3 bossPosition;
    private new void Start()
    {
        questPosition = new Vector3(1.5f, 35f, 0);
        player_T = GameObject.Find("Player").transform;
        middleBoss.SetActive(false);    // Map2 ���� �� �߰����� ( ��ŷ�� �ȵ���̵� ) ��Ȱ��ȭ 
        isInteractionStarted = false;   // ��ȣ�ۿ� �̼� �Ϸ� ����
        Debug.Log("����Ʈ2�� �����մϴ�!");
        Debug.Log("�̼� 1 : ��θ� ���󰡼� �߰������� óġ�ϼ���");
    }


    private void Update()
    {
        hp_cur = Player_Stat.instance.HPcurrent;
        bool isWithinXRange = player_T.position.x >= -0.5f && player_T.position.x <= 2.5f;  // �߰����� ��ȯ ���� X��ǥ ������ �ִ���
        bool isWithinYRange = player_T.position.y >= 34f && player_T.position.y <= 36f;     // �߰����� ��ȯ ���� y��ǥ ������ �ִ���

        if (isWithinXRange && isWithinYRange && !isBossAppear)  // �߰����� ��ȯ ������ ������ ������ ���� ��ȯ���� ���� ��� ���ο� �̼����� ������ ��ȯ�ؼ� óġ�ϴ� �̼��� ����
        {
            Debug.Log("�߰������� ��ȯ�Ǿ����ϴ�!!");
            middleBoss.SetActive(true); // �߰����� ��ȯ
            isBossAppear = true;    // �߰����� ��ȯ���� True�� ��ȯ : �߰������� 1ȸ�� ����
            bossAlive = true;
        }
        if (bossAlive)
        {
            bossPosition = middleBoss.transform.position;
            questPosition = bossPosition;
        }
        if (middleBoss != null && Input.GetKeyDown(KeyCode.O))  // �ӽ÷� ����Ʈ Ŭ���� ���� ���� ġƮŰ 42~45 Lines�� ���� ����
        {
            Destroy(middleBoss); // ���� �ı�
            bossAlive = false;
        }

        if (!isBossDead && middleBoss == null)
        {
            Debug.Log("�߰������� óġ�Ͽ����ϴ�!");
            questPosition = new Vector3(1.5f, 18f, 0);
            Debug.Log("��ȣŸ���� ������ �ּ���!");
            isBossDead = true;
            bossAlive = false;
        }


        if (middleBoss == null && Vector3.Distance(player_T.position, transform.position) <= 5f && !isRestore)    // �߰������� óġ�ǰ� ��ȣŸ���� ��ȣ�ۿ��� ������ ������ ��� ���� �̼� ����
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.F))   // FŰ�� ������ ��ȣ�ۿ� ����
            {
                interactionTime = 0.1f; // ��ȣ�ۿ� �����ϸ� ��ȣ�ۿ� �ð� �ʱ�ȭ
                Debug.Log("���峭 ��ȣ Ÿ���� �����մϴ�");
                interactPlayerPosition = player_T.position;   // ��ȣ�ۿ� ���۽� �÷��̾� ��ġ�� ���
                isInteractionStarted = true;
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
                questPosition = finVector;
                isInteractionStarted = false;
                Debug.Log("��� �������� ���ư�����");
            }
            else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // ���� �ߴ� ����
            {
                Debug.Log("��ȣ Ÿ�� ���� �ߴ�");
                isInteractionStarted = false;
                isInteractionStarted = false;
                interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
            else if (hp_cur < hp_prev)
            {
                // ��ȣ�ۿ��� �ߴܵǾ��� ��
                Debug.Log("��ȣ Ÿ�� ���� ����");
                isInteractionStarted = false;
                isInteractionStarted = false;
                interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
        }

        if (isRestore && (Vector3.Distance(finVector, player_T.position)) < 3f)
        {
            Debug.Log("����Ʈ2 Ŭ����!");
            QuestManager.instance.CompleteQuest();
        }


        hp_prev = hp_cur;
        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log("����Ʈ2 ġƮŰ Ŭ����!");
            QuestManager.instance.CompleteQuest();
        }
    }
}
