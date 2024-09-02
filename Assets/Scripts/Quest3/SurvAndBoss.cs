using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SurvAndBoss : PlayerController
{
    //private UnityEngine.Transform player;    // Player 위치 저장
    public GameObject boss; // 보
    public GameObject targetObject;

    private static int count = 0;   // 구출된 생존자 수 ( 3명일 떄는 생존자 구출 완료가 true인 bull 형태로도 활용 )
    private Vector3 finVector = new Vector3(2f, 24f, 0);    // 퀘스트3 마지막 미션에서 도달해야 할 위치
    private bool isBossAppear = false;
    private float hp_temp;
    private float hp_cur;
    private float interactionTime = 0f;
    private Vector3 interactPlayerPosition;
    private float requiredInteractionTime = 10.0f;
    private bool isBossDead = false;    // 보스몬스터 처치여부
    private Vector3 interactPosition;   // 상호작용할 questPosition의 후보들
    private void Start()
    {
        interactPosition = targetObject.transform.position;
        interactPosition = new Vector3(interactPosition.x + 3f, interactPosition.y , interactPosition.z);

        boss.transform.position = finVector;    // 보스 몬스터 시작 위치 정함
        boss.SetActive(false);  // 게임 시작 시 보스 비활성화
        isInteractionStarted = false;   // 생존자 구출에 사용하는 변수 ( 기능 : ... )

        GameObject player = GameObject.Find("Player"); // Player 오브젝트 찾기
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.questPosition = new Vector3 (27f , 14f , 0);
            }
        }

    }

    private void Update()
    {
        if (count < 3)  // 생존자가 모두 구출되지 않은 경우
        {
            if (questPosition != interactPosition)
            {
                if (Vector3.Distance(player_T.transform.position, interactPosition) < Vector3.Distance(player_T.transform.position, questPosition))
                {
                    GameObject player = GameObject.Find("Player"); // Player 오브젝트 찾기
                    if (player != null)
                    {
                        PlayerController playerController = player.GetComponent<PlayerController>();
                        if (playerController != null)
                        {
                            playerController.questPosition = interactPosition;
                        }
                    }
                }
            }
            CheckSurvivorInteraction(); // 생존자 구출하는 미션을 수행함
        }
        else            // 생존자 모두 구출한 경우 나머지 미션을 수행함
        {
            if (!isBossAppear && count == 3 && Vector3.Distance(player_T.position, finVector) <= 10f) // 플레이어가 시작 위치 근처로 간 경우 보스몬스터가 소환됨
            {
                Debug.Log("보스몬스터 출현!");
                boss.SetActive(true);   // 보스몬스터 소환
                if (boss.activeSelf)    // 보스가 소환되면
                {
                    isBossAppear = true;    // 보스가 1회만 소환되도록 함
                }
            }

            // 보스몬스터 처치 치트키
            if (boss != null && Input.GetKeyDown(KeyCode.O))
            {
                Destroy(boss); // 보스 파괴
            }

            if (!isBossDead && boss == null)    // 보스 몬스터를 처지할 경우
            {
                Debug.Log("중간보스를 처치하였습니다!");
                GameObject player = GameObject.Find("Player"); // Player 오브젝트 찾기
                if (player != null)
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.questPosition = finVector;
                    }
                }
                Debug.Log("출발 지점으로 돌아가세요");
                isBossDead = true;
            }

            if (boss == null && Vector3.Distance(player_T.position, finVector) <= 3f) // 보스 몬스터를 처치한 후 시작 위치에 도달하면 퀘스트3 마무리 후 엔딩씬 진입
            {
                Debug.Log("퀘스트3 클리어!");
                QuestManager.instance.CompleteQuest();  // 엔딩씬 맵으로 이동
            }
        }
    }

    private void CheckSurvivorInteraction()
    {
        if (Vector3.Distance(targetObject.transform.position, player_T.position) <= 3f)
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.F))
            {
                //hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f;
                Debug.Log("생존자 구출을 시작합니다");
                interactPlayerPosition = player_T.position;
                isInteractionStarted = true;
            }
            if (interactionTime > 0f)
            {
                interactionTime += Time.deltaTime;
            }

            if (interactionTime >= requiredInteractionTime)
            {
                count++;
                Debug.Log("생존자 " + count + "명 구출 완료");
                Destroy(gameObject);
                isInteractionStarted = false;
                if (count == 3)
                {
                    GameObject player = GameObject.Find("Player"); // Player 오브젝트 찾기
                    if (player != null)
                    {
                        PlayerController playerController = player.GetComponent<PlayerController>();
                        if (playerController != null)
                        {
                            playerController.questPosition = finVector;
                        }
                    }
                    Debug.Log("출발 지점으로 돌아가세요");
                }
            }
            else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // 구출 중단 조건
            {
                Debug.Log("생존자 구출 중단");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
            //else if ((interactPlayerPosition != player.position) && isInteractionStarted || ((hp_temp != hp_Cur) && isInteractionStarted))
            else if ((interactPlayerPosition != player_T.position) && isInteractionStarted)
            {
                // 상호작용이 중단되었을 때
                Debug.Log("생존자 구출 실패");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
        }
    }
}