using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ShotGun : Skill
{
    WeaknessType weaknessType = WeaknessType.Blow; // 공격 타임 : 타격
    public ShotGun() : base("ShotGun", 1f, 5f) { } // 생성자 : 스킬명, 1랩 데미지, 1랩 쿨타임
    public GameObject shotGunPrefab; // 샷건 프리펩
    public Player_Controller playerControl;
    public Vector3 finalDir;
    public float bulletSpeed = 10.0f;
    int numOfBullet = 0;
    public bool canPenetrate = true;   // 총알이 관통하는지 여부를 결정


    
    public override void LevelUp() // 샷건 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch (level)
        {
            case 1: // 0 -> 1랩 : 기본값으로 샷건 발사 가능
                numOfBullet = 3;
                break;
            case 3: // 2->3랩 : 쿨타임 1초 감소
                cooldown -= 1f;
                break;
            case 5: // 4->5랩 : 샷건 4갈래로 바뀜
                numOfBullet++;
                break;
            case 7: // 6->7랩 : 쿨타임 1초 감소
                cooldown -= 1f;
                break;
            case 8: // 7->8랩 : 샷건 5갈래로 바뀜
                numOfBullet++;
                break;
        }
    }

    public override void Activate(GameObject target) // 몬스터와 상호 작용 로직
    {
        switch (numOfBullet)
        {
            case 0:
                break;
            case 3: // 샷건 3발
                shootGun_3();
                break;
            case 4: // 샷건 4발
                shootGun_4();
                break;
            case 5: // 샷건 5발
                shootGun_5();
                break;
        }
    }




    void shootGun_3() // 샷건 3발
    {
        Vector3 baseDirection = finalDir;  // 기본 발사 방향을 마지막 바라본 방향으로 설정 ( 샷건은 시작부터 있지 않으니까 움직이다가 획득했다고 가정하에 작성 )
        for (int i = -1; i <= 1; i++)
        {
            // i값에 따라 각도를 조정합니다. -1, 0, 1에 따라 -30도, 0도, 30도
            float angle = 30.0f * i;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * baseDirection;

            // 총알 생성 위치를 플레이어 앞쪽으로 설정
            Vector3 bulletSpawnPosition = transform.position + direction * 0.5f; // 플레이어 위치에서 약간 떨어진 위치

            // 총알을 생성하고 발사
            GameObject bullet = Instantiate(shotGunPrefab, bulletSpawnPosition, Quaternion.identity);
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

            // 총알의 canPenetrate 변수를 true로 설정하여 관통 가능 설정
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.canPenetrate = true;
                bulletScript.damage = this.skillDamage;

                // 몬스터와 충돌 시 데미지를 입히는 이벤트 구독
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
                };
            }
        }
    }

    void shootGun_4() // 샷건 4발
        {
            Vector3 baseDirection = finalDir;  // 기본 발사 방향을 마지막 바라본 방향으로 설정
            for (int i = 0; i < 4; i++)
            {
                // 4발의 총알을 -45도, -15도, 15도, 45도 방향으로 발사
                float angle = -45.0f + 30.0f * i; // 30도씩 차이 나는 각도로 발사
                Vector3 direction = Quaternion.Euler(0, 0, angle) * baseDirection;

                // 총알 생성 위치를 플레이어 앞쪽으로 설정
                Vector3 bulletSpawnPosition = transform.position + direction * 0.5f; // 플레이어 위치에서 약간 떨어진 위치

                // 총알을 생성하고 발사
                GameObject bullet = Instantiate(shotGunPrefab, bulletSpawnPosition, Quaternion.identity);
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

                // 총알의 canPenetrate 변수를 true로 설정하여 관통 가능 설정
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.canPenetrate = true;
                    bulletScript.damage = this.skillDamage;

                    // 몬스터와 충돌 시 데미지를 입히는 이벤트 구독
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
                    };
                }
            }
        }


    void shootGun_5() // 샷건 5발
            {
                Vector3 baseDirection = finalDir;  // 기본 발사 방향을 마지막 바라본 방향으로 설정
                float angleStep = 24.0f;  // 각 총알 간의 각도 차이
                float initialAngle = -48.0f;  // 첫 번째 총알이 발사되는 각도

                // 각도를 기준으로 5발의 총알을 발사
                for (int i = 0; i < 5; i++)
                {
                    // 각 총알의 방향을 계산
                    float angle = initialAngle + angleStep * i;  // -48도에서 시작해서 24도 간격으로 발사
                    Vector3 direction = Quaternion.Euler(0, 0, angle) * baseDirection;

                    // 총알 생성 위치를 플레이어 앞쪽으로 설정
                    Vector3 bulletSpawnPosition = transform.position + direction * 0.5f; // 플레이어 위치에서 약간 떨어진 위치

                    // 총알을 생성하고 발사
                    GameObject bullet = Instantiate(shotGunPrefab, bulletSpawnPosition, Quaternion.identity);
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

                    // 총알의 canPenetrate 변수를 true로 설정하여 관통 가능 설정
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    if (bulletScript != null)
                    {
                        bulletScript.canPenetrate = true;
                        bulletScript.damage = this.skillDamage;

                        // 몬스터와 충돌 시 데미지를 입히는 이벤트 구독
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
                        };
                    }
                }

            }
}
