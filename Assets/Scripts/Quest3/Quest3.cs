using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Quest3 : MonoBehaviour
{
    public Player_Controller_Final Player_Controller_Final;
    public GameObject Boss;
    public GameObject Surv1;
    public GameObject Surv2;
    public GameObject Surv3;
    public int surviverCount = 3;
    public bool canInteracting1 = false; // 생존자1와 상호작용 가능 여부
    public bool canInteracting2 = false; // 생존자2와 상호작용 가능 여부
    public bool canInteracting3 = false; // 생존자3와 상호작용 가능 여부
    public bool canInteracting1_temp = false; // 생존자1와 상호작용 여부
    public bool canInteracting2_temp = false; // 생존자2와 상호작용 여부
    public bool canInteracting3_temp = false; // 생존자3와 상호작용 여부
    public float distance1;
    public float distance2;
    public float distance3;
    public int countTemp = 3; 
    private bool hasInteracted = false;
    private bool isEnd = false;

    void Start()
    {
        Surv1 = GameObject.FindGameObjectWithTag("Surv1");
        Surv2 = GameObject.FindGameObjectWithTag("Surv2");
        Surv3 = GameObject.FindGameObjectWithTag("Surv3");
        
        Boss = GameObject.FindGameObjectWithTag("Boss");
        if (Boss == null)
        {
            Debug.LogError("Boss 객체를 찾을 수 없습니다.");
        }
        Boss.SetActive(false);

        Player_Controller_Final = GetComponent<Player_Controller_Final>();
        if (Player_Controller_Final == null)
        {
            Debug.LogError("Player_Controller_Final is missing on the Player.");
        }

        Player_Controller_Final. playerFirstPosition = new Vector3(21f, -3f, 0f);
        Player_Controller_Final.interactionTime = 0f;
        Player_Controller_Final.canInteracting = false;
        Player_Controller_Final.distanceBetweenFirstPosition = 0f;
        Player_Controller_Final.isBossAppear = false;
        Player_Controller_Final.isBossDead = false;
    }

    public void Map3()
    {
        if (Player_Controller_Final.isInteracting && !isEnd)
        {
            Player_Controller_Final.distanceBetweenFirstPosition = Vector3.Distance(Player_Controller_Final.playerFirstPosition, transform.position);
            if ((Player_Controller_Final.distanceBetweenFirstPosition < 5f) && !Player_Controller_Final.isBossAppear)
            {
                // 생존자는 입구에 데려다 주고 보스 몬스터는 플레이어 혼자 잡고 엔딩씬으로 넘어가게끔
                Boss.SetActive(true);
                Player_Controller_Final.isBossAppear = true;
                Debug.Log("보스몬스터 출현!");

                // 테스트용 코드
                // 보스가 나타난 후 5초 뒤에 파괴하는 코루틴 시작
                StartCoroutine(DestroyBossAfterDelay(5f)); // 5초 후에 파괴
            }

            if (Player_Controller_Final.isBossAppear && !Player_Controller_Final.isBossDead)
            {
                if (Boss == null)
                {
                    Debug.Log("보스몬스터 처치!");
                    Player_Controller_Final.isBossDead = true;
                }
            }

            if ((Player_Controller_Final.distanceBetweenFirstPosition < 3f) && Player_Controller_Final.isBossDead)
            {
                Player_Controller_Final.isBossDead = false;
                Debug.Log("엔딩씬으로 넘어가기");
                isEnd = true;
            }
        }
        if (surviverCount > 0)
        { 
            Player_Controller_Final.hp_Cur = Player_Controller_Final.playerstat.HPcurrent;
            // 생존자들과 플레이어 사이의 거리 계산
            distance1 = Vector3.Distance(transform.position, Surv1.transform.position);
            distance2 = Vector3.Distance(transform.position, Surv2.transform.position);
            distance3 = Vector3.Distance(transform.position, Surv3.transform.position);
        
            canInteracting1 = (distance1 <= Player_Controller_Final.interactionRange && !Player_Controller_Final.isInteracting);
            canInteracting2 = (distance2 <= Player_Controller_Final.interactionRange && !Player_Controller_Final.isInteracting);
            canInteracting3 = (distance3 <= Player_Controller_Final.interactionRange && !Player_Controller_Final.isInteracting);

            if (Surv1 == null) Debug.LogWarning("Surv1 is null");
            if (Surv2 == null) Debug.LogWarning("Surv2 is null");
            if (Surv3 == null) Debug.LogWarning("Surv3 is null");

            if (canInteracting1 ^ canInteracting1_temp)
            {
                map3Algo();
                if (countTemp > surviverCount)
                {
                    SpriteRenderer sr = Surv1.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        // Order in Layer를 -1로 설정합니다.
                        sr.sortingOrder = -1;
                    }
                    countTemp--;
                    canInteracting1_temp = true;
                }
            }
            else if (canInteracting2 ^ canInteracting2_temp)
            {
                map3Algo();
                if (countTemp > surviverCount)
                {
                    SpriteRenderer sr = Surv2.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        // Order in Layer를 -1로 설정합니다.
                        sr.sortingOrder = -1;
                    }
                    countTemp--;
                    canInteracting2_temp = true;
                }
            }
            else if (canInteracting3 ^ canInteracting3_temp)
            {
                map3Algo();
                if (countTemp > surviverCount)
                {
                    SpriteRenderer sr = Surv3.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        //Order in Layer를 -1로 설정합니다.
                        sr.sortingOrder = -1;
                    }
                    countTemp--;
                    canInteracting3_temp = true;
                }
            }
        }
    }

    private void map3Algo()
    {
        if (!Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.F))
        {
            Player_Controller_Final.hp_temp = Player_Controller_Final.playerstat.HPcurrent;
            Player_Controller_Final.interactionTime = 0.1f;
            Debug.Log("생존자 구출을 시작합니다");
            hasInteracted = false;
            Player_Controller_Final.isInteractionStarted = !Player_Controller_Final.isInteractionStarted;
            Player_Controller_Final.interactPlayerPosition = transform.position;
        }

        if (Player_Controller_Final.interactionTime > 0f)
        {
            Player_Controller_Final.interactionTime += Time.deltaTime;
        }


        if ((Player_Controller_Final.interactionTime >= Player_Controller_Final.requiredInteractionTime) && !hasInteracted)
        {
            hasInteracted = true;
            if (surviverCount == 1)
            {
                Player_Controller_Final.isInteracting = true;
            }
            Player_Controller_Final.isInteractionStarted = false;  // 상호작용이 끝났으므로 초기화
            surviverCount--;
            Debug.Log("생존자 구출 완료" + " ( 남은 생존자 수 : " + surviverCount + ")");

            // 생존자가 플레이어를 따라가도록 설정
        }
        else if (Player_Controller_Final.isInteractionStarted && Input.GetKey(KeyCode.G))  // 구출 중단 조건
        {
            Debug.Log("구출이 중단되었습니다");
            Player_Controller_Final.isInteractionStarted = false;
            Player_Controller_Final.interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
        }
        else if ((Player_Controller_Final.interactPlayerPosition != transform.position) && Player_Controller_Final.isInteractionStarted || ((Player_Controller_Final.hp_temp != Player_Controller_Final.hp_Cur) && Player_Controller_Final.isInteractionStarted))
        {
            // 상호작용이 중단되었을 때
            Debug.Log("생존자 구출 실패");
            Player_Controller_Final.isInteractionStarted = false;
            Player_Controller_Final.interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
        }
    }

    IEnumerator DestroyBossAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        if (Boss != null) // 보스가 아직 살아 있다면
        {
            Destroy(Boss); // 보스 파괴
            Debug.Log("보스몬스터가 5초 후 파괴되었습니다.");
        }
    }
}




