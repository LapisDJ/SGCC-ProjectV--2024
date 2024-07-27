using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Player_Stat playerStat;
    [SerializeField]
    GameObject HPBar;

    [SerializeField]
    GameObject Bullet;
    float BulletSpeed = 10.0f;

    [SerializeField]
    float shootCooldown = 1.0f; // 총알 발사 쿨타임
    private float lastShootTime;

    Rigidbody2D rb;

    [SerializeField]
    float meleeAttackCooldown = 1.0f; // 근접 공격 쿨타임
    private float lastMeleeAttackTime;

    void Start()
    {
        playerStat = GetComponent<Player_Stat>();
        rb = GetComponent<Rigidbody2D>();

        if (playerStat == null)
        {
            Debug.LogError("PlayerStat is missing on the Player.");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the Player.");
        }

        if (HPBar == null)
        {
            Debug.LogError("HPBar is not assigned.");
        }
        else
        {
            Vector3 HPBarPosition = transform.position;
            HPBarPosition.y += 1.0f;
            HPBar.transform.position = HPBarPosition;
        }

        lastShootTime = -shootCooldown; // 게임 시작 시 즉시 발사 가능하도록 설정
        lastMeleeAttackTime = -meleeAttackCooldown; // 게임 시작 시 즉시 근접 공격 가능하도록 설정
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.X) && Time.time >= lastShootTime + shootCooldown)
        {
            ShootBullet();
            lastShootTime = Time.time; // 마지막 발사 시간 업데이트
        }

        if (Input.GetKey(KeyCode.Z) && Time.time >= lastMeleeAttackTime + meleeAttackCooldown)
        {
            MeleeAttack();
            lastMeleeAttackTime = Time.time; // 마지막 근접 공격 시간 업데이트
        }
    }

    void FixedUpdate()
    {
        // if (Input.GetKey(KeyCode.UpArrow))
        // {
        //     rb.MovePosition(transform.position + new Vector3(0.0f, 1.0f, 0.0f) * Time.deltaTime * playerStat.getPlayerSpeed());
        // }

        // if (Input.GetKey(KeyCode.DownArrow))
        // {
        //     rb.MovePosition(transform.position - new Vector3(0.0f, 1.0f, 0.0f) * Time.deltaTime * playerStat.getPlayerSpeed());
        // }
        // if (Input.GetKey(KeyCode.LeftArrow))
        // {
        //     rb.MovePosition(transform.position - new Vector3(1.0f, 0.0f, 0.0f) * Time.deltaTime * playerStat.getPlayerSpeed());
        // }
        // if (Input.GetKey(KeyCode.RightArrow))
        // {
        //     rb.MovePosition(transform.position + new Vector3(1.0f, 0.0f, 0.0f) * Time.deltaTime * playerStat.getPlayerSpeed());
        // }
        rb.velocity = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized * 5.0f;

        if (HPBar != null)
        {
            Vector3 HPBarPosition = transform.position;
            HPBarPosition.y += 1.0f;
            HPBar.transform.position = HPBarPosition;
        }
    }

    void ShootBullet()
    {
        GameObject nearestMonster = FindNearestMonster();
        if (nearestMonster != null)
        {
            Vector3 direction = (nearestMonster.transform.position - transform.position).normalized;
            Vector3 bulletSpawnPosition = transform.position + direction * 0.5f; // 플레이어 위치에서 약간 떨어진 위치
            GameObject bullet = Instantiate(Bullet, bulletSpawnPosition, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * BulletSpeed;
            }

            // 플레이어와 총알 간의 충돌을 무시
            Collider2D playerCollider = GetComponent<Collider2D>();
            Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
            if (playerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCollider);
            }
        }
    }

    void MeleeAttack()
    {
        Vector3 playerPosition = transform.position;
        Vector3 leftAttackPosition = playerPosition + new Vector3(-5.0f, 0.0f, 0.0f); // 플레이어의 왼쪽 1칸
        Vector3 rightAttackPosition = playerPosition + new Vector3(5.0f, 0.0f, 0.0f); // 플레이어의 오른쪽 1칸

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(leftAttackPosition, 0.5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster"))
            {
                Monster monster = hitCollider.GetComponent<Monster>();
                if (monster != null)
                {
                    //monster.TakeDamage(playerStat.AttackDamage); // 플레이어의 공격력만큼 피해를 줌
                }
            }
        }

        hitColliders = Physics2D.OverlapCircleAll(rightAttackPosition, 0.5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster"))
            {
                Monster monster = hitCollider.GetComponent<Monster>();
                if (monster != null)
                {
                    //monster.TakeDamage(playerStat.AttackDamage); // 플레이어의 공격력만큼 피해를 줌
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(-1.0f, 0.0f, 0.0f), 0.5f); // 왼쪽 공격 범위 표시
        Gizmos.DrawWireSphere(transform.position + new Vector3(1.0f, 0.0f, 0.0f), 0.5f); // 오른쪽 공격 범위 표시
    }
}
