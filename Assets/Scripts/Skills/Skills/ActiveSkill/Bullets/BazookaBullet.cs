using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaBullet : Bullet
{
    public float explosionRadius;
    public DamageInfo damageInfo;
    // Animator 컴포넌트를 참조
    public GameObject explosionPrefab;  // 폭발 이펙트 프리팹

    void Awake()
    {
        canPenetrate = false;

        // 총알의 Collider2D가 트리거로 설정되어 있는지 확인
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 몬스터와 충돌 시
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            // 생성된 폭발 프리팹의 크기를 explosionRadius에 맞춰 조정
            explosionPrefab.transform.localScale = new Vector3(explosionRadius, explosionRadius, 0);
            // 폭발 이펙트 프리팹을 몬스터와 충돌한 위치에 생성
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Debug.Log("explosionRadius : " + explosionRadius);
            // 1초 후에 폭발 이펙트 프리팹 삭제
            Destroy(explosion, 0.5f);
            // 폭발 데미지 처리
            ExplodeAndDamage(transform.position, explosionRadius, damageInfo);

            Destroy(gameObject);
        }
    }


    // 폭발 이펙트를 생성하는 메서드
    void ExplodeAndDamage(Vector3 explosionCenter, float radius, DamageInfo damageInfo)
{
    int monsterLayerMask = LayerMask.GetMask("Monster"); // "MonsterLayer"는 몬스터가 속한 레이어 이름

    Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(explosionCenter, radius, monsterLayerMask);

    foreach (Collider2D hitMonster in hitMonsters)
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


    protected float finalDamage(DamageInfo damageInfo)
    {
        float basicDamage = (damageInfo.playerDamage + damageInfo.skillDamage) * damageInfo.weaknessMultipler;
        if (damageInfo.isCritical)
            return basicDamage * 1.5f;
        else
            return basicDamage;
    }
}
