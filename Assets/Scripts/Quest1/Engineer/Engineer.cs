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
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Engineer : PlayerController
{
    public UnityEngine.Transform player;
    private Vector3 finVector = new Vector3(29.5f, -3.5f, 0);
    private float hp_temp;
    private float hp_cur;
    private float interactionTime = 0f;
    private Vector3 interactPlayerPosition;
    private float requiredInteractionTime = 10.0f;
    private bool following = false;
    private Vector3 inputDirection;
    private Vector3 startVector = new Vector3(-19f, 50f, 0);
    Vector3 dir_temp;   // �÷��̾ ���������� �ٶ� ���� ����
    private void Start()
    {
        transform.position = startVector;
    }
    private void Update()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (inputDirection == Vector3.zero)
        {
            // �÷��̾��� �ٶ󺸴� ����(����Ʈ) �������� ��ġ
            inputDirection = dir_temp; // �������� �������� �ݴ� ����
        }
        else
        {
            dir_temp = inputDirection;
        }
        float distance = Vector3.Distance(transform.position, player.position);

        if (!following && distance <= 3f)
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.F))
            {
                //hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f;
                Debug.Log("�����Ͼ� ������ �����մϴ�");
                interactPlayerPosition = player.position;
                isInteractionStarted = true;
            }

            if (interactionTime > 0f)
            {
                interactionTime += Time.deltaTime;
            }

            if (interactionTime >= requiredInteractionTime)
            {
                Debug.Log("�����Ͼ� ���� �Ϸ�");
                following = true;
                Debug.Log("��� �������� ���ư�����");
            }
            else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // ���� �ߴ� ����
            {
                Debug.Log("�����Ͼ� ���� �ߴ�");
                isInteractionStarted = false;
                interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
            //else if ((interactPlayerPosition != player.position) && isInteractionStarted || ((hp_temp != hp_Cur) && isInteractionStarted))
            else if ((interactPlayerPosition != player.position) && isInteractionStarted)
            {
                // ��ȣ�ۿ��� �ߴܵǾ��� ��
                Debug.Log("�����Ͼ� ���� ����");
                isInteractionStarted = false;
                interactionTime = 0f;  // ��ȣ�ۿ��� �ߴܵǸ� �ð��� �ʱ�ȭ
            }
        }


        if (following)
        {
            //transform.position = player.position - player.forward;
            transform.position = player.position - inputDirection * 1f;
            if ((Vector3.Distance(finVector, player.position)) < 3f)
            {
                Debug.Log("����Ʈ1 Ŭ����!");
                QuestManager.instance.CompleteQuest();
            }
        }
    }
}