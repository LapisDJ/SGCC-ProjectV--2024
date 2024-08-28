using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : Player
{
    // 엔지니어 전용 체력 설정
    [SerializeField] private float engineerBaseHP = 100.0f; // 엔지니어 기본 HP


    public void TakeDamage(float damage)
    {
        Debug.Log($"엔지니어가 {damage} 데미지를 받았습니다. 남은 체력: {engineerBaseHP}");
    }

    public void Die()
    {
        Debug.Log("엔지니어가 사망했습니다.");
    }
}
 