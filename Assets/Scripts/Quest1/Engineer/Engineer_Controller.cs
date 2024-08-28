using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Engineer_Controller : MonoBehaviour
{
    public GameObject Engineer;
    //public Player_Controller_Final Player_Controller_Final;
    public Transform player; // �÷��̾��� ��ġ�� ����
    public float followDistance = 1f; // �÷��̾�� �����Ͼ� ������ �Ÿ�      7f�� �ӽ�
    public bool isFollowing = false;
    Vector2 inputDirection;
    Vector2 dir_temp;   // �÷��̾ ���������� �ٶ� ���� ����

    
    void Start()
    {
        Engineer = GameObject.FindGameObjectWithTag("Engineer");
    }
    
    void Update()
    {
        //if (Player_Controller_Final.isMap1)
        //{
            inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            // �÷��̾ �������� �ʴ� ��쿡 ���� ó��
            if (inputDirection == Vector2.zero)
            {
                // �÷��̾��� �ٶ󺸴� ����(����Ʈ) �������� ��ġ
                inputDirection = dir_temp; // �������� �������� �ݴ� ����
            }
            else
            {
                dir_temp = inputDirection;
            }
            // �����Ͼ �÷��̾ ����ٴϴ� ���
            if (isFollowing)
            {
                FollowPlayer();
            }
        }
    //}

    void FollowPlayer()
    {
        // �÷��̾���� �Ÿ� ���
        float distance = Vector2.Distance(Engineer.transform.position, player.position);

        // �����Ͼ��� ��ǥ ��ġ�� �÷��̾��� �ٶ󺸴� ���� �ݴ������� ���
        Vector2 targetPosition = (Vector2)player.position - inputDirection * followDistance;

        // �����Ͼ ��ǥ ��ġ�� �̵�
        Engineer.transform.position = (Vector2)player.position - inputDirection * followDistance;
    }
}
