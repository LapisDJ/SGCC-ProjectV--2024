using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
    private new void Start()
    {
        GameObject surv1 = GameObject.Find("Surv1");
        if (surv1 != null)
        {
           surv1.transform.position = new Vector3(-5f, 48f, 0f);
        }

        GameObject surv2 = GameObject.Find("Surv2");
        if (surv2 != null)
        {
            surv2.transform.position = new Vector3(-15.5f, 12.5f, 0f);
        }

        GameObject surv3 = GameObject.Find("Surv3");
        if (surv3 != null)
        {
            surv3.transform.position = new Vector3(24.5f, 14.5f, 0f);
        }


        targetObject = this.gameObject;
        player_T = GameObject.Find("Player").transform;
        interactPosition = targetObject.transform.position;
        interactPosition = new Vector3(interactPosition.x + 3f, interactPosition.y, interactPosition.z);
        boss.transform.position = finVector;    
        boss.SetActive(false); 
        isInteractionStarted = false;   

        GameObject player = GameObject.Find("Player"); 
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.questPosition = new Vector3(27f, 14f, 0);
            }
        }
        questPosition = new Vector3(27f, 14f, 0);
        Debug.Log("퀘스트3 : 생존자 3명 구출");
        Debug.Log("길 안내를 따라 생존자를 구출하세요!");
    }

    private void Update()
    {
        GameObject player = GameObject.Find("Player"); 
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                hp_cur = Player_Stat.instance.HPcurrent;

                if (count < 3) 
                {
                    
                    if(count == 0)
                    {
                        playerController.questPosition = new Vector3(-12f, 12f, 0f);
                        questPosition = new Vector3(-12f, 12f, 0f);
                        
                    }

                    if (count == 1)
                    {
                        playerController.questPosition = new Vector3(-1f, 49f, 0f);
                        questPosition = new Vector3(-1f, 49f, 0f);

                    }

                    if (count == 2)
                    {
                        playerController.questPosition = new Vector3(28f, 14f, 0f);
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
                        playerController.questPosition = bossPosition;
                    }
                    
                    if (boss != null && Input.GetKeyDown(KeyCode.O))
                    {
                        Destroy(boss);
                        bossAlive = false;
                    }

                    if (!isBossDead && boss == null)    
                    {
                        Debug.Log("보스몬스터 처치!");
                        playerController.questPosition = finVector;
                        Debug.Log("출발 지점으로 돌아가세요");
                        isBossDead = true;
                    }

                    if (boss == null && Vector3.Distance(player_T.position, finVector) <= 3f)
                    {
                        Debug.Log("퀘스트3 클리어!");
                        QuestManager.instance.CompleteQuest();  
                    }
                }

            }
        }
        hp_prev = hp_cur;
    }

    private void CheckSurvivorInteraction() 
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                if (Vector3.Distance(targetObject.transform.position, player_T.position) <= interactReach)
                {
                    if (!isInteractionStarted && Input.GetKey(KeyCode.F))
                    {
                        interactionTime = 0.1f;
                        Debug.Log("생존자 구출 시작");
                        playerController.isInteractionStarted = true;
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
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        if (count == 3)
                        {
                            playerController.questPosition = finVector;
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
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        interactionTime = 0f;
                    }
                    else if (hp_cur < hp_prev)
                    {
                        Debug.Log("생존자 구출 실패");
                        playerController.isInteractionStarted = false;
                        isInteractionStarted = false;
                        interactionTime = 0f; 
                    }
                }
            }
        }
    }
}

