using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Quest3 : MonoBehaviour
{
    public Player_Controller_Final Player_Controller_Final;
    public GameObject Boss;
    public GameObject Surv1;
    public GameObject Surv2;
    public GameObject Surv3;
    public int surviverCount = 3;
    public bool canInteracting1 = false; // ������1�� ��ȣ�ۿ� ���� ����
    public bool canInteracting2 = false; // ������2�� ��ȣ�ۿ� ���� ����
    public bool canInteracting3 = false; // ������3�� ��ȣ�ۿ� ���� ����
    public bool canInteracting1_temp = false; // ������1�� ��ȣ�ۿ� ����
    public bool canInteracting2_temp = false; // ������2�� ��ȣ�ۿ� ����
    public bool canInteracting3_temp = false; // ������3�� ��ȣ�ۿ� ����
    public float distance1;
    public float distance2;
    public float distance3;
    public int countTemp = 3; 
    private bool hasInteracted = false;
    private bool isEnd = false;

    void Start()
    {
        Surv1 = GameObject.FindGameObjectWithTag("Surv1");
        Surv2 = GameObject.FindGameObjectWithTag("Surv2");
        Surv3 = GameObject.FindGameObjectWithTag("Surv3");
        
        Boss = GameObject.FindGameObjectWithTag("Boss");
        if (Boss == null)
        {
            Debug.LogError("Boss ��ü�� ã�� �� �����ϴ�.");
        }
        Boss.SetActive(false);

        Player_Controller_Final = GetComponent<Player_Controller_Final>();
        if (Player_Controller_Final == null)
        {
            Debug.LogError("Player_Controller_Final is missing on the Player.");
        }

        Player_Controller_Final. playerFirstPosition = new Vector3(21f, -3f, 0f);
        Player_Controller_Final.interactionTime = 0f;
        Player_Controller_Final.canInteracting = false;
        Player_Controller_Final.distanceBetweenFirstPosition = 0f;
        Player_Controller_Final.isBossAppear = false;
        Player_Controller_Final.isBossDead = false;
    }

    public void Map3()
    {
        if (Player_Controller_Final.isInteracting && !isEnd)
        {
            Player_Controller_Final.distanceBetweenFirstPosition = Vector3.Distance(Player_Controller_Final.playerFirstPosition, transform.position);
            if ((Player_Controller_Final.distanceBetweenFirstPosition < 5f) && !Player_Controller_Final.isBossAppear)
            {
                // �����ڴ� �Ա��� ������ �ְ� ���� ���ʹ� �÷��̾� ȥ�� ��� ���������� �Ѿ�Բ�
                Boss.SetActive(true);
                Player_Controller_Final.isBossAppear = true;
                Debug.Log("�������� ����!");

                // �׽�Ʈ�� �ڵ�
                // ������ ��Ÿ�� �� 5�� �ڿ� �ı��ϴ� �ڷ�ƾ ����
                StartCoroutine(DestroyBossAfterDelay(5f)); // 5�� �Ŀ� �ı�
            }

            if (Player_Controller_Final.isBossAppear && !Player_Controller_Final.isBossDead)
            {
                if (Boss == null)
                {
                    Debug.Log("�������� óġ!");
                    Player_Controller_Final.isBossDead = true;
                }
            }

            if ((Player_Controller_Final.distanceBetweenFirstPosition < 3f) && Player_Controller_Final.isBossDead)
            {
                Player_Controller_Final.isBossDead = false;
                Debug.Log("���������� �Ѿ��");
                isEnd = true;
            }
        }
        if (surviverCount > 0)
        { 
            Player_Controller_Final.hp_Cur = Player_Controller_Final.playerstat.HPcurrent;
            // �����ڵ�� �÷��̾� ������ �Ÿ� ���
            distance1 = Vector3.Distance(transform.position, Surv1.transform.position);
            distance2 = Vector3.Distance(transform.position, Surv2.transform.position);
            distance3 = Vector3.Distance(transform.position, Surv3.transform.position);
        
            canInteracting1 = (distance1 <= Player_Controller_Final.interactionRange && !Player_Controller_Final.isInteracting);
            canInteracting2 = (distance2 <= Player_Controller_Final.interactionRange && !Player_Controller_Final.isInteracting);
            canInteracting3 = (distance3 <= Player_Controller_Final.interactionRange && !Player_Controller_Final.isInteracting);

            if (Surv1 == null) Debug.LogWarning("Surv1 is null");
            if (Surv2 == null) Debug.LogWarning("Surv2 is null");
            if (Surv3 == null) Debug.LogWarning("Surv3 is null");

            if (canInteracting1 ^ canInteracting1_temp)
            {
                map3Algo();
                if (countTemp > surviverCount)
                {
                    SpriteRenderer sr = Surv1.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        // Order in Layer�� -1�� �����մϴ�.
                        sr.sortingOrder = -1;
                    }
                    countTemp--;
                    canInteracting1_temp = true;
                }
            }
            else if (canInteracting2 ^ canInteracting2_temp)
            {
                map3Algo();
                if (countTemp > surviverCount)
                {
                    SpriteRenderer sr = Surv2.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        // Order in Layer�� -1�� �����մϴ�.
                        sr.sortingOrder = -1;
                    }
                    countTemp--;
                    canInteracting2_temp = true;
                }
            }
            else if (canInteracting3 ^ canInteracting3_temp)
            {
                map3Algo();
                if (countTemp > surviverCount)
                {
                    SpriteRenderer sr = Surv3.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        //Order in Layer�� -1�� �����մϴ�.
                        sr.sortingOrder = -1;
                    }
                    countTemp--;
                    canInteracting3_temp = true;
                }
            }
        }
    }

    private void map3Algo()
    {
        if (!Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.F))
        {
            Player_Controller_Final.hp_temp = Player_Controller_Final.playerstat.HPcurrent;
            Player_Controller_Final.interactionTime = 0.1f;
            Debug.Log("������ ������ �����մϴ�");
            hasInteracted = false;
            Player_Controller_Final.isInteractionStarted = !Player_Controller_Final.isInteractionStarted;
            Player_Controller_Final.interactPlayerPosition = transform.position;
        }

        if (Player_Controller_Final.interactionTime > 0f)
        {
            Player_Controller_Final.interactionTime += Time.deltaTime;
        }


        if ((Player_Controller_Final.interactionTime >= Player_Controller_Final.requiredInteractionTime) && !hasInteracted)
        {
            hasInteracted = true;
            if (surviverCount == 1)
            {
                Player_Controller_Final.isInteracting = true;
            }
            Player_Controller_Final.isInteractionStarted = false;  // ��ȣ�ۿ��� �������Ƿ� �ʱ�ȭ
            surviverCount--;
            Debug.Log("������ ���� �Ϸ�" + " ( ���� ������ �� : " + surviverCount + ")");

            // �����ڰ� �÷��̾ ���󰡵��� ����
        }
        else if (Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.G))  // ���� �ߴ� ����
        {
            Debug.Log("������ �ߴܵǾ����ϴ�");
            Player_Controller_Final.isInteractionStarted = false;
            Player_Controller_Final.interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
        }
        else if ((Player_Controller_Final.interactPlayerPosition != transform.position) && Player_Controller_Final.isInteractionStarted || ((Player_Controller_Final.hp_temp != Player_Controller_Final.hp_Cur) && Player_Controller_Final.isInteractionStarted))
        {
            // ��ȣ�ۿ��� �ߴܵǾ��� ��
            Debug.Log("������ ���� ����");
            Player_Controller_Final.isInteractionStarted = false;
            Player_Controller_Final.interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
        }
    }

    IEnumerator DestroyBossAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // ������ �ð���ŭ ���
        if (Boss != null) // ������ ���� ��� �ִٸ�
        {
            Destroy(Boss); // ���� �ı�
            Debug.Log("�������Ͱ� 5�� �� �ı��Ǿ����ϴ�.");
        }
    }
}




