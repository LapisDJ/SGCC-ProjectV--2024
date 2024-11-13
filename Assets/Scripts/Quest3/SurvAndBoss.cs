using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UI;

public class SurvAndBoss : PlayerController
{
    public GameObject boss;
    public GameObject targetObject;
    private static int count = 0;
    private Vector3 finVector = new Vector3(2f, 24f, 0);
    private bool isBossAppear = false;
    private float hp_prev;
    private float hp_cur;
    private float interactionTime = 0f;
    private Vector3 interactPlayerPosition;
    private float requiredInteractionTime = 10.0f;
    private bool isBossDead = false;
    private Vector3 interactPosition;
    private Vector3 bossPosition;
    private bool bossAlive = false;
    private float interactReach = 5.0f;
    public Slider interactionSlider1; // 상호작용 게이지 바 연결
    public Slider interactionSlider2;
    public Slider interactionSlider3;
    private void Start()
    {
        GameObject gauzeBar1 = GameObject.Find("Gauze_Bar2"); // Gauze_Bar3 오브젝트 찾기
        if (gauzeBar1 != null)
        {
            interactionSlider1 = gauzeBar1.GetComponentInChildren<Slider>(); // 하위에 있는 Slider 컴포넌트 가져오기
        }
        GameObject gauzeBar2 = GameObject.Find("Gauze_Bar1"); // Gauze_Bar3 오브젝트 찾기
        if (gauzeBar2 != null)
        {
            interactionSlider2 = gauzeBar2.GetComponentInChildren<Slider>(); // 하위에 있는 Slider 컴포넌트 가져오기
        }
        GameObject gauzeBar3 = GameObject.Find("Gauze_Bar3"); // Gauze_Bar3 오브젝트 찾기
        if (gauzeBar3 != null)
        {
            interactionSlider3 = gauzeBar3.GetComponentInChildren<Slider>(); // 하위에 있는 Slider 컴포넌트 가져오기
        }

        // 상호작용 게이지 초기화
        if (interactionSlider1 != null)
        {
            interactionSlider1.minValue = 0f;
            interactionSlider1.maxValue = requiredInteractionTime;
            interactionSlider1.value = 0f; // 초기값 설정
            interactionSlider1.gameObject.SetActive(false); // 상호작용 시작 시에만 표시
        }

        // 상호작용 게이지 초기화
        if (interactionSlider2 != null)
        {
            interactionSlider2.minValue = 0f;
            interactionSlider2.maxValue = requiredInteractionTime;
            interactionSlider2.value = 0f; // 초기값 설정
            interactionSlider2.gameObject.SetActive(false); // 상호작용 시작 시에만 표시
        }
        // 상호작용 게이지 초기화
        if (interactionSlider3 != null)
        {
            interactionSlider3.minValue = 0f;
            interactionSlider3.maxValue = requiredInteractionTime;
            interactionSlider3.value = 0f; // 초기값 설정
            interactionSlider3.gameObject.SetActive(false); // 상호작용 시작 시에만 표시
        }


        GameObject surv1 = GameObject.Find("Surv1");
        if (surv1 != null)
        {
            surv1.transform.position = new Vector3(-4f, 48f, 0f);
        }

        GameObject surv2 = GameObject.Find("Surv2");
        if (surv2 != null)
        {
            surv2.transform.position = new Vector3(-15f, 13f, 0f);
        }

        GameObject surv3 = GameObject.Find("Surv3");
        if (surv3 != null)
        {
            surv3.transform.position = new Vector3(25f, 15f, 0f);
        }


        targetObject = this.gameObject;
        player_T = GameObject.Find("Player").transform;
        interactPosition = targetObject.transform.position;
        interactPosition = new Vector3(interactPosition.x + 3f, interactPosition.y, interactPosition.z);
        boss.transform.position = finVector;
        boss.SetActive(false);
        isInteractionStarted = false;

        GameObject player = GameObject.Find("Player");
        questPosition = new Vector3(27f, 14f, 0);
        Debug.Log("퀘스트3 : 생존자 3명 구출");
        Debug.Log("길 안내를 따라 생존자를 구출하세요!");
    }

    private void Update()
    {
        hp_cur = Player_Stat.instance.HPcurrent;

        if (count < 3)
        {

            if (count == 0)
            {
                questPosition = new Vector3(-12f, 12f, 0f);

            }

            if (count == 1)
            {
                questPosition = new Vector3(-1f, 49f, 0f);

            }

            if (count == 2)
            {
                questPosition = new Vector3(28f, 14f, 0f);

            }
            CheckSurvivorInteraction();
        }
        else
        {
            if (!isBossAppear && count == 3 && Vector3.Distance(player_T.position, finVector) <= 10f)
            {
                Debug.Log("보스몬스터 출현!");
                boss.SetActive(true);
                if (boss.activeSelf)
                {
                    isBossAppear = true;
                }
                bossAlive = true;
            }

            if (bossAlive)
            {
                bossPosition = boss.transform.position;
                questPosition = bossPosition;
            }

            if (boss != null && Input.GetKeyDown(KeyCode.O))
            {
                Destroy(boss);
                bossAlive = false;
            }

            if (!isBossDead && boss == null)
            {
                Debug.Log("보스몬스터 처치!");
                questPosition = finVector;
                Debug.Log("출발 지점으로 돌아가세요");
                isBossDead = true;
            }

            if (boss == null && Vector3.Distance(player_T.position, finVector) <= 3f)
            {
                Debug.Log("퀘스트3 클리어!");
                questPosition = new Vector3(-20f, 50f, 0);  // static 초기화
                isInteractionStarted = false;
                player_T.position = new Vector3(28.5f, -2.5f, 0);
                QuestManager.instance.CompleteQuest();
                count = 0;
            }
        }



        hp_prev = hp_cur;
        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log("퀘스트3 치트키 클리어!");
            questPosition = new Vector3(-20f, 50f, 0);  // static 초기화
            isInteractionStarted = false;
            player_T.position = new Vector3(28.5f, -2.5f, 0);
            QuestManager.instance.CompleteQuest();
            count = 0;
        }
    }


    private void CheckSurvivorInteraction()
    {

        if (Vector3.Distance(targetObject.transform.position, player_T.position) <= interactReach)
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.F))
            {
                interactionTime = 0.1f;
                Debug.Log("생존자 구출 시작");
                isInteractionStarted = true;
            }
            if (interactionTime > 0f)
            {
                interactionTime += Time.deltaTime;
            }
            if (interactionTime >= requiredInteractionTime)
            {
                count++;
                Debug.Log("생존자 " + count + "명을 구출했습니다");

                Destroy(gameObject);
                isInteractionStarted = false;
                if (count == 3)
                {
                    questPosition = finVector;
                    Debug.Log("출발지점으로 돌아가세요");
                }
                else
                {
                    Debug.Log("길 안내를 따라 다음 생존자를 구출하세요!");
                }
            }
            else if (isInteractionStarted && Input.GetKey(KeyCode.G))
            {
                Debug.Log("생존자 구출을 중단합니다");
                isInteractionStarted = false;
                interactionTime = 0f;
            }
            else if (hp_cur < hp_prev)
            {
                Debug.Log("생존자 구출 실패");
                isInteractionStarted = false;
                interactionTime = 0f;
            }
        }
        if (count == 0)
        {
            // 상호작용 진행 상태에 따라 게이지 바를 업데이트
            if (isInteractionStarted)
            {
                // 상호작용 게이지 바 표시
                if (interactionSlider1 != null)
                {
                    interactionSlider1.gameObject.SetActive(true);
                    interactionSlider1.value = interactionTime; // 상호작용 시간에 따라 게이지 업데이트
                }
            }
            else if (interactionSlider1 != null)
            {
                // 상호작용이 끝나거나 중단되면 게이지 바 숨기기
                interactionSlider1.gameObject.SetActive(false);
            }
        }

        if (count == 1)
        {
            // 상호작용 진행 상태에 따라 게이지 바를 업데이트
            if (isInteractionStarted)
            {
                // 상호작용 게이지 바 표시
                if (interactionSlider2 != null)
                {
                    interactionSlider2.gameObject.SetActive(true);
                    interactionSlider2.value = interactionTime; // 상호작용 시간에 따라 게이지 업데이트
                }
            }
            else if (interactionSlider2 != null)
            {
                // 상호작용이 끝나거나 중단되면 게이지 바 숨기기
                interactionSlider2.gameObject.SetActive(false);
            }
        }
        if(count == 2)
        {
            // 상호작용 진행 상태에 따라 게이지 바를 업데이트
            if (isInteractionStarted)
            {
                // 상호작용 게이지 바 표시
                if (interactionSlider3 != null)
                {
                    interactionSlider3.gameObject.SetActive(true);
                    interactionSlider3.value = interactionTime; // 상호작용 시간에 따라 게이지 업데이트
                }
            }
            else if (interactionSlider3 != null)
            {
                // 상호작용이 끝나거나 중단되면 게이지 바 숨기기
                interactionSlider3.gameObject.SetActive(false);
            }
        }
    }
}

