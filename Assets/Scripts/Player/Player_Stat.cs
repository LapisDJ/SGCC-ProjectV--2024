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
    public float Speedadd = 5.0f;
    public float Speedmulti = 1.0f;
    public float Speedfin;
    //attackdamage
    public float ADbylevel = 5.0f;
    public float ADadditional;

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
        switch(player_level)
        {
            case 1:
                HPmax = HPbylevel * HPbonus;
                HPpreviousmax = HPmax;
                HPcurrent = HPmax;
                ADbylevel = 5.0f;
                ADadditional = 0.0f;
                break;
            case 2:
                ADbylevel++;
                break;
            case 3:
                ADbylevel++;
                break;
            case 4:
                ADbylevel++;
                break;
            case 5:
                ADbylevel++;
                Luck += 0.05f;
                break;
            case 6:
                ADbylevel++;
                break;
            case 7:
                ADbylevel++;
                break;
            case 8:
                ADbylevel++;
                break;
            case 9:
                ADbylevel++;
                break;
            case 10:
                ADbylevel+=3;
                WorkSpeed += 0.05f;
                Luck += 0.05f;
                Speedmulti += 0.1f;
                HPbonus += 0.1f;
                break;
            case 11:
                ADbylevel++;
                break;
            // 나머지 레벨에 따른 스탯 증가(자동사냥 '해줘')

            default:
                // 최고 레벨을 넘어가는 경우 처리
                break;
        }
        HPmax = HPbylevel * HPbonus;
        HPcurrent += HPmax - HPpreviousmax;
        Speedfin = Speedadd * Speedmulti;
        
        // 현재 체력 증가량만큼 추가
    }
    public void Getdamage(float damage)
    {
        this.HPcurrent -= damage;
    }
}

