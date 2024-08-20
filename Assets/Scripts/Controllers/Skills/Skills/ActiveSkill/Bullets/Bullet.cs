using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Player_Stat PlayerStat;
    public float bulletDamage; // 총알 데미지
    public bool canPenetrate = false;   // 총알이 관통하는지 여부를 결정
    public bool isBazukapo = false;    // 바주카포인지 여부 판단
    public float explosionRadius = 1.0f;    // 폭발 반경 ( == 피해 범위 ) - 변화

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            if (isBazukapo)
            {
                Monster monster = other.GetComponent<Monster>();
                if (monster != null)
                {
                    monster.TakeDamage(bulletDamage);
                }
            }
            else
            {
                Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                foreach (Collider2D hitMonster in hitMonsters)
                {
                    if (hitMonster.CompareTag("Monster"))
                    {
                        Monster monster = hitMonster.GetComponent<Monster>(); ;
                        if (monster != null)
                        {
                            monster.TakeDamage(bulletDamage);
                        }
                    }
                }
            }

            // 총알 제거
            if (!canPenetrate)              // 총알이 관통하는 여부에 따라 총알 제거하도록 수정
            {
                Destroy(gameObject);
            }
        }
    }
}
