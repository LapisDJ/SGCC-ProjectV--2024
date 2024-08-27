using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Player_Controller_Final : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Player_Stat playerstat;
    [SerializeField] public Vector3 dir;
    [SerializeField] float speed;
    public Quest1 Quest1;
    public Quest2 Quest2;
    public Quest3 Quest3;


    public bool isInteracting = false; // 맵 전체 상호작용 완료 여부
    public float interactionTime = 0f; // 상호작용중인 시간
    public float requiredInteractionTime = 10.0f; // 필요한 상호작용 시간
    public float interactionRange = 5.0f; // 상호작용 가능 거리                                     // Quest1 상호작용 거리 ( 모든 맵 동일하게 사용할 예정 )
    public bool canInteracting = false; // 엔지니어와 상호작용 가능 여부
    public float distanceBetweenPlayer;  // 엔지니어와 플레이어 사이 거리
    public bool isInteractionStarted = false; // 상호작용이 시작되었는지 여부
    public Vector3 playerFirstPosition = new Vector3(0.5f, -3f, 0f);//new Vector3(29.5f, -3.5f, 0f); // 맵에서 플레이어 시작 위치
    public Vector3 interactPlayerPosition; // 상호작용시 플레이어 위치
    public float hp_temp;   // 상호작용시 hp
    public float hp_Cur; // 현재 hp
    public float distanceBetweenFirstPosition = 0f; // 맵에서 플레이어 시작 위치와 현재 위치 사이의 거리
    public bool isBossAppear = false;
    public bool isBossDead = false;
    public bool isWithinXRange = false;     // Quest2 보스몬스터 출현 조건 사용
    public bool isWithinYRange = false;     // Quest2 보스몬스터 출현 조건 사용
    public Engineer_Controller engineerController;
    // 맵 전환 bool 변수
    public bool isMap1 = true;   // map1 일때 Quest1 스크립트 사용 여부
    public bool isMap2 = false;   // map2 진입 여부                                                                       
    public bool isMap3 = false;   // map3 진입 여부
    void Start()
    {
        engineerController = GetComponent<Engineer_Controller>();
        if (engineerController == null)
        {
            Debug.LogError("engineerController is missing on the Player.");
        }


        transform.position = playerFirstPosition;
        playerstat = GetComponent<Player_Stat>();
        rb = GetComponent<Rigidbody2D>();
        Quest1 = GetComponent<Quest1>();
        Quest2 = GetComponent<Quest2>();
        Quest3 = GetComponent<Quest3>();
        if (playerstat == null)
        {
            Debug.LogError("PlayerStat is missing on the Player.");
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the Player.");
        }
        if (Quest1 == null)
        {
            Debug.LogError("Quest1 is missing on the Player.");
        }
        if (Quest2 == null)
        {
            Debug.LogError("Quest2 is missing on the Player.");
        }
        if (Quest3 == null)
        {
            Debug.LogError("Quest3 is missing on the Player.");
        }
    }

    void Update()
    {
        if (isMap2)
        {
            // x 좌표가 -0.5에서 2.5 사이인지 확인
            isWithinXRange = transform.position.x >= -0.5f && transform.position.x <= 2.5f;
            // y 좌표가 34에서 36 사이인지 확인
            isWithinYRange = transform.position.y >= 34f && transform.position.y <= 36f;
        }
        // 플레이어가 상호작용 중일 때는 이동을 막음
        if (!isInteractionStarted)
        {
            // 플레이어 이동 로직
            dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
            speed = playerstat.speedAdd * playerstat.speedMulti;
            rb.velocity = dir * speed;
        }
        else
        {
            // 플레이어가 상호작용 중이라면 움직임을 멈추도록 함
            rb.velocity = Vector2.zero;
        }

        if (isMap1)
        {
            Quest1.Map1();
        }
        if (isMap2)
        {
            Quest2.Map2();
        }
        if (isMap3)
        {
            Quest3.Map3();
        }
    }
}