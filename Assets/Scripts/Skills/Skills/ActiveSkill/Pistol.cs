using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Pistol : Skill
{
    WeaknessType weaknessType = WeaknessType.Blow; // ���� Ÿ�� : Ÿ��
    protected override void Awake()
    {
        base.Awake();
        skillName = "Pistol";
        skillDamage = 1f;
        cooldown = 3f;
    }
    public GameObject pistolPrefab; // ���� ������
    public float bulletSpeed = 10.0f;
    public bool canPenetrate = false;   // �Ѿ��� �����ϴ��� ���θ� ����


    public override void Activate() // ���Ϳ� ��ȣ �ۿ� ����
    {
        GameObject nearestMonster = FindNearestMonster();
        if (nearestMonster != null)
        {
            Vector3 direction = (nearestMonster.transform.position - transform.position).normalized;
            Vector3 bulletSpawnPosition = transform.position + direction * 0.5f; // �÷��̾� ��ġ���� �ణ ������ ��ġ
            GameObject bullet = Instantiate(pistolPrefab, bulletSpawnPosition, Quaternion.identity);

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

            // ���Ϳ��� �������� �ִ� ����
            Bullet bulletScript = bullet.AddComponent<Bullet>();
            bulletScript.damage = this.skillDamage;
            bulletScript.canPenetrate = this.canPenetrate;

            // �Ѿ��� ���Ϳ� �浹�ϸ� �Ѿ��� �����ϴ� ����
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

        // ������ 8 �̻��� ��� 2ȸ ����
        if (level >= 8)
        {
            StartCoroutine(DoubleAttack());
        }
    }

    public override void LevelUp() // ���� ������ ����
    {
        base.LevelUp(); // ��ų ������
        this.skillDamage += 1f;

        switch (level)
        {
            case 3: // 2->3�� : ��Ÿ�� 1�� ����
                this.cooldown -= 1f;
                break;
            case 5: // 4->5�� : ��Ÿ�� 1�� ����
                this.cooldown -= 1f;
                break;
            case 7: // 6->7�� : ������ 2 ����
                this.skillDamage += 2f;
                break;
        }
    }

    private IEnumerator DoubleAttack() // 8���� 2ȸ ������ �ڷ�ƾ���� ����
    {
        yield return new WaitForSeconds(0.25f); // 0.25�� ���
        Activate(); // �� ��° ����
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