using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Stat : MonoBehaviour
{
    [SerializeField] int player_level = 1;
    [SerializeField] float expgainrate = 1.0f;
    [SerializeField] float WorkSpeed = 1.0f;
    [SerializeField] float Luck = 0.0f;
    [SerializeField] float CriticalChance = 0.0f;
    [SerializeField] class HP
    {
        public float current;
        public float bylevel = 50.0f;
        public float bonus = 1.0f; 
        public float max;
        public float previousmax;
    }
    [SerializeField] class Speed
    {
        public float add = 5.0f;
        public float multi = 1.0f;
        public float fin;
    }
    [SerializeField] class AttackDamage
    {
        public float bylevel = 5.0f;
        public float additional;
    }
    [SerializeField] HP hp;
    [SerializeField] Speed speed;
    [SerializeField] AttackDamage ad;
    void Start()
    {
        hp = GetComponent<HP>();
        speed = GetComponent<Speed>();
        ad = GetComponent<AttackDamage>();
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
        hp.max = hp.bylevel * hp.bonus;
        hp.previousmax = hp.max;
        switch(player_level)
        {
            case 1:
                hp.max = hp.bylevel * hp.bonus;
                hp.previousmax = hp.max;
                hp.current = hp.max;
                ad.bylevel = 5.0f;
                ad.additional = 0.0f;
                break;
            case 2:
                ad.bylevel++;
                break;
            case 3:
                ad.bylevel++;
                break;
            case 4:
                ad.bylevel++;
                break;
            case 5:
                ad.bylevel++;
                Luck += 0.05f;
                break;
            case 6:
                ad.bylevel++;
                break;
            case 7:
                ad.bylevel++;
                break;
            case 8:
                ad.bylevel++;
                break;
            case 9:
                ad.bylevel++;
                break;
            case 10:
                ad.bylevel+=3;
                WorkSpeed += 0.05f;
                Luck += 0.05f;
                speed.multi += 0.1f;
                hp.bonus += 0.1f;
                break;
            case 11:
                ad.bylevel++;
                break;
            // 나머지 레벨에 따른 스탯 증가(자동사냥 '해줘')

            default:
                // 최고 레벨을 넘어가는 경우 처리
                break;
        }
        hp.max = hp.bylevel * hp.bonus;
        hp.current += hp.max - hp.previousmax;
        speed.fin = speed.add * speed.multi;
        
        // 현재 체력 증가량만큼 추가
    }
    public float GetCurrentHP()
    {
        return this.hp.current;
    }
    public float GetMaxHP()
    {
        return this.hp.max;
    }
    public float GetAD()
    {
        return this.ad.additional + this.ad.bylevel;
    }
    public float GetSpeed()
    {
        return this.speed.fin;
    }
    public void Getdamage(float damage)
    {
        this.hp.current -= damage;
    }
}

