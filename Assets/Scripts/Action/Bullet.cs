using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Player_Stat PlayerStat;
    public float bulletDamage;

    void Start ()
    {
        bulletDamage = PlayerStat.GetAD();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            // 몬스터 피격 판정 처리
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(bulletDamage);
            }

            // 총알 제거
            Destroy(gameObject);
        }
    }
}
