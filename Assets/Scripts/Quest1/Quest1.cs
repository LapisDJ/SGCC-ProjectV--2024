// Player 오브젝트에 넣어서 Map1에서 사용할 스크립트 ( Player_Controller.cs에서 isMap1 이 true일 경우 활성화되게 하며 Map1에서의 할 기능을 다하고 Map2로 넘어갈 경우 isMap1을 false로 바꾸고 isMap2를 true로 반환하여 Map2로 넘어가게끔 할 예정 )
// 플레이어가 엔지니어 구출에 관여하는 기능 ( 엔지니어를 구출하는 조건을 만족하는 경우 상호작용이 가능하도록 ) + Quest1 완료 조건을 넣어서 퀘스트를 완료하면 Quest2를 수행하기 위해 Map2로 넘어가게끔
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Quest1 : MonoBehaviour
{
    public Player_Controller_Final Player_Controller_Final;
    public GameObject engineer;

    void Start()
    {
        engineer = GameObject.FindGameObjectWithTag("Engineer");
        if (engineer == null)
        {
            Debug.LogError("Engineer 객체를 찾을 수 없습니다.");
        }

        Player_Controller_Final = GetComponent<Player_Controller_Final>();
        if (Player_Controller_Final == null)
        {
            Debug.LogError("Player_Controller_Final is missing on the Player.");
        }
    }

    public void Map1()
    {
        Player_Controller_Final.hp_Cur = Player_Controller_Final.playerstat.HPcurrent;
        // 엔지니어와 플레이어 사이의 거리 계산
        Player_Controller_Final.distanceBetweenPlayer = Vector3.Distance(transform.position, engineer.transform.position);
        Player_Controller_Final.canInteracting = (Player_Controller_Final.distanceBetweenPlayer <= Player_Controller_Final.interactionRange && !Player_Controller_Final.isInteracting);

        // 상호작용 가능시 E키로 상호작용 시작
        if (Player_Controller_Final.canInteracting)
        {
            if (!Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.F))
            {
                Player_Controller_Final.hp_temp = Player_Controller_Final.playerstat.HPcurrent;
                Player_Controller_Final.interactionTime = 0.1f;
                Debug.Log("엔지니어 구출을 시작합니다");
                Player_Controller_Final.isInteractionStarted = !Player_Controller_Final.isInteractionStarted;
                Player_Controller_Final.interactPlayerPosition = transform.position;
            }

            if (Player_Controller_Final.interactionTime > 0f)
            {
                Player_Controller_Final.interactionTime += Time.deltaTime;
            }


            if (Player_Controller_Final.interactionTime >= Player_Controller_Final.requiredInteractionTime)
            {
                Player_Controller_Final.isInteracting = true;
                Player_Controller_Final.isInteractionStarted = false;  // 상호작용이 끝났으므로 초기화
                Debug.Log("엔지니어 구출 완료");

                // 엔지니어가 플레이어를 따라가도록 설정
                Player_Controller_Final.engineerController.isFollowing = true;

            }
            else if(Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.G))  // 구출 중단 조건
            {
                Debug.Log("구출이 중단되었습니다");
                Player_Controller_Final.isInteractionStarted = false;
                Player_Controller_Final.interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
            else if ((Player_Controller_Final.interactPlayerPosition != transform.position) && Player_Controller_Final.isInteractionStarted || ((Player_Controller_Final.hp_temp != Player_Controller_Final.hp_Cur) && Player_Controller_Final. isInteractionStarted))
            {
                // 상호작용이 중단되었을 때
                Debug.Log("엔지니어 구출 실패");
                Player_Controller_Final.isInteractionStarted = false;
                Player_Controller_Final.interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
        }

        if (Player_Controller_Final.isInteracting)
        {
            Player_Controller_Final.distanceBetweenFirstPosition = Vector3.Distance(Player_Controller_Final.playerFirstPosition, transform.position);
            if (Player_Controller_Final.distanceBetweenFirstPosition < 3f)
            {
                Debug.Log("Map2 진입 가능");                                                            // Map2 진입하는 씬으로 변경할 것
                Player_Controller_Final.isInteracting = false;
                Player_Controller_Final.isMap1 = false;
                Player_Controller_Final.isMap2 = true;
            }
        }
    }
}