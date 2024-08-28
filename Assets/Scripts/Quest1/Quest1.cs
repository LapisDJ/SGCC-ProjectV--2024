// Player ������Ʈ�� �־ Map1���� ����� ��ũ��Ʈ ( Player_Controller.cs���� isMap1 �� true�� ��� Ȱ��ȭ�ǰ� �ϸ� Map1������ �� ����� ���ϰ� Map2�� �Ѿ ��� isMap1�� false�� �ٲٰ� isMap2�� true�� ��ȯ�Ͽ� Map2�� �Ѿ�Բ� �� ���� )
// �÷��̾ �����Ͼ� ���⿡ �����ϴ� ��� ( �����Ͼ �����ϴ� ������ �����ϴ� ��� ��ȣ�ۿ��� �����ϵ��� ) + Quest1 �Ϸ� ������ �־ ����Ʈ�� �Ϸ��ϸ� Quest2�� �����ϱ� ���� Map2�� �Ѿ�Բ�
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Quest1 : MonoBehaviour
{
    public Player_Controller_Final Player_Controller_Final;
    public GameObject engineer;

    void Start()
    {
        engineer = GameObject.FindGameObjectWithTag("Engineer");
        if (engineer == null)
        {
            Debug.LogError("Engineer ��ü�� ã�� �� �����ϴ�.");
        }

        Player_Controller_Final = GetComponent<Player_Controller_Final>();
        if (Player_Controller_Final == null)
        {
            Debug.LogError("Player_Controller_Final is missing on the Player.");
        }
    }

    public void Map1()
    {
        Player_Controller_Final.hp_Cur = Player_Controller_Final.playerstat.HPcurrent;
        // �����Ͼ�� �÷��̾� ������ �Ÿ� ���
        Player_Controller_Final.distanceBetweenPlayer = Vector3.Distance(transform.position, engineer.transform.position);
        Player_Controller_Final.canInteracting = (Player_Controller_Final.distanceBetweenPlayer <= Player_Controller_Final.interactionRange && !Player_Controller_Final.isInteracting);

        // ��ȣ�ۿ� ���ɽ� EŰ�� ��ȣ�ۿ� ����
        if (Player_Controller_Final.canInteracting)
        {
            if (!Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.F))
            {
                Player_Controller_Final.hp_temp = Player_Controller_Final.playerstat.HPcurrent;
                Player_Controller_Final.interactionTime = 0.1f;
                Debug.Log("�����Ͼ� ������ �����մϴ�");
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
                Debug.Log("�����Ͼ� ���� �Ϸ�");

                // �����Ͼ �÷��̾ ���󰡵��� ����
                Player_Controller_Final.engineerController.isFollowing = true;

            }
            else if(Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.G))  // ���� �ߴ� ����
            {
                Debug.Log("������ �ߴܵǾ����ϴ�");
                Player_Controller_Final.isInteractionStarted = false;
                Player_Controller_Final.interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
            else if ((Player_Controller_Final.interactPlayerPosition != transform.position) && Player_Controller_Final.isInteractionStarted || ((Player_Controller_Final.hp_temp != Player_Controller_Final.hp_Cur) && Player_Controller_Final. isInteractionStarted))
            {
                // ��ȣ�ۿ��� �ߴܵǾ��� ��
                Debug.Log("�����Ͼ� ���� ����");
                Player_Controller_Final.isInteractionStarted = false;
                Player_Controller_Final.interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
        }

        if (Player_Controller_Final.isInteracting)
        {
            Player_Controller_Final.distanceBetweenFirstPosition = Vector3.Distance(Player_Controller_Final.playerFirstPosition, transform.position);
            if (Player_Controller_Final.distanceBetweenFirstPosition < 3f)
            {
                Debug.Log("Map2 ���� ����");                                                            // Map2 �����ϴ� ������ ������ ��
                Player_Controller_Final.isInteracting = false;
                Player_Controller_Final.isMap1 = false;
                Player_Controller_Final.isMap2 = true;
            }
        }
    }
}