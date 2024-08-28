using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Player_Stat playerStat;
    public PlayerController playerController;

    void Awake() // 초기화: 스탯, 컨트롤러 불러오기
    {
        playerStat = GetComponent<Player_Stat>();
        playerController = GetComponent<PlayerController>();
    }


    public void TakeDamage(float damage) // 플레이어가 받는 데미지 로직
    {
        if (playerStat != null)
        {
            playerStat.HPcurrent -= damage;
            if (playerStat.HPcurrent <= 0)
            {
                Die();
            }
        }
    }

    public void Die() // 플레이어 사망 게임 오버
    {
        Destroy(this);
    }
}
