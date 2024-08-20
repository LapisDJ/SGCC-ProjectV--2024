using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer_Controller : MonoBehaviour
{
    [SerializeField] Player_Stat playerstat;
    public Transform player; // �÷��̾��� ��ġ�� ����
    public float followDistance = 1f; // �÷��̾�� �����Ͼ� ������ �Ÿ�
    public float moveSpeedMultiplier = 1.0f; // �����Ͼ��� �̵� �ӵ� ������
    private bool isFollowing = false;

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

    void StartFollowing()
    {
        isFollowing = true; // �����Ͼ �÷��̾ ������ ����
    }

    void FollowPlayer()
    {
        // �÷��̾���� �Ÿ� ���
        float distance = Vector2.Distance(transform.position, player.position);

        // �����Ͼ �÷��̾ ������ �Ÿ���ŭ �����ϸ鼭 ���󰡵��� ����
        if (distance > followDistance)
        {
            // �÷��̾� ��ġ���� �����Ͼ��� ��ġ�� �� ���͸� ����ȭ(normalized)�Ͽ� ���� ���͸� ����
            Vector2 direction = ((Vector2)transform.position - (Vector2)player.position).normalized;

            // �÷��̾� ��ġ���� followDistance��ŭ ������ ��ǥ ��ġ�� ���
            Vector2 targetPosition = (Vector2)player.position + direction * followDistance;

            // �����Ͼ ��ǥ ��ġ�� �̵�
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, playerstat.speedFin * moveSpeedMultiplier * Time.deltaTime);
        }
    }
}
