using UnityEngine;

public class SurvAndBoss : Player_Controller
{
    public Transform player;
    public GameObject boss;
    private static int count = 0;
    private Vector3 finVector = new Vector3(2f, 24f, 0);
    private bool isBossAppear = false;
    private float hp_temp;
    private float hp_cur;
    private float interactionTime = 0f;
    private Vector3 interactPlayerPosition;
    private float requiredInteractionTime = 10.0f;
    private void Start()
    {
        boss.transform.position = finVector;
        boss.SetActive(false);
        isInteractionStarted = false;
    }

    private void Update()
    {
        if (count < 3)
        {
            CheckSurvivorInteraction();
        }
        else
        {
            if (!isBossAppear && count == 3 && Vector3.Distance(player.position, finVector) <= 10f)
            {
                Debug.Log("보스몬스터 출현!");
                boss.SetActive(true);
                if (boss.activeSelf)
                {
                    isBossAppear = true;
                }
            }
            if (boss != null && Input.GetKeyDown(KeyCode.O))
            {
                Destroy(boss); // 보스 파괴
            }

            if (boss == null && Vector3.Distance(player.position, finVector) <= 3f)
            {
                Debug.Log("퀘스트3 클리어!");
                QuestManager.instance.CompleteQuest();
            }
        }
    }

    private void CheckSurvivorInteraction()
    {
        if (Vector3.Distance(transform.position, player.position) <= 3f )
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.F))
            {
                //hp_temp = playerstat.HPcurrent;
                interactionTime = 0.1f;
                Debug.Log("생존자 구출을 시작합니다");
                interactPlayerPosition = player.position;
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
            else if ((interactPlayerPosition != player.position) && isInteractionStarted)
            {
                // 상호작용이 중단되었을 때
                Debug.Log("생존자 구출 실패");
                isInteractionStarted = false;
                interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
            } 
        }
    }
}
