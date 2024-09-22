using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UI; // UI 관련 라이브러리 추가

public class Engineer : PlayerController
{
    public float Engineer_HP = 100f; // 엔지니어 시작 체력 설정
    public float maxHP = 100f;      // 최대 체력
    public Slider hpSlider;         // HP 바를 연결할 슬라이더
    public Vector3 hpBarOffset = new Vector3(0, -27f, 0); // 엔지니어 머리 위에 표시될 HP 바의 오프셋 위치


    private Vector3 questFinVector = new Vector3(29.5f, -3.5f, 0);   // 플레이어 시작위치 ( 퀘스트 완료 조건으로 사용 )
    private float hp_prev;
    private float interactionTime = 0f;                                                                         // Player_Controller.cs에서 public
    private Vector3 interactPlayerPosition;                                                                     // Player_Controller.cs에서 public
    private float requiredInteractionTime = 10.0f;                                                              // Player_Controller.cs에서 public
    private bool following = false;     // 엔지니어가 플레이어 따라다니는지 여부
    private Vector3 inputDirection;     // 플레이어 이동방향 저장
    private Vector3 engineerStartVector = new Vector3(-18f, 50f, 0); // 엔지니어 시작 위치
    private Vector3 dir_temp;   // 플레이어가 마지막으로 바라본 방향 저장
    private float engineerPlayerDistance;   // 엔지니어 플레이어 사이 거리 저장
    private Vector3 finVector = new Vector3(29.5f, -3.5f, 0);
    private bool questEnd = false;
    private void Start()
    {
        // 시작할 때 HP 슬라이더 값을 최대 체력에 맞게 초기화
        if (hpSlider != null)
        {
            hpSlider.minValue = 0;         // 슬라이더의 최소값 설정
            hpSlider.maxValue = maxHP;
            hpSlider.value = Engineer_HP;
        }
        QuestManager.instance.currentQuest = 1;
        player_T.position = questFinVector;
        transform.position = engineerStartVector;   // 엔지니어 시작위치 정하기
        Debug.Log("퀘스트1을 시작합니다!");
        Debug.Log("미션 1 : 경로를 따라가서 엔지니어를 구출하세요");
    }


    private void Update()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;  // 플레이어 이동 방향 저장

        // 엔지니어 머리 위에 HP 바 위치 설정
        if (hpSlider != null)
        {
            Vector3 worldPosition = transform.position - new Vector3 (0 , -0.5f , 0); // 엔지니어의 위치에서 오프셋 추가
            hpSlider.transform.position = worldPosition; // 슬라이더 위치를 화면 좌표로 설정
        }

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
                isInteractionStarted = false;
                Debug.Log("미션2 : 엔지니어를 보호해서 무사히 출발 지점으로 돌아가세요");
            }
            else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // G키를 누르면 상호작용 종료
            {
                Debug.Log("엔지니어 구출 중단");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
            else if (Player_Stat.instance.HPcurrent < hp_prev)
            {
                Debug.Log("엔지니어 구출 실패");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
        }

        // 엔지니어가 사망한 경우 조건 추가 or 엔지니어 사망시 자동으로 종료되도록 이전에 구현하거나 ]
        if (following)  // 엔지니어가 플레이어를 따라다닐 경우 : 플레이어가 시작위치로 가면 Quest1을 클리어하고 Map2로 넘어가서 Quest2를 실행하도록
        {
            questPosition = finVector;
            transform.position = player_T.position - inputDirection * 1f;   // 엔지니어가 플레이어 뒤를 따라다님
            if ((Vector3.Distance(questFinVector, player_T.position)) < 3f && !questEnd) // 퀘스트1 완료 조건
            {
                questEnd = true;
                Debug.Log("퀘스트1 클리어!");
                QuestManager.instance.CompleteQuest();  // 퀘스트 메니저를 통해 다음 Map2로 이동
            }
        }

        
        hp_prev = Player_Stat.instance.HPcurrent;
        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log("퀘스트1 치트키 클리어!");
            QuestManager.instance.CompleteQuest();
        }
    }

    public void TakeDamage(float damage) // 엔지니어가 받는 데미지 로직
    {
        Debug.Log("엔지니어 체력 : " + Engineer_HP);
        Engineer_HP -= damage;

        // HP가 0 이하로 떨어지면 사망 처리
        if (Engineer_HP <= 0)
        {
            Engineer_HP = 0;
            Die();
        }
        
        // 슬라이더 값 업데이트
        if (hpSlider != null)
        {
            hpSlider.value = Engineer_HP;
        }
        
    }

    public void Die() // 엔지니어 사망 시 게임 오버
    {
        // 게임 오브젝트를 파괴하고 게임 오버 처리
        Destroy(this.gameObject);

        // 게임 마지막 화면으로 가도록 퀘스트 상태 변경
        QuestManager.instance.currentQuest = 3;
        QuestManager.instance.CompleteQuest();
    }
}