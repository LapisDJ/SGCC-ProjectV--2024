/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : Player
{
    // �����Ͼ� ���� ü�� ����
    [SerializeField] private float engineerBaseHP = 100.0f; // �����Ͼ� �⺻ HP


    public void TakeDamage(float damage)
    {
        Debug.Log($"�����Ͼ {damage} �������� �޾ҽ��ϴ�. ���� ü��: {engineerBaseHP}");
    }

    public void Die()
    {
        Debug.Log("�����Ͼ ����߽��ϴ�.");
    }
}
*/
// �����Ͼ� ü�� ���� �߰��ؼ� ����Ʈ�� ����ؾ��� 

using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Engineer : PlayerController
{
    private Vector3 questFinVector = new Vector3(29.5f, -3.5f, 0);   // �÷��̾� ������ġ ( ����Ʈ �Ϸ� �������� ��� )
    private float hp_prev;
    private float hp_cur;                                                                                         // Player_Controller.cs���� public ( ���� �̱��� )
    private float interactionTime = 0f;                                                                         // Player_Controller.cs���� public
    private Vector3 interactPlayerPosition;                                                                     // Player_Controller.cs���� public
    private float requiredInteractionTime = 10.0f;                                                              // Player_Controller.cs���� public
    private bool following = false;     // �����Ͼ �÷��̾� ����ٴϴ��� ����
    private Vector3 inputDirection;     // �÷��̾� �̵����� ����
    private Vector3 engineerStartVector = new Vector3(-19f, 50f, 0); // �����Ͼ� ���� ��ġ
    private Vector3 dir_temp;   // �÷��̾ ���������� �ٶ� ���� ����
    private float engineerPlayerDistance;   // �����Ͼ� �÷��̾� ���� �Ÿ� ����
    private Vector3 finVector = new Vector3(29.5f, -3.5f, 0);
    private bool questEnd = false;
    private new void Start()
    {
        transform.position = engineerStartVector;   // �����Ͼ� ������ġ ���ϱ�
        Debug.Log("����Ʈ1�� �����մϴ�!");
        Debug.Log("�̼� 1 : ��θ� ���󰡼� �����Ͼ �����ϼ���");
    }


    private void Update()
    {
        GameObject player2 = GameObject.Find("Player"); // Player ������Ʈ ã��
        if (player2 != null)
        {
            PlayerController playerController = player2.GetComponent<PlayerController>();
            if (playerController != null)
            {
                inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;  // �÷��̾� �̵� ���� ����
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
                        playerController.isInteractionStarted = true;
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
                        playerController.isInteractionStarted = false;
                        Debug.Log("�̼�2 : �����Ͼ ��ȣ�ؼ� ������ ��� �������� ���ư�����");
                    }
                    else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // GŰ�� ������ ��ȣ�ۿ� ����
                    {
                        Debug.Log("�����Ͼ� ���� �ߴ�");
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
                    }
                    else if (hp_cur < hp_prev)
                    {
                        Debug.Log("�����Ͼ� ���� ����");
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
                    }
                }

                // �����Ͼ ����� ��� ���� �߰� or �����Ͼ� ����� �ڵ����� ����ǵ��� ������ �����ϰų� ]
                if (following)  // �����Ͼ �÷��̾ ����ٴ� ��� : �÷��̾ ������ġ�� ���� Quest1�� Ŭ�����ϰ� Map2�� �Ѿ�� Quest2�� �����ϵ���
                {
                    playerController.questPosition = finVector;
                    transform.position = player_T.position - inputDirection * 1f;   // �����Ͼ �÷��̾� �ڸ� ����ٴ�
                    if ((Vector3.Distance(questFinVector, player_T.position)) < 3f && !questEnd) // ����Ʈ1 �Ϸ� ����
                    {
                        questEnd = true;
                        Debug.Log("����Ʈ1 Ŭ����!");
                        QuestManager.instance.CompleteQuest();  // ����Ʈ �޴����� ���� ���� Map2�� �̵�
                    }
                }
            }
        }
        hp_prev = hp_cur;
    }
}