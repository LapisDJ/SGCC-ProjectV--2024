using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazooka : Skill
{
    WeaknessType weaknessType = WeaknessType.Blow; // ���� Ÿ�� : Ÿ��
    public GameObject bazookaPrefab; // ����ī�� ������
    public float bulletSpeed = 10.0f;
    private bool isBazookaAutoFire = false; // ����ī�� On Off�� ����
    public float exploreRadius = 1f;
    public bool canPenetrate = false;   // �Ѿ��� �����ϴ��� ���θ� ����

    protected override void Awake()
    {
        base.Awake();
        skillName = "Bazooka";
        skillDamage = 2f;
        cooldown = 5f;
    }
    public override void LevelUp() // ����ī�� ������ ����
    {
        base.LevelUp(); // ��ų ������
        this.skillDamage += 1f;

        switch (level)
        {
            case 1: // 0 -> 1�� : �⺻������ ����ī�� �߻� ����
                isBazookaAutoFire = !isBazookaAutoFire;
                break;
            case 3: // 2->3�� : ��Ÿ�� 1�� ����
                cooldown -= 1f;
                break;
            case 4: // 3->4�� : ���� ���� ������ ����(������ 1�� ������ 1.5)
                exploreRadius += 0.5f;
                break;
            case 5: // 4->5�� : ��Ÿ�� 1�� ����
                cooldown -= 1f;
                break;
            case 6: // 5->6�� : ���� ���� ������ ���� (1.5��2)
                exploreRadius += 0.5f;
                break;
            case 7: // 6->7�� : ��Ÿ�� 1�� ����
                cooldown -= 1f;
                break;
            case 8: // 7->8�� : ���� ���� ������ (2��3)
                exploreRadius += 1f;
                break;
        }
    }


    public override void Activate() // ���Ϳ� ��ȣ �ۿ� ����
    {
        GameObject nearestMonster = FindNearestMonster();
        if (nearestMonster != null)
        {
            Vector3 direction = (nearestMonster.transform.position - transform.position).normalized;
            Vector3 bulletSpawnPosition = transform.position + direction * 0.5f; // �÷��̾� ��ġ���� �ణ ������ ��ġ
            GameObject bullet = Instantiate(bazookaPrefab, bulletSpawnPosition, Quaternion.identity);
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

            // �Ѿ��� ���� ��ũ��Ʈ ����
            Bullet explodeScript = bullet.GetComponent<Bullet>();
            if (explodeScript != null)
            {
                explodeScript.isBazukapo = true;
                explodeScript.explosionRadius = exploreRadius; // ���� �ݰ� ����
                explodeScript.damage = this.skillDamage;

                // ���Ϳ� �浹 �� ���� �� ������ ó��
                explodeScript.OnHitMonster += (hitMonster) =>
                {
                    ExplodeAndDamage(hitMonster.transform.position, exploreRadius);
                    Destroy(bullet); // �Ѿ� ����
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
