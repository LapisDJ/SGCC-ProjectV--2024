using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Surv3_Controller : MonoBehaviour
{
    [SerializeField] Player_Stat playerstat;
    public Transform player; // �÷��̾��� ��ġ�� ����
    public float followDistance = 9f; // �÷��̾�� �����Ͼ� ������ �Ÿ� �ӽð�   
    public bool isFollowing = false;
    Vector2 inputDirection;
    Vector2 dir_temp;   // �÷��̾ ���������� �ٶ� ���� ����

    void Start()
    {
        playerstat = GetComponent<Player_Stat>();
        if (playerstat == null)
        {
            Debug.LogError("PlayerStat is missing on the Player.");
        }
    }

    void Update()
    {
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
        // �÷��̾ �������� ���� ��츦 ����
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing.");
            return;
        }
        // �����Ͼ �÷��̾ ����ٴϴ� ���
        if (isFollowing)
        {
            FollowPlayer();
        }
        
    }

    void FollowPlayer()
    {
        // �÷��̾���� �Ÿ� ���
        float distance = Vector2.Distance(transform.position, player.position);

            // �����Ͼ��� ��ǥ ��ġ�� �÷��̾��� �ٶ󺸴� ���� �ݴ������� ���
            Vector2 targetPosition = (Vector2)player.position - inputDirection * 3f;

        // �����Ͼ ��ǥ ��ġ�� �̵�
        transform.position = (Vector2)player.position - inputDirection * 3f;
    }
}
