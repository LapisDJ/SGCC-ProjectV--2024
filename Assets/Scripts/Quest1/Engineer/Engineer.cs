using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UI; // UI ���� ���̺귯�� �߰�

public class Engineer : PlayerController
{
    public float Engineer_HP = 100f; // �����Ͼ� ���� ü�� ����
    public float maxHP = 100f;      // �ִ� ü��
    public Slider hpSlider;         // HP �ٸ� ������ �����̴�
    public Vector3 hpBarOffset = new Vector3(0, -27f, 0); // �����Ͼ� �Ӹ� ���� ǥ�õ� HP ���� ������ ��ġ


    private Vector3 questFinVector = new Vector3(29.5f, -3.5f, 0);   // �÷��̾� ������ġ ( ����Ʈ �Ϸ� �������� ��� )
    private float hp_prev;
    private float interactionTime = 0f;                                                                         // Player_Controller.cs���� public
    private Vector3 interactPlayerPosition;                                                                     // Player_Controller.cs���� public
    private float requiredInteractionTime = 10.0f;                                                              // Player_Controller.cs���� public
    private bool following = false;     // �����Ͼ �÷��̾� ����ٴϴ��� ����
    private Vector3 inputDirection;     // �÷��̾� �̵����� ����
    private Vector3 engineerStartVector = new Vector3(-18f, 50f, 0); // �����Ͼ� ���� ��ġ
    private Vector3 dir_temp;   // �÷��̾ ���������� �ٶ� ���� ����
    private float engineerPlayerDistance;   // �����Ͼ� �÷��̾� ���� �Ÿ� ����
    private Vector3 finVector = new Vector3(29.5f, -3.5f, 0);
    private bool questEnd = false;
    private void Start()
    {
        // ������ �� HP �����̴� ���� �ִ� ü�¿� �°� �ʱ�ȭ
        if (hpSlider != null)
        {
            hpSlider.minValue = 0;         // �����̴��� �ּҰ� ����
            hpSlider.maxValue = maxHP;
            hpSlider.value = Engineer_HP;
        }
        QuestManager.instance.currentQuest = 1;
        player_T.position = questFinVector;
        transform.position = engineerStartVector;   // �����Ͼ� ������ġ ���ϱ�
        Debug.Log("����Ʈ1�� �����մϴ�!");
        Debug.Log("�̼� 1 : ��θ� ���󰡼� �����Ͼ �����ϼ���");
    }


    private void Update()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;  // �÷��̾� �̵� ���� ����

        // �����Ͼ� �Ӹ� ���� HP �� ��ġ ����
        if (hpSlider != null)
        {
            Vector3 worldPosition = transform.position - new Vector3 (0 , -0.5f , 0); // �����Ͼ��� ��ġ���� ������ �߰�
            hpSlider.transform.position = worldPosition; // �����̴� ��ġ�� ȭ�� ��ǥ�� ����
        }

        if (inputDirection == Vector3.zero) // �÷��̾ �̵����� ���� ���
        {
            inputDirection = dir_temp; // ������ ������ ���������� �÷��̾� �̵��� �������� ����
        }
        else
        {
            dir_temp = inputDirection;  // ���������� �÷��̾ �̵��� ���� ����
        }
        engineerPlayerDistance = Vector3.Distance(transform.position, player_T.position);   // �÷��̾�� �����Ͼ� ���� �Ÿ� ����

        if (!following && engineerPlayerDistance <= 3f) // �����Ͼ� ���� ����
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.F))   // FŰ�� ������ ��ȣ�ۿ� ����
            {
                //hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f; // ��ȣ�ۿ� �����ϸ� �ð� �ʱ�ȭ�ؼ� ��ȣ�ۿ����� �˸�
                Debug.Log("�����Ͼ� ������ �����մϴ�");
                interactPlayerPosition = player_T.position; // ��ȣ�ۿ� ������ �÷��̾� ��ġ ( ��ȣ�ۿ� ���� ���ǿ� ��� )
                isInteractionStarted = true;
            }

            if (interactionTime > 0f && interactionTime < 10.5f)   // ��ȣ�ۿ�ð��� 0���� ũ�� : ��ȣ�ۿ��� �������̸�
            {
                interactionTime += Time.deltaTime;  // ��ȣ�ۿ� �ð� ����
            }

            if (interactionTime >= requiredInteractionTime)
            {
                Debug.Log("�����Ͼ� ���� �Ϸ�");
                following = true;
                isInteractionStarted = false;
                Debug.Log("�̼�2 : �����Ͼ ��ȣ�ؼ� ������ ��� �������� ���ư�����");
            }
            else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // GŰ�� ������ ��ȣ�ۿ� ����
            {
                Debug.Log("�����Ͼ� ���� �ߴ�");
                isInteractionStarted = false;
                interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
            else if (Player_Stat.instance.HPcurrent < hp_prev)
            {
                Debug.Log("�����Ͼ� ���� ����");
                isInteractionStarted = false;
                interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
        }

        // �����Ͼ ����� ��� ���� �߰� or �����Ͼ� ����� �ڵ����� ����ǵ��� ������ �����ϰų� ]
        if (following)  // �����Ͼ �÷��̾ ����ٴ� ��� : �÷��̾ ������ġ�� ���� Quest1�� Ŭ�����ϰ� Map2�� �Ѿ�� Quest2�� �����ϵ���
        {
            questPosition = finVector;
            transform.position = player_T.position - inputDirection * 1f;   // �����Ͼ �÷��̾� �ڸ� ����ٴ�
            if ((Vector3.Distance(questFinVector, player_T.position)) < 3f && !questEnd) // ����Ʈ1 �Ϸ� ����
            {
                questEnd = true;
                Debug.Log("����Ʈ1 Ŭ����!");
                QuestManager.instance.CompleteQuest();  // ����Ʈ �޴����� ���� ���� Map2�� �̵�
            }
        }

        
        hp_prev = Player_Stat.instance.HPcurrent;
        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log("����Ʈ1 ġƮŰ Ŭ����!");
            QuestManager.instance.CompleteQuest();
        }
    }

    public void TakeDamage(float damage) // �����Ͼ �޴� ������ ����
    {
        Debug.Log("�����Ͼ� ü�� : " + Engineer_HP);
        Engineer_HP -= damage;

        // HP�� 0 ���Ϸ� �������� ��� ó��
        if (Engineer_HP <= 0)
        {
            Engineer_HP = 0;
            Die();
        }
        
        // �����̴� �� ������Ʈ
        if (hpSlider != null)
        {
            hpSlider.value = Engineer_HP;
        }
        
    }

    public void Die() // �����Ͼ� ��� �� ���� ����
    {
        // ���� ������Ʈ�� �ı��ϰ� ���� ���� ó��
        Destroy(this.gameObject);

        // ���� ������ ȭ������ ������ ����Ʈ ���� ����
        QuestManager.instance.currentQuest = 3;
        QuestManager.instance.CompleteQuest();
    }
}