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
// 엔지니어 체력 조건 추가해서 퀘스트에 사용해야함 

using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Engineer : PlayerController
{
    private Vector3 questFinVector = new Vector3(29.5f, -3.5f, 0);   // 플레이어 시작위치 ( 퀘스트 완료 조건으로 사용 )
    private float hp_prev;
    private float hp_cur;                                                                                         // Player_Controller.cs에서 public ( 아직 미구현 )
    private float interactionTime = 0f;                                                                         // Player_Controller.cs에서 public
    private Vector3 interactPlayerPosition;                                                                     // Player_Controller.cs에서 public
    private float requiredInteractionTime = 10.0f;                                                              // Player_Controller.cs에서 public
    private bool following = false;     // 엔지니어가 플레이어 따라다니는지 여부
    private Vector3 inputDirection;     // 플레이어 이동방향 저장
    private Vector3 engineerStartVector = new Vector3(-19f, 50f, 0); // 엔지니어 시작 위치
    private Vector3 dir_temp;   // 플레이어가 마지막으로 바라본 방향 저장
    private float engineerPlayerDistance;   // 엔지니어 플레이어 사이 거리 저장
    private Vector3 finVector = new Vector3(29.5f, -3.5f, 0);
    private bool questEnd = false;
    private new void Start()
    {
        transform.position = engineerStartVector;   // 엔지니어 시작위치 정하기
        Debug.Log("퀘스트1을 시작합니다!");
        Debug.Log("미션 1 : 경로를 따라가서 엔지니어를 구출하세요");
    }


    private void Update()
    {
        GameObject player2 = GameObject.Find("Player"); // Player 오브젝트 찾기
        if (player2 != null)
        {
            PlayerController playerController = player2.GetComponent<PlayerController>();
            if (playerController != null)
            {
                inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;  // 플레이어 이동 방향 저장
                if (inputDirection == Vector3.zero) // 플레이어가 이동하지 않은 경우
                {
                    inputDirection = dir_temp; // 이전에 저장한 마지막으로 플레이어 이동한 방향으로 저장
                }
                else
                {
                    dir_temp = inputDirection;  // 마지막으로 플레이어가 이동한 방향 저장
                }
                engineerPlayerDistance = Vector3.Distance(transform.position, player_T.position);   // 플레이어와 엔지니어 사이 거리 저장

                if (!following && engineerPlayerDistance <= 3f) // 엔지니어 구출 조건
                {
                    if (!isInteractionStarted && Input.GetKey(KeyCode.F))   // F키를 누르면 상호작용 시작
                    {
                        //hp_temp = playerstat.HPcurrent;
                        interactionTime = 0.1f; // 상호작용 시작하면 시간 초기화해서 상호작용함을 알림
                        Debug.Log("엔지니어 구출을 시작합니다");
                        interactPlayerPosition = player_T.position; // 상호작용 시작한 플레이어 위치 ( 상호작용 중지 조건에 사용 )
                        playerController.isInteractionStarted = true;
                        isInteractionStarted = true;
                    }

                    if (interactionTime > 0f && interactionTime < 10.5f)   // 상호작용시간이 0보다 크면 : 상호작용이 진행중이면
                    {
                        interactionTime += Time.deltaTime;  // 상호작용 시간 갱신
                    }

                    if (interactionTime >= requiredInteractionTime)
                    {
                        Debug.Log("엔지니어 구출 완료");
                        following = true;
                        playerController.isInteractionStarted = false;
                        Debug.Log("미션2 : 엔지니어를 보호해서 무사히 출발 지점으로 돌아가세요");
                    }
                    else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // G키를 누르면 상호작용 종료
                    {
                        Debug.Log("엔지니어 구출 중단");
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
                    }
                    else if (hp_cur < hp_prev)
                    {
                        Debug.Log("엔지니어 구출 실패");
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
                    }
                }

                // 엔지니어가 사망한 경우 조건 추가 or 엔지니어 사망시 자동으로 종료되도록 이전에 구현하거나 ]
                if (following)  // 엔지니어가 플레이어를 따라다닐 경우 : 플레이어가 시작위치로 가면 Quest1을 클리어하고 Map2로 넘어가서 Quest2를 실행하도록
                {
                    playerController.questPosition = finVector;
                    transform.position = player_T.position - inputDirection * 1f;   // 엔지니어가 플레이어 뒤를 따라다님
                    if ((Vector3.Distance(questFinVector, player_T.position)) < 3f && !questEnd) // 퀘스트1 완료 조건
                    {
                        questEnd = true;
                        Debug.Log("퀘스트1 클리어!");
                        QuestManager.instance.CompleteQuest();  // 퀘스트 메니저를 통해 다음 Map2로 이동
                    }
                }
            }
        }
        hp_prev = hp_cur;
    }
}