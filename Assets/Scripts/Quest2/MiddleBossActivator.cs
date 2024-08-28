using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MiddleBossActivator : PlayerController
{
    public GameObject middleBoss;
    public UnityEngine.Transform player;
    private bool isRestore = false;
    private Vector3 finVector = new Vector3(0.5f, -3f, 0);
    private float hp_temp;
    private float hp_cur;
    private bool isBossAppear = false;
    private float interactionTime = 0f;
    private Vector3 interactPlayerPosition;
    private float requiredInteractionTime = 10.0f;
    private void Start()
    {
        middleBoss.SetActive(false);
        isInteractionStarted = false;
    }
    private void Update()
    {
        //hp_cur = playerstat.HPcurrent;
        bool isWithinXRange = player.position.x >= -0.5f && player.position.x <= 2.5f;
        bool isWithinYRange = player.position.y >= 34f && player.position.y <= 36f;

        if (isWithinXRange && isWithinYRange && !isBossAppear)
        {
            middleBoss.SetActive(true);
            Debug.Log("보스몬스터 출현!");
            isBossAppear = true;
        }

        if (middleBoss != null && Input.GetKeyDown(KeyCode.O))
        {
            Destroy(middleBoss); // 보스 파괴
        }
        if (middleBoss == null && Vector3.Distance(player.position, transform.position) <= 3f && !isRestore)
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.F))
            {
                //hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f;
                Debug.Log("고장난 신호 타워를 복구합니다");
                interactPlayerPosition = player.position;
                isInteractionStarted = true;
            }

            if (interactionTime > 0f)
            {
                interactionTime += Time.deltaTime;
            }

            if (interactionTime >= requiredInteractionTime)
            {
                Debug.Log("신호 타워 복구 완료");
                isRestore = true;
                Debug.Log("출발 지점으로 돌아가세요");
            }
            else if (isInteractionStarted && Input.GetKey(KeyCode.G))  // 구출 중단 조건
            {
                Debug.Log("신호 타워 복구 중단");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
            //else if ((interactPlayerPosition != player.position) && isInteractionStarted || ((hp_temp != hp_Cur) && isInteractionStarted))
            else if ((interactPlayerPosition != player.position) && isInteractionStarted)
            {
                // 상호작용이 중단되었을 때
                Debug.Log("신호 타워 복구 실패");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            }
        }

        if (isRestore && (Vector3.Distance(finVector, player.position)) < 3f)
        {
            Debug.Log("퀘스트2 클리어!");
            QuestManager.instance.CompleteQuest();
        }
    }
}