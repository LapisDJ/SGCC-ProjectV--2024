using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Stat : MonoBehaviour
{
    [SerializeField] int player_level = 1; // 플레이어 레벨
    [SerializeField] float expgainrate = 1.0f; // 플레이어 경험치
    [SerializeField] float WorkSpeed = 1.0f; // 플레이어 작업 속도
    [SerializeField] float Luck = 0.0f; // 플레이어 행운
    [SerializeField] float CriticalChance = 0.0f; // 플레이어 치명타 확률
    //hp
    public float HPcurrent; // 플레이어 현재 HP
    [SerializeField] float HPbylevel = 50.0f; // 플레이어 10랩마다 성장 체력
    [SerializeField] float HPbonus = 1.0f; // 플
    public float HPmax; // 플례이어 최대체력
    public float HPpreviousmax;
    //speed
    public float speedAdd = 5.0f;
    public float speedMulti = 1.0f;
    public float speedFin;
    //attackdamage
    public float attackDamageByLevel = 5.0f; //플레이어 자체 공격력

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
    void LevelUp()
    {
        player_level++;
        UpdateStatsByLevel();
    }

    void InitializeStats()
    {
        // 플레이어 초기 스탯 설정
        UpdateStatsByLevel();
    }
    void UpdateStatsByLevel()
    {
        HPmax = HPbylevel * HPbonus;
        HPpreviousmax = HPmax;
        switch (player_level)
        {
            case 1:
                HPmax = HPbylevel * HPbonus;
                HPpreviousmax = HPmax;
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
                Luck += 0.05f;
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
                WorkSpeed += 0.05f;
                Luck += 0.05f;
                speedMulti += 0.1f;
                HPbonus += 0.1f;
                break;
            case 11:
                attackDamageByLevel++;
                break;
            // 나머지 레벨에 따른 스탯 증가(자동사냥 '해줘')

            default:
                // 최고 레벨을 넘어가는 경우 처리
                break;
        }
        HPmax = HPbylevel * HPbonus;
        HPcurrent += HPmax - HPpreviousmax;
        speedFin = speedAdd * speedMulti;

        // 현재 체력 증가량만큼 추가
    }

    public bool CheckCritical()
    {
        float randomValue = Random.Range(0f, 1f); // 0에서 1 사이의 랜덤 값을 생성
        return randomValue < CriticalChance;
    }
}