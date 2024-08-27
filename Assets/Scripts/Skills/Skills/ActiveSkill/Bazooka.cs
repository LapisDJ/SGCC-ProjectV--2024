using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazooka : Skill
{
    WeaknessType weaknessType = WeaknessType.Blow; // 공격 타임 : 타격
    public GameObject bazookaPrefab; // 바주카포 프리펩
    public float bulletSpeed = 10.0f;
    private bool isBazookaAutoFire = false; // 바주카포 On Off로 구현
    public float exploreRadius = 1f;
    public bool canPenetrate = false;   // 총알이 관통하는지 여부를 결정

    protected override void Awake()
    {
        base.Awake();
        skillName = "Bazooka";
        skillDamage = 2f;
        cooldown = 5f;
    }
    public override void LevelUp() // 바주카포 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch (level)
        {
            case 1: // 0 -> 1랩 : 기본값으로 바주카포 발사 가능
                isBazookaAutoFire = !isBazookaAutoFire;
                break;
            case 3: // 2->3랩 : 쿨타임 1초 감소
                cooldown -= 1f;
                break;
            case 4: // 3->4랩 : 폭발 범위 반지름 증가(반지름 1→ 반지름 1.5)
                exploreRadius += 0.5f;
                break;
            case 5: // 4->5랩 : 쿨타임 1초 감소
                cooldown -= 1f;
                break;
            case 6: // 5->6랩 : 폭발 범위 반지름 증가 (1.5→2)
                exploreRadius += 0.5f;
                break;
            case 7: // 6->7랩 : 쿨타임 1초 감소
                cooldown -= 1f;
                break;
            case 8: // 7->8랩 : 폭발 범위 반지름 (2→3)
                exploreRadius += 1f;
                break;
        }
    }


   public override void Activate() // 몬스터와 상호 작용 로직
{
    GameObject nearestMonster = FindNearestMonster();
        if (nearestMonster != null)
        {
            Vector3 direction = (nearestMonster.transform.position - transform.position).normalized;
            Vector3 bulletSpawnPosition = transform.position + direction * 0.5f; // 플레이어 위치에서 약간 떨어진 위치
            GameObject bullet = Instantiate(bazookaPrefab, bulletSpawnPosition, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * bulletSpeed;
            }
            // 총알의 Sprite Renderer 설정
            SpriteRenderer bulletSr = bullet.GetComponent<SpriteRenderer>();
            if (bulletSr != null)
            {
                bulletSr.sortingLayerName = "Player"; // 총알의 sorting layer를 플레이어와 같은 것으로 설정
                bulletSr.sortingOrder = 1; // 플레이어보다 앞에 보이도록 설정
            }
            // 플레이어와 총알 간의 충돌을 무시
            Collider2D playerCollider = GetComponent<Collider2D>();
            Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
            if (playerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCollider);
            }

            // 총알의 폭발 스크립트 설정
            Bullet explodeScript = bullet.GetComponent<Bullet>();
            if (explodeScript != null)
            {
                explodeScript.isBazukapo = true;
                explodeScript.explosionRadius = exploreRadius; // 폭발 반경 설정
                explodeScript.damage = this.skillDamage;

                // 몬스터에 충돌 시 폭발 및 데미지 처리
                explodeScript.OnHitMonster += (hitMonster) =>
                {
                    ExplodeAndDamage(hitMonster.transform.position, exploreRadius);
                    Destroy(bullet); // 총알 제거
                };
            }

        }
    }
    void ExplodeAndDamage(Vector3 explosionCenter, float radius)
    {
        Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(explosionCenter, radius);

        foreach (Collider2D hitMonster in hitMonsters)
        {
            if (hitMonster.CompareTag("Monster"))
            {
                Monster monster = hitMonster.GetComponent<Monster>();
                if (monster != null)
                {
                    float weaknessMultiplier = (hitMonster.CompareTag("Monster1") || hitMonster.CompareTag("Monster3")) ? 1.5f : 1f;

                    DamageInfo damageInfo = new DamageInfo
                    {
                        skillDamage = this.skillDamage,
                        playerDamage = player.playerStat.attackDamageByLevel,
                        weaknessMultipler = weaknessMultiplier,
                        isCritical = player.playerStat.CheckCritical()
                    };

                    float totalDamage = finalDamage(damageInfo);
                    monster.TakeDamage(totalDamage);
                }
            }
        }
    }


    GameObject FindNearestMonster()
    {
        GameObject[] Monsters = GameObject.FindGameObjectsWithTag("Monster");
        GameObject NearestMonster = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject monster in Monsters)
        {
            float distance = Vector3.Distance(transform.position, monster.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                NearestMonster = monster;
            }
        }

        return NearestMonster;
    }

}
