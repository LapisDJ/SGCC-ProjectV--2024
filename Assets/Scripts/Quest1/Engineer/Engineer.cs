/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : Player
{
    // 엔지니어 전용 체력 설정
    [SerializeField] private float engineerBaseHP = 100.0f; // 엔지니어 기본 HP


    public void TakeDamage(float damage)
    {
        Debug.Log($"엔지니어가 {damage} 데미지를 받았습니다. 남은 체력: {engineerBaseHP}");
    }

    public void Die()
    {
        Debug.Log("엔지니어가 사망했습니다.");
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
    Vector3 dir_temp;   // 플레이어가 마지막으로 바라본 방향 저장
    private void Start()
    {
        transform.position = startVector;
    }
    private void Update()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (inputDirection == Vector3.zero)
        {
            // 플레이어의 바라보는 방향(디폴트) 뒤쪽으로 위치
            inputDirection = dir_temp; // 오른쪽을 기준으로 반대 방향
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
                Debug.Log("엔지니어 구출을 시작합니다");
                interactPlayerPosition = player.position;
                isInteractionStarted = true;
            }

            if (interactionTime > 0f)
            {
                interactionTime += Time.deltaTime;
            }

            if (interactionTime >= requiredInteractionTime)
            {
                Debug.Log("엔지니어 구출 완료");
                following = true;
                Debug.Log("출발 지점으로 돌아가세요");
            }
            else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // 구출 중단 조건
            {
                Debug.Log("엔지니어 구출 중단");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
            //else if ((interactPlayerPosition != player.position) && isInteractionStarted || ((hp_temp != hp_Cur) && isInteractionStarted))
            else if ((interactPlayerPosition != player.position) && isInteractionStarted)
            {
                // 상호작용이 중단되었을 때
                Debug.Log("엔지니어 구출 실패");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
        }


        if (following)
        {
            //transform.position = player.position - player.forward;
            transform.position = player.position - inputDirection * 1f;
            if ((Vector3.Distance(finVector, player.position)) < 3f)
            {
                Debug.Log("퀘스트1 클리어!");
                QuestManager.instance.CompleteQuest();
            }
        }
    }
}