// Player 오브젝트에 넣어서 Map2에서 사용할 스크립트 ( Player_Controller.cs에서 isMap2 이 true일 경우 활성화되게 하며 Map2에서의 할 기능을 다하고 Map3로 넘어갈 경우 isMap2을 false로 바꾸고 isMap3를 true로 반환하여 Map3로 넘어가게끔 할 예정 )
// 플레이어가 엔지니어 구출에 관여하는 기능 ( 엔지니어를 구출하는 조건을 만족하는 경우 상호작용이 가능하도록 ) + Quest2 완료 조건을 넣어서 퀘스트를 완료하면 Quest3를 수행하기 위해 Map3로 넘어가게끔
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Quest2 : MonoBehaviour
{
    public Player_Controller_Final Player_Controller_Final;
    public GameObject signalTower;
    public GameObject middleBoss;
    
    
    void Start()
    {
        signalTower = GameObject.FindGameObjectWithTag("signalTower");
        if (signalTower == null)
        {
            Debug.LogError("signalTower 객체를 찾을 수 없습니다.");
        }
        middleBoss = GameObject.FindGameObjectWithTag("middleBoss");
        if (middleBoss == null)
        {
            Debug.LogError("middleBoss 객체를 찾을 수 없습니다.");
        }
        middleBoss.SetActive(false);
        Player_Controller_Final = GetComponent<Player_Controller_Final>();
        if (Player_Controller_Final == null)
        {
            Debug.LogError("Player_Controller_Final is missing on the Player.");
        }
        Player_Controller_Final. playerFirstPosition = new Vector3(0.5f, -3f, 0f);
        Player_Controller_Final.interactionTime = 0f;
        Player_Controller_Final.canInteracting = false;
        Player_Controller_Final.distanceBetweenFirstPosition = 0f;
    }

    public void Map2()
    {
        if (!Player_Controller_Final.isBossAppear && (Player_Controller_Final.isWithinXRange && Player_Controller_Final.isWithinYRange))
        {
            middleBoss.SetActive(true);
            Player_Controller_Final.isBossAppear = true;
            Debug.Log("보스몬스터 출현!");

            // 테스트용 코드
            // 보스가 나타난 후 5초 뒤에 파괴하는 코루틴 시작
            StartCoroutine(DestroyBossAfterDelay(5f)); // 5초 후에 파괴
        }

        if (Player_Controller_Final.isBossAppear && !Player_Controller_Final.isBossDead)
        {
            if (middleBoss == null)
            {
                Debug.Log("보스몬스터 처치!");
                Player_Controller_Final.isBossDead = true;
            }
        }
        
        if(Player_Controller_Final.isBossDead)
        {
            Player_Controller_Final.hp_Cur = Player_Controller_Final.playerstat.HPcurrent;
            // 신호 타워와 플레이어 사이의 거리 계산
            Player_Controller_Final.distanceBetweenPlayer = Vector3.Distance(transform.position, signalTower.transform.position);

            Player_Controller_Final.canInteracting = (Player_Controller_Final.distanceBetweenPlayer <= Player_Controller_Final.interactionRange && !Player_Controller_Final.isInteracting);

            // 상호작용 가능시 F키로 상호작용 시작
            if (Player_Controller_Final.canInteracting)
            {
                if (!Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.F))
                {
                    Player_Controller_Final.hp_temp = Player_Controller_Final.playerstat.HPcurrent;
                    Player_Controller_Final.interactionTime = 0.1f;
                    Debug.Log("고장난 신호 타워를 복구합니다");
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
                    Debug.Log("신호 타워 복구 완료");
                }
                else if(Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.G))  // 구출 중단 조건
                {
                    Debug.Log("신호 타워 복구 중단");
                    Player_Controller_Final.isInteractionStarted = false;
                    Player_Controller_Final.interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
                }
                else if ((Player_Controller_Final.interactPlayerPosition != transform.position) && Player_Controller_Final.isInteractionStarted || ((Player_Controller_Final.hp_temp != Player_Controller_Final.hp_Cur) && Player_Controller_Final. isInteractionStarted))
                {
                    // 상호작용이 중단되었을 때
                    Debug.Log("신호 타워 복구 실패");
                    Player_Controller_Final.isInteractionStarted = false;
                    Player_Controller_Final.interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
                }
            }
            if (Player_Controller_Final.isInteracting)
            {
                Player_Controller_Final.distanceBetweenFirstPosition = Vector3.Distance(Player_Controller_Final.playerFirstPosition, transform.position);
                if (Player_Controller_Final.distanceBetweenFirstPosition < 3f)
                {
                    Debug.Log("Map3 진입 가능");                                                            // Map3 진입하는 씬으로 변경할 것
                    Player_Controller_Final.isInteracting = false;
                    Player_Controller_Final.isMap2 = false;
                    Player_Controller_Final.isMap3 = true;
                }
            }
        }
    }
    // 테스트용 함수
    // 5초 후에 middleBoss를 파괴하는 코루틴
    IEnumerator DestroyBossAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        if (middleBoss != null) // 보스가 아직 살아 있다면
        {
            Destroy(middleBoss); // 보스 파괴
            Debug.Log("보스몬스터가 5초 후 파괴되었습니다.");
        }
    }
}


