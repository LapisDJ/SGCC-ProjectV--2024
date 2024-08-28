// Player ������Ʈ�� �־ Map2���� ����� ��ũ��Ʈ ( Player_Controller.cs���� isMap2 �� true�� ��� Ȱ��ȭ�ǰ� �ϸ� Map2������ �� ����� ���ϰ� Map3�� �Ѿ ��� isMap2�� false�� �ٲٰ� isMap3�� true�� ��ȯ�Ͽ� Map3�� �Ѿ�Բ� �� ���� )
// �÷��̾ �����Ͼ� ���⿡ �����ϴ� ��� ( �����Ͼ �����ϴ� ������ �����ϴ� ��� ��ȣ�ۿ��� �����ϵ��� ) + Quest2 �Ϸ� ������ �־ ����Ʈ�� �Ϸ��ϸ� Quest3�� �����ϱ� ���� Map3�� �Ѿ�Բ�
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Quest2 : MonoBehaviour
{
    public Player_Controller_Final Player_Controller_Final;
    public GameObject signalTower;
    public GameObject middleBoss;
    
    
    void Start()
    {
        signalTower = GameObject.FindGameObjectWithTag("signalTower");
        if (signalTower == null)
        {
            Debug.LogError("signalTower ��ü�� ã�� �� �����ϴ�.");
        }
        middleBoss = GameObject.FindGameObjectWithTag("middleBoss");
        if (middleBoss == null)
        {
            Debug.LogError("middleBoss ��ü�� ã�� �� �����ϴ�.");
        }
        middleBoss.SetActive(false);
        Player_Controller_Final = GetComponent<Player_Controller_Final>();
        if (Player_Controller_Final == null)
        {
            Debug.LogError("Player_Controller_Final is missing on the Player.");
        }
        Player_Controller_Final. playerFirstPosition = new Vector3(0.5f, -3f, 0f);
        Player_Controller_Final.interactionTime = 0f;
        Player_Controller_Final.canInteracting = false;
        Player_Controller_Final.distanceBetweenFirstPosition = 0f;
    }

    public void Map2()
    {
        if (!Player_Controller_Final.isBossAppear && (Player_Controller_Final.isWithinXRange && Player_Controller_Final.isWithinYRange))
        {
            middleBoss.SetActive(true);
            Player_Controller_Final.isBossAppear = true;
            Debug.Log("�������� ����!");

            // �׽�Ʈ�� �ڵ�
            // ������ ��Ÿ�� �� 5�� �ڿ� �ı��ϴ� �ڷ�ƾ ����
            StartCoroutine(DestroyBossAfterDelay(5f)); // 5�� �Ŀ� �ı�
        }

        if (Player_Controller_Final.isBossAppear && !Player_Controller_Final.isBossDead)
        {
            if (middleBoss == null)
            {
                Debug.Log("�������� óġ!");
                Player_Controller_Final.isBossDead = true;
            }
        }
        
        if(Player_Controller_Final.isBossDead)
        {
            Player_Controller_Final.hp_Cur = Player_Controller_Final.playerstat.HPcurrent;
            // ��ȣ Ÿ���� �÷��̾� ������ �Ÿ� ���
            Player_Controller_Final.distanceBetweenPlayer = Vector3.Distance(transform.position, signalTower.transform.position);

            Player_Controller_Final.canInteracting = (Player_Controller_Final.distanceBetweenPlayer <= Player_Controller_Final.interactionRange && !Player_Controller_Final.isInteracting);

            // ��ȣ�ۿ� ���ɽ� FŰ�� ��ȣ�ۿ� ����
            if (Player_Controller_Final.canInteracting)
            {
                if (!Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.F))
                {
                    Player_Controller_Final.hp_temp = Player_Controller_Final.playerstat.HPcurrent;
                    Player_Controller_Final.interactionTime = 0.1f;
                    Debug.Log("���峭 ��ȣ Ÿ���� �����մϴ�");
                    Player_Controller_Final.isInteractionStarted = !Player_Controller_Final.isInteractionStarted;
                    Player_Controller_Final.interactPlayerPosition = transform.position;
                }

                if (Player_Controller_Final.interactionTime > 0f)
                {
                    Player_Controller_Final.interactionTime += Time.deltaTime;
                }


                if (Player_Controller_Final.interactionTime >= Player_Controller_Final.requiredInteractionTime)
                {
                    Player_Controller_Final.isInteracting = true;
                    Player_Controller_Final.isInteractionStarted = false;  // ��ȣ�ۿ��� �������Ƿ� �ʱ�ȭ
                    Debug.Log("��ȣ Ÿ�� ���� �Ϸ�");
                }
                else if(Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.G))  // ���� �ߴ� ����
                {
                    Debug.Log("��ȣ Ÿ�� ���� �ߴ�");
                    Player_Controller_Final.isInteractionStarted = false;
                    Player_Controller_Final.interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
                }
                else if ((Player_Controller_Final.interactPlayerPosition != transform.position) && Player_Controller_Final.isInteractionStarted || ((Player_Controller_Final.hp_temp != Player_Controller_Final.hp_Cur) && Player_Controller_Final. isInteractionStarted))
                {
                    // ��ȣ�ۿ��� �ߴܵǾ��� ��
                    Debug.Log("��ȣ Ÿ�� ���� ����");
                    Player_Controller_Final.isInteractionStarted = false;
                    Player_Controller_Final.interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
                }
            }
            if (Player_Controller_Final.isInteracting)
            {
                Player_Controller_Final.distanceBetweenFirstPosition = Vector3.Distance(Player_Controller_Final.playerFirstPosition, transform.position);
                if (Player_Controller_Final.distanceBetweenFirstPosition < 3f)
                {
                    Debug.Log("Map3 ���� ����");                                                            // Map3 �����ϴ� ������ ������ ��
                    Player_Controller_Final.isInteracting = false;
                    Player_Controller_Final.isMap2 = false;
                    Player_Controller_Final.isMap3 = true;
                }
            }
        }
    }
    // �׽�Ʈ�� �Լ�
    // 5�� �Ŀ� middleBoss�� �ı��ϴ� �ڷ�ƾ
    IEnumerator DestroyBossAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // ������ �ð���ŭ ���
        if (middleBoss != null) // ������ ���� ��� �ִٸ�
        {
            Destroy(middleBoss); // ���� �ı�
            Debug.Log("�������Ͱ� 5�� �� �ı��Ǿ����ϴ�.");
        }
    }
}


