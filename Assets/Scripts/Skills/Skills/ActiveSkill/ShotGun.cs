using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Skill
{
    WeaknessType weaknessType = WeaknessType.Blow;  // 공격 타입 : 타격

    protected override void Awake()
    {
        base.Awake();
        skillName = "샷건";
        skillDamage = 1f;
        cooldown = 5f;
        icon = Resources.Load<Sprite>("UI/Icon/10");
    }

    [SerializeField] public GameObject shotGunPrefab; // 샷건 프리펩
    private Vector2 dir; // 플레이어 이동 방향
    private Vector2 lastdir;
    public float bulletSpeed = 10.0f;
    public int numOfBullet = 0;
    public bool canPenetrate = true;   // 총알이 관통하는지 여부를 결정

    public override void Activate() // 샷건 발사 및 몬스터와의 상호 작용 로직
    {
        dir = player.GetComponent<Rigidbody2D>().velocity.normalized; // 이동 방향 단위 벡터
        if (dir == Vector2.zero) dir = lastdir;
        else lastdir = dir;

        Vector3 baseDirection = dir;  // 기본 발사 방향 설정: 플레이어 이동 방향
        float angleStep = 30.0f;  // 각 총알 간의 기본 각도 차이
        float initialAngle = -(numOfBullet - 1) * angleStep / 2;  // 첫 총알이 발사될 각도

        for (int i = 0; i < numOfBullet; i++)
        {
            float angle = initialAngle + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * baseDirection;

            // 총알 생성 위치를 플레이어 앞쪽으로 설정
            Vector3 bulletSpawnPosition = player.transform.position + direction * 0.5f;

            // 총알을 생성하고 발사
            GameObject ShotGunbullet = Instantiate(shotGunPrefab, bulletSpawnPosition, Quaternion.identity);
            ShotGunBullet ShotGunBulletScript = ShotGunbullet.GetComponent<ShotGunBullet>();

            // 총알의 설정
            if (ShotGunBulletScript != null)
            {
                ShotGunBulletScript.canPenetrate = canPenetrate;
                DamageInfo damageInfo = new DamageInfo
                {
                    skillDamage = this.skillDamage,
                    playerDamage = player.playerStat.attackDamageByLevel,
                    isCritical = player.playerStat.CheckCritical()
                };

                ShotGunBulletScript.damageInfo = damageInfo;
            }

            Rigidbody2D bulletRb = ShotGunbullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * bulletSpeed;
            }

             // 플레이어와 총알 간의 충돌을 무시
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D bulletCollider = ShotGunbullet.GetComponent<Collider2D>();
            if (playerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCollider);
            }
        }
    }

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
            case 5: // 4->5랩 : 샷건 4발
                numOfBullet = 4;
                break;
            case 7: // 6->7랩 : 쿨타임 1초 감소
                cooldown -= 1f;
                break;
            case 8: // 7->8랩 : 샷건 5발
                numOfBullet = 5;
                break;
        }
    }
}
