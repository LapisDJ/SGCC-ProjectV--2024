using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer_Controller : MonoBehaviour
{
    [SerializeField] Player_Stat playerstat;
    public Transform player; // �÷��̾��� ��ġ�� ����
    public float followDistance = 7f; // �÷��̾�� �����Ͼ� ������ �Ÿ�      7f�� �ӽ�
    public float moveSpeedMultiplier = 1.0f; // �����Ͼ��� �̵� �ӵ� ������
    public bool isFollowing = false;

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

        // �����Ͼ �÷��̾ ������ �Ÿ���ŭ �����ϸ鼭 ���󰡵��� ����
        if (distance > followDistance)
        {
            // �÷��̾� ��ġ���� �����Ͼ��� ��ġ�� �� ���͸� ����ȭ�Ͽ� ���� ���͸� ����
            Vector2 direction = (player.position - transform.position).normalized;

            // �÷��̾� ��ġ���� followDistance��ŭ ������ ��ǥ ��ġ�� ���
            Vector2 targetPosition = (Vector2)player.position - direction * followDistance;

            // �����Ͼ ��ǥ ��ġ�� �̵�
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, 10f * Time.deltaTime);         // 10f �� �ӽ�
        }
    }
}
