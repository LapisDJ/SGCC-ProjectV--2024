using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer_Controller : MonoBehaviour
{
    [SerializeField] Player_Stat playerstat;
    public Transform player; // 플레이어의 위치를 저장
    public float followDistance = 1f; // 플레이어와 엔지니어 사이의 거리
    public float moveSpeedMultiplier = 1.0f; // 엔지니어의 이동 속도 보정값
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
        // 엔지니어가 플레이어를 따라다니는 기능
        if (isFollowing)
        {
            FollowPlayer();
        }
    }

    void StartFollowing()
    {
        isFollowing = true; // 엔지니어가 플레이어를 따르게 설정
    }

    void FollowPlayer()
    {
        // 플레이어와의 거리 계산
        float distance = Vector2.Distance(transform.position, player.position);

        // 엔지니어가 플레이어를 지정된 거리만큼 유지하면서 따라가도록 설정
        if (distance > followDistance)
        {
            // 플레이어 위치에서 엔지니어의 위치를 뺀 벡터를 정규화(normalized)하여 방향 벡터를 얻음
            Vector2 direction = ((Vector2)transform.position - (Vector2)player.position).normalized;

            // 플레이어 위치에서 followDistance만큼 떨어진 목표 위치를 계산
            Vector2 targetPosition = (Vector2)player.position + direction * followDistance;

            // 엔지니어를 목표 위치로 이동
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, playerstat.speedFin * moveSpeedMultiplier * Time.deltaTime);
        }
    }
}
