using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ShotGun : Skill
{
    WeaknessType weaknessType = WeaknessType.Blow; // ���� Ÿ�� : Ÿ��
    protected override void Awake()
    {
        base.Awake();
        skillName = "ShotGun";
        skillDamage = 1f;
        cooldown = 5f;
    }
    public GameObject shotGunPrefab; // ���� ������
    public Player_Controller playerControl;
    public Vector3 finalDir;
    public float bulletSpeed = 10.0f;
    int numOfBullet = 0;
    public bool canPenetrate = true;   // �Ѿ��� �����ϴ��� ���θ� ����


    
    public override void LevelUp() // ���� ������ ����
    {
        base.LevelUp(); // ��ų ������
        this.skillDamage += 1f;

        switch (level)
        {
            case 1: // 0 -> 1�� : �⺻������ ���� �߻� ����
                numOfBullet = 3;
                break;
            case 3: // 2->3�� : ��Ÿ�� 1�� ����
                cooldown -= 1f;
                break;
            case 5: // 4->5�� : ���� 4������ �ٲ�
                numOfBullet++;
                break;
            case 7: // 6->7�� : ��Ÿ�� 1�� ����
                cooldown -= 1f;
                break;
            case 8: // 7->8�� : ���� 5������ �ٲ�
                numOfBullet++;
                break;
        }
    }

    public override void Activate(GameObject target) // ���Ϳ� ��ȣ �ۿ� ����
    {
        switch (numOfBullet)
        {
            case 0:
                break;
            case 3: // ���� 3��
                shootGun_3();
                break;
            case 4: // ���� 4��
                shootGun_4();
                break;
            case 5: // ���� 5��
                shootGun_5();
                break;
        }
    }




    void shootGun_3() // ���� 3��
    {
        Vector3 baseDirection = finalDir;  // �⺻ �߻� ������ ������ �ٶ� �������� ���� ( ������ ���ۺ��� ���� �����ϱ� �����̴ٰ� ȹ���ߴٰ� �����Ͽ� �ۼ� )
        for (int i = -1; i <= 1; i++)
        {
            // i���� ���� ������ �����մϴ�. -1, 0, 1�� ���� -30��, 0��, 30��
            float angle = 30.0f * i;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * baseDirection;

            // �Ѿ� ���� ��ġ�� �÷��̾� �������� ����
            Vector3 bulletSpawnPosition = transform.position + direction * 0.5f; // �÷��̾� ��ġ���� �ణ ������ ��ġ

            // �Ѿ��� �����ϰ� �߻�
            GameObject bullet = Instantiate(shotGunPrefab, bulletSpawnPosition, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * bulletSpeed;
            }
            // �Ѿ��� Sprite Renderer ����
            SpriteRenderer bulletSr = bullet.GetComponent<SpriteRenderer>();
            if (bulletSr != null)
            {
                bulletSr.sortingLayerName = "Player"; // �Ѿ��� sorting layer�� �÷��̾�� ���� ������ ����
                bulletSr.sortingOrder = 1; // �÷��̾�� �տ� ���̵��� ����
            }
            // �÷��̾�� �Ѿ� ���� �浹�� ����
            Collider2D playerCollider = GetComponent<Collider2D>();
            Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
            if (playerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCollider);
            }

            // �Ѿ��� canPenetrate ������ true�� �����Ͽ� ���� ���� ����
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.canPenetrate = true;
                bulletScript.damage = this.skillDamage;

                // ���Ϳ� �浹 �� �������� ������ �̺�Ʈ ����
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

    void shootGun_4() // ���� 4��
        {
            Vector3 baseDirection = finalDir;  // �⺻ �߻� ������ ������ �ٶ� �������� ����
            for (int i = 0; i < 4; i++)
            {
                // 4���� �Ѿ��� -45��, -15��, 15��, 45�� �������� �߻�
                float angle = -45.0f + 30.0f * i; // 30���� ���� ���� ������ �߻�
                Vector3 direction = Quaternion.Euler(0, 0, angle) * baseDirection;

                // �Ѿ� ���� ��ġ�� �÷��̾� �������� ����
                Vector3 bulletSpawnPosition = transform.position + direction * 0.5f; // �÷��̾� ��ġ���� �ణ ������ ��ġ

                // �Ѿ��� �����ϰ� �߻�
                GameObject bullet = Instantiate(shotGunPrefab, bulletSpawnPosition, Quaternion.identity);
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.velocity = direction * bulletSpeed;
                }
                // �Ѿ��� Sprite Renderer ����
                SpriteRenderer bulletSr = bullet.GetComponent<SpriteRenderer>();
                if (bulletSr != null)
                {
                    bulletSr.sortingLayerName = "Player"; // �Ѿ��� sorting layer�� �÷��̾�� ���� ������ ����
                    bulletSr.sortingOrder = 1; // �÷��̾�� �տ� ���̵��� ����
                }
                // �÷��̾�� �Ѿ� ���� �浹�� ����
                Collider2D playerCollider = GetComponent<Collider2D>();
                Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
                if (playerCollider != null && bulletCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, bulletCollider);
                }

                // �Ѿ��� canPenetrate ������ true�� �����Ͽ� ���� ���� ����
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.canPenetrate = true;
                    bulletScript.damage = this.skillDamage;

                    // ���Ϳ� �浹 �� �������� ������ �̺�Ʈ ����
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


    void shootGun_5() // ���� 5��
            {
                Vector3 baseDirection = finalDir;  // �⺻ �߻� ������ ������ �ٶ� �������� ����
                float angleStep = 24.0f;  // �� �Ѿ� ���� ���� ����
                float initialAngle = -48.0f;  // ù ��° �Ѿ��� �߻�Ǵ� ����

                // ������ �������� 5���� �Ѿ��� �߻�
                for (int i = 0; i < 5; i++)
                {
                    // �� �Ѿ��� ������ ���
                    float angle = initialAngle + angleStep * i;  // -48������ �����ؼ� 24�� �������� �߻�
                    Vector3 direction = Quaternion.Euler(0, 0, angle) * baseDirection;

                    // �Ѿ� ���� ��ġ�� �÷��̾� �������� ����
                    Vector3 bulletSpawnPosition = transform.position + direction * 0.5f; // �÷��̾� ��ġ���� �ణ ������ ��ġ

                    // �Ѿ��� �����ϰ� �߻�
                    GameObject bullet = Instantiate(shotGunPrefab, bulletSpawnPosition, Quaternion.identity);
                    Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                    if (bulletRb != null)
                    {
                        bulletRb.velocity = direction * bulletSpeed;
                    }
                    // �Ѿ��� Sprite Renderer ����
                    SpriteRenderer bulletSr = bullet.GetComponent<SpriteRenderer>();
                    if (bulletSr != null)
                    {
                        bulletSr.sortingLayerName = "Player"; // �Ѿ��� sorting layer�� �÷��̾�� ���� ������ ����
                        bulletSr.sortingOrder = 1; // �÷��̾�� �տ� ���̵��� ����
                    }
                    // �÷��̾�� �Ѿ� ���� �浹�� ����
                    Collider2D playerCollider = GetComponent<Collider2D>();
                    Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
                    if (playerCollider != null && bulletCollider != null)
                    {
                        Physics2D.IgnoreCollision(playerCollider, bulletCollider);
                    }

                    // �Ѿ��� canPenetrate ������ true�� �����Ͽ� ���� ���� ����
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    if (bulletScript != null)
                    {
                        bulletScript.canPenetrate = true;
                        bulletScript.damage = this.skillDamage;

                        // ���Ϳ� �浹 �� �������� ������ �̺�Ʈ ����
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
