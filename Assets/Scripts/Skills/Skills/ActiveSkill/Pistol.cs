using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Pistol : Skill
{
    WeaknessType weaknessType = WeaknessType.Blow; // 공격 타임 : 타격
    protected override void Awake()
    {
        base.Awake();
        skillName = "Pistol";
        skillDamage = 1f;
        cooldown = 3f;
    }
    public GameObject pistolPrefab;// 권총 프리펩
    public float bulletSpeed = 10.0f;
    public bool canPenetrate = false;    // 총알이 관통하는지 여부를 결정


    public override void Activate() // 몬스터와 상호 작용 로직
    {
        GameObject nearestMonster = FindNearestMonster();
        if (nearestMonster != null)
        {
            Vector3 direction = (nearestMonster.transform.position - transform.position).normalized;
            Vector3 bulletSpawnPosition = transform.position + direction * 0.5f;  // 플레이어 위치에서 약간 떨어진 위치
            GameObject bullet = Instantiate(pistolPrefab, bulletSpawnPosition, Quaternion.identity);

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

            // 몬스터에게 데미지를 주는 로직
            Bullet bulletScript = bullet.AddComponent<Bullet>();
            bulletScript.damage = this.skillDamage;
            bulletScript.canPenetrate = this.canPenetrate;

            // 총알이 몬스터와 충돌하면 총알을 제거하는 로직
            bulletScript.OnHitMonster += (hitMonster) =>
            {
                Monster monster = hitMonster.GetComponent<Monster>();
                if (monster != null)
                {
                    float weaknessMultipler = (hitMonster.CompareTag("Monster1") || hitMonster.CompareTag("Monster3")) ? 1.5f : 1f;

                    DamageInfo damageInfo = new DamageInfo
                    {
                        skillDamage = this.skillDamage,
                        playerDamage = player.playerStat.attackDamageByLevel,
                        weaknessMultipler = weaknessMultipler,
                        isCritical = player.playerStat.CheckCritical()
                    };

                    float totalDamage = finalDamage(damageInfo);
                    monster.TakeDamage(totalDamage);
                }

                if (!canPenetrate)
                {
                    Destroy(bullet);
                }
            };
        }

        // 레벨이 8 이상일 경우 2회 공격
        if (level >= 8)
        {
            StartCoroutine(DoubleAttack());
        }
    }

    public override void LevelUp() // 권총 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch (level)
        {
            case 3: // 2->3랩 : 쿨타임 1초 감소
                this.cooldown -= 1f;
                break;
            case 5: // 4->5랩 : 쿨타임 1초 감소
                this.cooldown -= 1f;
                break;
            case 7: // 6->7랩 : 데미지 2 증가
                this.skillDamage += 2f;
                break;
        }
    }

    private IEnumerator DoubleAttack() // 8레벨 2회 공격을 코루틴으로 구현
    {
        yield return new WaitForSeconds(0.25f); // 0.25초 대기
        Activate(); // 두 번째 공격
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