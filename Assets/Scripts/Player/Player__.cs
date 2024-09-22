using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player__ : MonoBehaviour
{

    public PlayerController playerController;

    void Awake() // 초기화: 스탯, 컨트롤러 불러오기
    {
        playerController = GetComponent<PlayerController>();
    }


    public void TakeDamage(float damage) // 플레이어가 받는 데미지 로직
    {
        if (Player_Stat.instance != null)
        {
            if(Player_Stat.instance.isinvincible)
            {
                damage = 0;
            }
            Player_Stat.instance.HPcurrent -= damage;
            if (Player_Stat.instance.HPcurrent <= 0)
            {
                Die();
            }
        }
    }

    public void Die() // 플레이어 사망 게임 오버
    {
        QuestManager.instance.currentQuest = 3;
        QuestManager.instance.CompleteQuest();
    }
}