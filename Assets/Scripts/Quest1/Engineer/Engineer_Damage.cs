using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 라이브러리 추가

public class Engineer_Damage : MonoBehaviour
{
    public float Engineer_HP = 100; // 엔지니어 시작 체력 설정
    public float maxHP = 100f;       // 최대 체력

    public void TakeDamage(float damage) // 플레이어가 받는 데미지 로직
    {
        Engineer_HP -= damage;
        if (Engineer_HP <= 0)
        {
            Die();
        }

    }

    public void Die() //엔지니어 사망시 게임 오버
    {
        Destroy(this.gameObject);
        // 게임 마지막 화면으로 가도록!
        QuestManager.instance.currentQuest = 3;
        QuestManager.instance.CompleteQuest();
    }
}
