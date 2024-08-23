using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Player_Stat playerstat;
    [SerializeField] public Vector3 dir;
    [SerializeField] float speed;

    // Quest 1 변수 //
    public bool isInteracting = false; // 맵 전체 상호작용 완료 여부

    public float interactionTime = 0f; // 상호작용중인 시간
    public float requiredInteractionTime = 10.0f; // 필요한 상호작용 시간
    public float interactionRange = 5.0f; // 상호작용 가능 거리

    public bool canInteracting = false; // 엔지니어와 상호작용 가능 여부
    public bool canInteracting1 = false; // 생존자1와 상호작용 가능 여부
    public bool canInteracting2 = false; // 생존자2와 상호작용 가능 여부
    public bool canInteracting3 = false; // 생존자3와 상호작용 가능 여부
    public bool canInteracting1_temp = false; // 생존자1와 상호작용 여부
    public bool canInteracting2_temp = false; // 생존자2와 상호작용 여부
    public bool canInteracting3_temp = false; // 생존자3와 상호작용 여부

    public GameObject engineer;
    public GameObject Surv1;
    public GameObject Surv2;
    public GameObject Surv3;

    public float distance;  // 엔지니어와 플레이어 사이 거리
    public float distance1;
    public float distance2;
    public float distance3;

    public bool isInteractionStarted = false; // 상호작용이 시작되었는지 여부

    public Engineer_Controller engineerController;
    public Surv1_Controller Surv1Controller;
    public Surv2_Controller Surv2Controller;
    public Surv3_Controller Surv3Controller;

    public Vector3 playerFirst; // 맵에서 플레이어 시작 위치
    public Vector3 tempPlayerPosition; // 상호작용시 플레이어 위치
    public float hp_temp;   // 상호작용시 hp
    public float hp_Cur; // 현재 hp

    public float distance_first; // 맵에서 플레이어 시작 위치와 현재 위치 사이의 거리
    public bool isMap2 = false;   // map2 진입시 함수 호출 여부                                                                       
    public bool isMap3 = true;   // map3 진입시 함수 호출 여부                                                        ( 테스트시에는 true 설정 )
    
    public int surviverCount = 3;   // 맵3 남은 생존자 수
    public int countTemp = 3;
    public bool reset = false;  // 각 맵마다 시작 위치 리셋할때 사용
    void Start()
    {
        playerstat = GetComponent<Player_Stat>();
        rb = GetComponent<Rigidbody2D>();
        engineer = GameObject.FindGameObjectWithTag("Engineer");
        engineerController = engineer.GetComponent<Engineer_Controller>();
        Surv1 = GameObject.FindGameObjectWithTag("Surv1");
        Surv1Controller = Surv1.GetComponent<Surv1_Controller>();
        Surv2 = GameObject.FindGameObjectWithTag("Surv2");
        Surv2Controller = Surv2.GetComponent<Surv2_Controller>();
        Surv3 = GameObject.FindGameObjectWithTag("Surv3");
        Surv3Controller = Surv3.GetComponent<Surv3_Controller>();

        if (playerstat == null)
        {
            Debug.LogError("PlayerStat is missing on the Player.");
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the Player.");
        }
        playerFirst = transform.position;
    }

    void Update()
    {
        if (reset)
        {
            playerFirst = transform.position;
        }
        hp_Cur = playerstat.HPcurrent;
        distance_first = Vector3.Distance(playerFirst, transform.position);
        // 플레이어의 이동 처리
        HandleMovement();

        // Map1 상호작용 처리
        //if (engineerController.isMap1)
        //{
        //    Map1();
        //}
        // Map2 상호작용 처리
        //if (isMap2)
        //{
        //    Map2();
        //}
        // Map3 상호작용 처리
        if (isMap3)
        {
            Map3();
        }
    }

    private void HandleMovement()
    {
        // 플레이어 이동 로직
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
        speed = playerstat.speedAdd * playerstat.speedMulti;
        rb.velocity = dir * speed;
    }

    private void Map1()
    {
        // 엔지니어와 플레이어 사이의 거리 계산
        distance = Vector3.Distance(transform.position, engineer.transform.position);
        canInteracting = (distance <= interactionRange && !isInteracting);

        // 상호작용 가능시 E키로 상호작용 시작
        if (canInteracting )
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.E))
            {
                hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f;
                Debug.Log("엔지니어 구출을 시작합니다");
                isInteractionStarted = !isInteractionStarted;
                tempPlayerPosition = transform.position;
            }

            if (interactionTime > 0f)
            {
                interactionTime += Time.deltaTime;
            }

            if (interactionTime >= requiredInteractionTime)
            {
                isInteracting = true;
                isInteractionStarted = false;  // 상호작용이 끝났으므로 초기화
                Debug.Log("엔지니어 구출 완료");

                // 엔지니어가 플레이어를 따라가도록 설정
                engineerController.isFollowing = true;
                
            }
            else if (  (tempPlayerPosition != transform.position)  && isInteractionStarted || ((hp_temp != hp_Cur) && isInteractionStarted))                                  
            {
                // 상호작용이 중단되었을 때
                Debug.Log("엔지니어 구출 실패");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
        }

        if ( isInteracting )
        {
            if (distance_first < 3f)
            {
                Debug.Log("Map2 진입 가능");                                                            // Map2 진입하는 씬으로 변경할 것
                isInteracting = false;
                engineerController.isMap1 = false;
                isMap2 = true;
                reset = true;
            }
        }
       
    }

    private void Map2() // Map1 코드 재사용
    {
        reset = false;
        // 신호 타워와 플레이어 사이의 거리 계산
        distance = Vector3.Distance(transform.position, engineer.transform.position);
        canInteracting = (distance <= interactionRange && !isInteracting);

        // 상호작용 가능시 E키로 상호작용 시작
        if (canInteracting)
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.E))
            {
                hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f;
                Debug.Log("고장난 신호 타워를 복구합니다");
                isInteractionStarted = !isInteractionStarted;
                tempPlayerPosition = transform.position;
            }

            if (interactionTime > 0f)
            {
                interactionTime += Time.deltaTime;
            }

            if (interactionTime >= requiredInteractionTime)
            {
                isInteracting = true;
                isInteractionStarted = false;  // 상호작용이 끝났으므로 초기화
                Debug.Log("신호 타워를 복구 완료");
                
                
            }
            else if ((tempPlayerPosition != transform.position) && isInteractionStarted || ( (hp_temp != hp_Cur) && isInteractionStarted ))
            {
                // 상호작용이 중단되었을 때
                Debug.Log("신호 타워를 복구 실패");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
        }

        if (isInteracting)
        {
            if (distance_first < 3f)
            {
                Debug.Log("Map3 진입 가능");                                                            // Map3 진입하는 씬으로 변경할 것
                isInteracting = false;
                isMap3 = true;
                isMap2 = false;
                reset = true;
            }
        }

    }







    private void Map3()
    {
        reset = false;
        // 생존자들과 플레이어 사이의 거리 계산
        distance1 = Vector3.Distance(transform.position, Surv1.transform.position);
        distance2 = Vector3.Distance(transform.position, Surv2.transform.position);
        distance3 = Vector3.Distance(transform.position, Surv3.transform.position);
        if (surviverCount > 0)
        {
            
            canInteracting1 = (distance1 <= interactionRange && !isInteracting);
            canInteracting2 = (distance2 <= interactionRange && !isInteracting);
            canInteracting3 = (distance3 <= interactionRange && !isInteracting);


            if (Surv1 == null) Debug.LogWarning("Surv1 is null");
            if (Surv2 == null) Debug.LogWarning("Surv2 is null");
            if (Surv3 == null) Debug.LogWarning("Surv3 is null");


            if (canInteracting1 ^ canInteracting1_temp)
            {
                
                map3Algo();
                if(countTemp > surviverCount)
                {
                    Surv1Controller.isFollowing = true;
                    countTemp--;
                    canInteracting1_temp = true;
                }
            }
            else if (canInteracting2 ^ canInteracting2_temp)
            {
                
                map3Algo();
                if (countTemp > surviverCount)
                {
                    Surv2Controller.isFollowing = true;
                    countTemp--;
                    canInteracting2_temp = true;
                }
            }
            else if (canInteracting3 ^ canInteracting3_temp)
            {
                map3Algo();
                if (countTemp > surviverCount)
                {
                    Surv3Controller.isFollowing = true;
                    countTemp--;
                    canInteracting3_temp = true;
                }
            }
            // 상호작용 가능시 E키로 상호작용 시작

            
            

        }
        if (isInteracting)
        {
            if (distance_first < 3f)
            {
                isMap3 = false;
                Debug.Log("보스 몬스터 출현");          // 생존자는 입구에 데려다 주고 보스 몬스터는 플레이어 혼자 잡고 엔딩씬으로 넘어가게끔

            }
        }
        
    }

    private void map3Algo()
    {
        //if (!(canInteracting1 && canInteracting2 && canInteracting3))
        //{
            if (!isInteractionStarted && Input.GetKey(KeyCode.E))
            {
                hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f;
                Debug.Log("생존자 구출을 시작합니다");
                isInteractionStarted = !isInteractionStarted;
                tempPlayerPosition = transform.position;
            }

            if (interactionTime > 0f)
            {
                interactionTime += Time.deltaTime;
            }

            if (interactionTime >= requiredInteractionTime)
            {
                interactionTime = 0;
                if (surviverCount == 1)
                {
                    isInteracting = true;
                }
                isInteractionStarted = false;  // 상호작용이 끝났으므로 초기화
            surviverCount--;
            Debug.Log("생존자 구출 완료" + " ( 남은 생존자 수 : " + surviverCount + ")");

                // 엔지니어가 플레이어를 따라가도록 설정
                
            }
            else if ((tempPlayerPosition != transform.position) && isInteractionStarted || ((hp_temp != hp_Cur) && isInteractionStarted))
            {
                // 상호작용이 중단되었을 때
                Debug.Log("생존자 구출 실패");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
        //}
    }
}