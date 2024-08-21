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
    public bool isInteracting = false; // 엔지니어와 상호작용 완료 여부
    public float interactionTime = 0f; // 상호작용중인 시간
    public float requiredInteractionTime = 10.0f; // 필요한 상호작용 시간
    public float interactionRange = 1.0f; // 상호작용 가능 거리
    public bool canInteracting = false; // 엔지니어와 상호작용 가능 여부
    public GameObject engineer;
    public float distance;
    private bool isInteractionStarted = false; // 상호작용이 시작되었는지 여부
    private Engineer_Controller engineerController;

    void Start()
    {
        playerstat = GetComponent<Player_Stat>();
        rb = GetComponent<Rigidbody2D>();
        engineer = GameObject.FindGameObjectWithTag("Engineer");
        engineerController = engineer.GetComponent<Engineer_Controller>();

        if (playerstat == null)
        {
            Debug.LogError("PlayerStat is missing on the Player.");
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the Player.");
        }
    }

    void Update()
    {
        // 플레이어의 이동 처리
        HandleMovement();

        // 상호작용 처리
        HandleInteraction();
    }

    private void HandleMovement()
    {
        // 플레이어 이동 로직
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
        speed = playerstat.speedAdd * playerstat.speedMulti;
        rb.velocity = dir * speed;
    }

    private void HandleInteraction()
    {
        // 엔지니어와 플레이어 사이의 거리 계산
        distance = Vector2.Distance(transform.position, engineer.transform.position);
        canInteracting = (distance <= interactionRange && !isInteracting);

        // 상호작용 가능시 E키로 상호작용 시작
        if (canInteracting )
        {
            if (!isInteractionStarted && Input.GetKey(KeyCode.E))
            {
                Debug.Log("엔지니어 구출을 시작합니다");
                isInteractionStarted = !isInteractionStarted;
            }

            interactionTime += Time.deltaTime;

            if (interactionTime >= requiredInteractionTime)
            {
                isInteracting = true;
                isInteractionStarted = false;  // 상호작용이 끝났으므로 초기화
                Debug.Log("엔지니어 구출 완료");

                // 엔지니어가 플레이어를 따라가도록 설정
                engineerController.isFollowing = true;
            }
        }
        else if (Input.GetKey(KeyCode.P))                                                                   // 조건 수정할거임
        {
            // 상호작용이 중단되었을 때
            Debug.Log("엔지니어 구출 실패");
            isInteractionStarted = false;
            interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
        }
    }
}