using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MiddleBossActivator : PlayerController
{
    public GameObject middleBoss;
    public UnityEngine.Transform player;
    private bool isRestore = false;
    private Vector3 finVector = new Vector3(0.5f, -3f, 0);
    private float hp_temp;
    private float hp_cur;
    private bool isBossAppear = false;
    private float interactionTime = 0f;
    private Vector3 interactPlayerPosition;
    private float requiredInteractionTime = 10.0f;
    private void Start()
    {
        middleBoss.SetActive(false);
        isInteractionStarted = false;
    }
    private void Update()
    {
        //hp_cur = playerstat.HPcurrent;
        bool isWithinXRange = player.position.x >= -0.5f && player.position.x <= 2.5f;
        bool isWithinYRange = player.position.y >= 34f && player.position.y <= 36f;

        if (isWithinXRange && isWithinYRange && !isBossAppear)
        {
            middleBoss.SetActive(true);
            Debug.Log("�������� ����!");
            isBossAppear = true;
        }

        if (middleBoss != null && Input.GetKeyDown(KeyCode.O))
        {
            Destroy(middleBoss); // ���� �ı�
        }
        if (middleBoss == null && Vector3.Distance(player.position, transform.position) <= 3f && !isRestore)
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.F))
            {
                //hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f;
                Debug.Log("���峭 ��ȣ Ÿ���� �����մϴ�");
                interactPlayerPosition = player.position;
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