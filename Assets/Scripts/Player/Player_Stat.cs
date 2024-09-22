using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Stat : MonoBehaviour
{
    public static Player_Stat instance;
    [SerializeField] public int player_level = 1; // 플레이어 레벨
    [SerializeField] float expgainratebylevel = 1.0f; // 플레이어 경험치
    public float expgainratebypassive = 1.0f;
    [SerializeField] float expgainratefin;
    [SerializeField] float WorkSpeedbylevel = 1.0f; // 플레이어 작업 속도
    public float WorkSpeedbypassive = 1.0f; 
    [SerializeField] float WorkSpeedfin;
    [SerializeField] float Luckbylevel = 0.0f; // 플레이어 행운
    public float Luckbypassive = 0.0f;
    [SerializeField] float Luckfin;
    [SerializeField] float CriticalChance = 0.0f; // 플레이어 치명타 확률
    public float CriticalChancebypassive = 0.0f;
    [SerializeField] float CriticalChancefin;
    //hp
    public float HPcurrent; // 플레이어 현재 HP
    [SerializeField] float HPbylevel = 50.0f; // 플레이어 10랩마다 성장 체력
    [SerializeField] float HPbonus = 1.0f; // 플
    public float HPbypassive;
    public float HPmax; // 플례이어 최대체력
    public float HPpreviousmax;
    //speed
    public float speedAdd = 5.0f;
    public float speedMulti = 1.0f;
    public float speedbypassive;
    public float speedFin;
    //attackdamage
    public float attackDamageByLevel = 5.0f; //플레이어 자체 공격력
    public float attackDamagebypassive;
    public float attackDamagefin;
    public float instancedeathchance;
    public bool isinvincible = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeStats();
    }

    void Update()
    {
        // 레벨 업 테스트용 코드(L키 누르면 레벨업)
        if (Input.GetKeyDown(KeyCode.L))
        {
            LevelUp();
        }
    }
    public void LevelUp()
    {
        player_level++;
        UpdateStatsByLevel();
        UIManager.instance.isskillchoose = true;
    }

    void InitializeStats()
    {
        // 플레이어 초기 스탯 설정
        UpdateStatsByLevel();
    }
    void UpdateStatsByLevel()
    {
        HPpreviousmax = HPbylevel * HPbonus * HPbypassive;
        switch (player_level)
        {
            case 1:
                HPcurrent = HPmax;
                attackDamageByLevel = 5.0f;
                break;
            case 2:
                attackDamageByLevel++;
                break;
            case 3:
                attackDamageByLevel++;
                break;
            case 4:
                attackDamageByLevel++;
                break;
            case 5:
                attackDamageByLevel++;
                Luckbylevel += 0.05f;
                break;
            case 6:
                attackDamageByLevel++;
                break;
            case 7:
                attackDamageByLevel++;
                break;
            case 8:
                attackDamageByLevel++;
                break;
            case 9:
                attackDamageByLevel++;
                break;
            case 10:
                attackDamageByLevel += 3;
                WorkSpeedbylevel += 0.05f;
                Luckbylevel += 0.05f;
                speedMulti += 0.1f;
                HPbonus += 0.1f;
                break;
            case 11:
                attackDamageByLevel++;
                break;
            case 12:
                attackDamageByLevel++;
                break;
            case 13:
                attackDamageByLevel++;
                break;
            case 14:
                attackDamageByLevel++;
                break;
            case 15:
                attackDamageByLevel++;
                Luckbylevel += 0.05f;
                break;
            case 16:
                attackDamageByLevel++;
                break;
            case 17:
                attackDamageByLevel++;
                break;
            case 18:
                attackDamageByLevel++;
                break;
            case 19:
                attackDamageByLevel++;
                break;
            case 20:
                attackDamageByLevel+= 3;
                Luckbylevel += 0.05f;
                WorkSpeedbylevel += 0.05f;
                break;
            case 21:
                attackDamageByLevel++;
                break;
            case 22:
                attackDamageByLevel++;
                break;
            case 23:
                attackDamageByLevel++;
                break;
            case 24:
                attackDamageByLevel++;
                break;
            case 25:
                attackDamageByLevel++;
                Luckbylevel += 0.05f;
                break;
            case 26:
                attackDamageByLevel++;
                break;
            case 27:
                attackDamageByLevel++;
                break;
            case 28:
                attackDamageByLevel++;
                break;
            case 29:
                attackDamageByLevel++;
                break;
            case 30:
                attackDamageByLevel += 3;
                break;
            

            default:
                // 최고 레벨을 넘어가는 경우 처리
                break;
        }
        setStat();
    }

    public bool CheckCritical()
    {
        float randomValue = Random.Range(0f, 1f); // 0에서 1 사이의 랜덤 값을 생성
        return randomValue < CriticalChance;
    }
    public void setStat()
    {
        expgainratefin = expgainratebylevel + expgainratebypassive;
        WorkSpeedfin = WorkSpeedbylevel * WorkSpeedbypassive;
        Luckfin = Luckbylevel + Luckbypassive;
        CriticalChancefin = CriticalChance + CriticalChancebypassive;
        HPmax = HPbylevel * HPbonus * HPbylevel;
        HPcurrent += HPmax - HPpreviousmax;
        speedFin = speedAdd * speedMulti * speedbypassive;
        attackDamagefin = attackDamageByLevel * attackDamagebypassive;
    }
}