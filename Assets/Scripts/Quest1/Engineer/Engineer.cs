using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : Player
{
    

    void Awake() // 초기화: 스탯, 컨트롤러 불러오기
    {
        playerStat = GetComponent<Player_Stat>(); // 부모 클래스의 stat 참조
        playerController = GetComponent<Player_Controller>(); // 부모 클래스의 controller 참조
    }

    
    /*
    public void TakeDamage(float damage)
    {
        base.TakeDamage(damage); // 부모 클래스의 TakeDamage 메서드를 호출

        if (playerStat.HPcurrent <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("엔지니어가 죽었습니다.");
        // 퀘스트 실패 로직 추가
        Destroy(gameObject);
    }
    */
}