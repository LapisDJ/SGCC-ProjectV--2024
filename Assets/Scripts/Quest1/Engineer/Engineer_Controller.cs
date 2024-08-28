using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Engineer_Controller : MonoBehaviour
{
    public GameObject Engineer;
    //public Player_Controller_Final Player_Controller_Final;
    public Transform player; // 플레이어의 위치를 저장
    public float followDistance = 1f; // 플레이어와 엔지니어 사이의 거리      7f는 임시
    public bool isFollowing = false;
    Vector2 inputDirection;
    Vector2 dir_temp;   // 플레이어가 마지막으로 바라본 방향 저장

    
    void Start()
    {
        Engineer = GameObject.FindGameObjectWithTag("Engineer");
    }
    
    void Update()
    {
        //if (Player_Controller_Final.isMap1)
        //{
            inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            // 플레이어가 움직이지 않는 경우에 대한 처리
            if (inputDirection == Vector2.zero)
            {
                // 플레이어의 바라보는 방향(디폴트) 뒤쪽으로 위치
                inputDirection = dir_temp; // 오른쪽을 기준으로 반대 방향
            }
            else
            {
                dir_temp = inputDirection;
            }
            // 엔지니어가 플레이어를 따라다니는 기능
            if (isFollowing)
            {
                FollowPlayer();
            }
        }
    //}

    void FollowPlayer()
    {
        // 플레이어와의 거리 계산
        float distance = Vector2.Distance(Engineer.transform.position, player.position);

        // 엔지니어의 목표 위치를 플레이어의 바라보는 방향 반대쪽으로 계산
        Vector2 targetPosition = (Vector2)player.position - inputDirection * followDistance;

        // 엔지니어를 목표 위치로 이동
        Engineer.transform.position = (Vector2)player.position - inputDirection * followDistance;
    }
}
