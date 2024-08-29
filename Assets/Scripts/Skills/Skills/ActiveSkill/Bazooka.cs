using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazooka : Skill
{
    WeaknessType weaknessType = WeaknessType.Blow; // 공격 타임 : 타격
    [SerializeField] public GameObject bazookaPrefab; // 바주카포 프리펩
    public float bulletSpeed = 10.0f;
    public float exploreRadius = 1f;

    protected override void Awake()
    {
        base.Awake();
        skillName = "바주카포";
        skillDamage = 2f;
        cooldown = 5f;
        icon = Resources.Load<Sprite>("UI/Icon/20");
    }
    public override void LevelUp() // 바주카포 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch (level)
        {
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
            
            Vector3 direction = (nearestMonster.transform.position - player.transform.position).normalized;
            Vector3 bulletSpawnPosition = player.transform.position + direction * 0.5f; // 플레이어 위치에서 약간 떨어진 위치
            GameObject bazookaBullet = Instantiate(bazookaPrefab, bulletSpawnPosition, Quaternion.identity);
            Debug.Log("Bazooka bullet 프리펩 생성 완료");

            BazookaBullet BazookaBulletScript = bazookaBullet.GetComponent<BazookaBullet>();

            // 플레이어와 총알 간의 충돌을 무시
            Collider2D playerCollider = GetComponent<CircleCollider2D>();
            Collider2D bulletCollider = bazookaBullet.GetComponent<CapsuleCollider2D>();
            if (playerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCollider);
            }

            if (BazookaBulletScript != null)
            {
                DamageInfo damageInfo = new DamageInfo
                {
                    skillDamage = this.skillDamage,
                    playerDamage = player.playerStat.attackDamageByLevel,
                    isCritical = player.playerStat.CheckCritical()
                };

                BazookaBulletScript.damageInfo = damageInfo;
            }

            Rigidbody2D bulletRb = bazookaBullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * bulletSpeed;
            }
        }
    }

    GameObject FindNearestMonster()
    {
        // 여러 태그를 가진 몬스터들을 찾기
        GameObject[] monsters1 = GameObject.FindGameObjectsWithTag("Monster1");
        GameObject[] monsters2 = GameObject.FindGameObjectsWithTag("Monster2");
        GameObject[] monsters3 = GameObject.FindGameObjectsWithTag("Monster3");

        // 하나의 리스트에 모든 몬스터들을 합침
        List<GameObject> allMonsters = new List<GameObject>();
        allMonsters.AddRange(monsters1);
        allMonsters.AddRange(monsters2);
        allMonsters.AddRange(monsters3);

        GameObject nearestMonster = null;
        float minDistance = Mathf.Infinity;

        // 모든 몬스터들 중에서 가장 가까운 몬스터를 찾음
        foreach (GameObject monster in allMonsters)
        {
            float distance = Vector3.Distance(transform.position, monster.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestMonster = monster;
            }
        }

        return nearestMonster;
    }

}
