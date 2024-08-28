using UnityEngine;

public class SurvAndBoss : Player_Controller
{
    public Transform player;
    public GameObject boss;
    private static int count = 0;
    private Vector3 finVector = new Vector3(2f, 24f, 0);
    private bool isBossAppear = false;
    private float hp_temp;
    private float hp_cur;
    private float interactionTime = 0f;
    private Vector3 interactPlayerPosition;
    private float requiredInteractionTime = 10.0f;
    private void Start()
    {
        boss.transform.position = finVector;
        boss.SetActive(false);
        isInteractionStarted = false;
    }

    private void Update()
    {
        if (count < 3)
        {
            CheckSurvivorInteraction();
        }
        else
        {
            if (!isBossAppear && count == 3 && Vector3.Distance(player.position, finVector) <= 10f)
            {
                Debug.Log("�������� ����!");
                boss.SetActive(true);
                if (boss.activeSelf)
                {
                    isBossAppear = true;
                }
            }
            if (boss != null && Input.GetKeyDown(KeyCode.O))
            {
                Destroy(boss); // ���� �ı�
            }

            if (boss == null && Vector3.Distance(player.position, finVector) <= 3f)
            {
                Debug.Log("����Ʈ3 Ŭ����!");
                QuestManager.instance.CompleteQuest();
            }
        }
    }

    private void CheckSurvivorInteraction()
    {
        if (Vector3.Distance(transform.position, player.position) <= 3f )
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.F))
            {
                //hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f;
                Debug.Log("������ ������ �����մϴ�");
                interactPlayerPosition = player.position;
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
            else if ((interactPlayerPosition != player.position) && isInteractionStarted)
            {
                // ��ȣ�ۿ��� �ߴܵǾ��� ��
                Debug.Log("������ ���� ����");
                isInteractionStarted = false;
                interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            } 
        }
    }
}
