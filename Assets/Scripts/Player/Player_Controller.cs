using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Player_Stat playerstat;
    [SerializeField] public Vector3 dir;    // public 변환
    [SerializeField] float speed;

    // Quest 1 변수 //
    public bool isInteracting = false; // 엔지니어와 상호작용 완료 여부
    public float interactionTime = 0f; // 상호작용중인 시간
    public float requiredInteractionTime = 10.0f; // 필요한 상호작용 시간
    public float interactionRange = 10.0f; // 상호작용 가능 거리 (1타일)
    public bool canInteracting = false; // 엔지니어와 상호작용 가능 여부
    public Vector2 playerLocation; // 플레이어 현재 위치
    public Vector2 playerLocationBefore; // 플레이어 이전 위치
    public GameObject engineer;
    public float distance;
    public Engineer_Controller engineerController;
    // Quest 1 변수 //

    void Start()
    {
        playerstat = GetComponent<Player_Stat>();
        rb = GetComponent<Rigidbody2D>();
        if (playerstat == null)
        {
            Debug.LogError("PlayerStat is missing on the Player.");
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the Player.");
        }
        engineer = GameObject.FindGameObjectWithTag("Engineer");
    }

    void FixedUpdate()
    {
        speed = playerstat.speedAdd * playerstat.speedMulti;
        //플레이어 이동
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
        rb.velocity = dir * speed;

        // 엔지니어와 플레이어 사이의 거리가 1타일 이내일 때 상호작용 //
        playerLocation = rb.transform.position;
        distance = Vector2.Distance(transform.position, engineer.transform.position);
        canInteracting = ( ( distance <= interactionRange )  ); // && ( !isInteracting )
        // 조건문 확인
        if (canInteracting)
        {
            Debug.Log("엔지니어 구출 가능 상태");
            Debug.Log(distance + "    pppppp ");
        }

        /*
        // 상호작용 가능시 E키로 상호작용 시작 //
        if (canInteracting && Input.GetKey(KeyCode.E))
        {
            interactionTime += Time.deltaTime;
            if (interactionTime >= requiredInteractionTime)
            {
                isInteracting = true;
                Debug.Log("엔지니어 구출 완료");
            }
            else
            {
                // 각 초마다 로그 출력 (매 프레임마다 출력되지 않도록 조정)
                int elapsedSeconds = Mathf.FloorToInt(interactionTime);
                switch (elapsedSeconds)
                {
                    case 1:
                        Debug.Log("상호작용 1초 경과");
                        break;
                    case 2:
                        Debug.Log("상호작용 2초 경과");
                        break;
                    case 3:
                        Debug.Log("상호작용 3초 경과");
                        break;
                    case 4:
                        Debug.Log("상호작용 4초 경과");
                        break;
                    case 5:
                        Debug.Log("상호작용 5초 경과");
                        break;
                    case 6:
                        Debug.Log("상호작용 6초 경과");
                        break;
                    case 7:
                        Debug.Log("상호작용 7초 경과");
                        break;
                    case 8:
                        Debug.Log("상호작용 8초 경과");
                        break;
                    case 9:
                        Debug.Log("상호작용 9초 경과");
                        break;
                }
            }
        }
        else
        {
            interactionTime = 0f;  // 상호작용이 중단되면 시간을 초기화
        }

        playerLocationBefore = playerLocation;
        */
    }
}

