using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGunBullet : Bullet
{
    public float range = 5f; // 샷건의 사거리
    private Vector3 spawnPosition;
    public DamageInfo damageInfo;

    void Awake()
    {
        canPenetrate = true;
        spawnPosition = transform.position; // 총알이 생성된 위치를 기록
    }

    void Update()
    {
        // 현재 위치와 스폰 위치 사이의 거리를 계산
        float distanceTravelled = Vector3.Distance(spawnPosition, transform.position);

        // 사거리를 초과하면 총알 파괴
        if (distanceTravelled > range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster1") || collision.CompareTag("Monster2") || collision.CompareTag("Monster3"))
        {
            Monster monster = collision.GetComponent<Monster>();
            if (monster != null)
            {
                float totalDamage;
                float weaknessMultiplier = (monster.CompareTag("Monster1") || monster.CompareTag("Monster3")) ? 1.5f : 1f;

                damageInfo.weaknessMultipler = weaknessMultiplier;

                totalDamage = finalDamage(damageInfo);
                monster.TakeDamage(totalDamage);
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
