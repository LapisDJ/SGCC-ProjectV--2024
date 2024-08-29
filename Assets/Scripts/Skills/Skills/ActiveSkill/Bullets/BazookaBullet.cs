using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaBullet : Bullet
{
    public float explosionRadius;
    public DamageInfo damageInfo;

    void Awake()
    {
        canPenetrate = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster1")||collision.CompareTag("Monster2")||collision.CompareTag("Monster3"))
        {
            Destroy(gameObject); // 총알 제거
            ExplodeAndDamage(collision.transform.position, explosionRadius, damageInfo);
        }
    }

    void ExplodeAndDamage(Vector3 explosionCenter, float radius, DamageInfo damageInfo)
    {
        Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(explosionCenter, radius);

        foreach (Collider2D hitMonster in hitMonsters)
        {
            if (hitMonster.CompareTag("Monster1")||hitMonster.CompareTag("Monster2")||hitMonster.CompareTag("Monster3"))
            {
                Monster monster = hitMonster.GetComponent<Monster>();
                if (monster != null)
                {
                    float totalDamage;
                    float weaknessMultiplier = (hitMonster.CompareTag("Monster1") || hitMonster.CompareTag("Monster3")) ? 1.5f : 1f;

                    damageInfo.weaknessMultipler = weaknessMultiplier;

                    totalDamage = finalDamage(damageInfo);
                    monster.TakeDamage(totalDamage);
                }
            }
        }
    }

    protected float finalDamage(DamageInfo damageInfo)
    {
        float basicDamage = (damageInfo.playerDamage + damageInfo.skillDamage) * damageInfo.weaknessMultipler;
        if (damageInfo.isCritical)
            return basicDamage * 1.5f;
        else
            return basicDamage;
    }
}
