using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector2 inputVec;        // 방향키 좌표값을 입력받을 변수
    private Vector2 lastDirection;  // 플레이어가 마지막으로 바라본 방향 저장
    
    PlayerStat playerStat;
    [SerializeField]
    GameObject HPBar;
    [SerializeField]
    GameObject Bullet;
    float BulletSpeed = 10.0f;

    [SerializeField]
    GameObject Rocket;
    float RocketSpeed = 10.0f;

    [SerializeField]
    float shootCooldown = 3.0f; // 권총 발사 쿨타임
    float shootGunCooldown = 5.0f;  //샷건 발사 쿨타임
    float bazookaCooldown = 5.0f; // 바주카포 발사 쿨타임
    private float lastShootTime;
    private float lastBazookaTime;

    private bool isBazookaAutoFire = false; // 바주카포 On Off로 구현

    Rigidbody2D rb;

    [SerializeField]
    float meleeAttackCooldown = 1.0f; // 근접 공격 쿨타임
    private float lastMeleeAttackTime;

    void Start()
    {
        playerStat = GetComponent<PlayerStat>();
        rb = GetComponent<Rigidbody2D>();

        if (playerStat == null)
        {
            Debug.LogError("PlayerStat is missing on the Player.");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the Player.");
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Rigidbody의 회전을 고정
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
        lastBazookaTime = -bazookaCooldown;
        lastMeleeAttackTime = -meleeAttackCooldown; // 게임 시작 시 즉시 근접 공격 가능하도록 설정

    }

    void Update()
    {
        // 매 프레임당 방향키 입력받기
        inputVec.x = Input.GetAxisRaw("Horizontal");   // 좌측방향키는 -값 , 우측방향키는 +값으로 inputVec.x에 업데이트
        inputVec.y = Input.GetAxisRaw("Vertical");     // 아래방향키는 -값 , 윗방향키는 +값으로 inputVec.y에 업데이트 
        if (inputVec != Vector2.zero)
        {
            lastDirection = inputVec.normalized;
        }

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



        if (Input.GetKey(KeyCode.B) && Time.time >= lastShootTime + shootCooldown)  // B키 권총
        {
            ShootBullet();
            lastShootTime = Time.time; // 마지막 발사 시간 업데이트
        }

        if (Input.GetKey(KeyCode.N) && Time.time >= lastShootTime + shootCooldown)  // N키 샷건
        {
            ShootGun();
            lastShootTime = Time.time; // 마지막 발사 시간 업데이트
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleBazookaAutoFire();
        }
    }

    void FixedUpdate()
    {
        Vector2 nextVec = inputVec.normalized * playerStat.getPlayerSpeed() * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + nextVec);

        if (HPBar != null)
        {
            Vector3 HPBarPosition = transform.position;
            HPBarPosition.y += 1.0f;
            HPBar.transform.position = HPBarPosition;
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
                    monster.TakeDamage(playerStat.__AttackDamage); // 플레이어의 공격력만큼 피해를 줌
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
                    monster.TakeDamage(playerStat.__AttackDamage); // 플레이어의 공격력만큼 피해를 줌
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

    void ShootGun() // 샷건
    {
        Vector3 baseDirection = (inputVec == Vector2.zero) ? new Vector3(lastDirection.x, lastDirection.y, 0) : new Vector3(inputVec.x, inputVec.y, 0).normalized;  // 기본 발사 방향을 inputVec를 사용하여 설정
        if (baseDirection == Vector3.zero)  // inputVec이 0일 때 총알을 발사하지 않음
            return;

        // 각도를 기준으로 3발의 총알을 발사
        for (int i = -1; i <= 1; i++)
        {
            // i값에 따라 각도를 조정합니다. -1, 0, 1에 따라 -30도, 0도, 30도
            float angle = 30.0f * i;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * baseDirection;

            // 총알 생성 위치를 플레이어 앞쪽으로 설정
            Vector3 bulletSpawnPosition = transform.position + direction * 0.5f; // 플레이어 위치에서 약간 떨어진 위치

            // 총알을 생성하고 발사
            GameObject bullet = Instantiate(Bullet, bulletSpawnPosition, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * BulletSpeed;
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
            }
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
        }
    }
    void ShootBazooka()
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

            // 총알의 isBazukapo 변수를 true로 설정하여 주변적에게 피해를 입힘
            Bullet exploseScript = bullet.GetComponent<Bullet>();
            if (exploseScript != null)
            {
                exploseScript.isBazukapo = true;
            }

        }
    }

    // 바주카포 발사를 ON OFF로 구현
    void ToggleBazookaAutoFire()
    {
        isBazookaAutoFire = !isBazookaAutoFire;
        if (isBazookaAutoFire)
        {
            InvokeRepeating("ShootBazooka", 0f, bazookaCooldown);
        }
        else
        {
            CancelInvoke("ShootBazooka");
        }
    }
}
