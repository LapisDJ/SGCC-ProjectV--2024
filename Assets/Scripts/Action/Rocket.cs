/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public PlayerStat PlayerStat;
    public GameObject explosionEffectPrefab;  // 폭발 이펙트 프리팹을 연결할 변수
    public float rocketDamage;  // 로켓 피해량
    public float explosionRadius = 1.0f;    // 폭발 반경 ( == 피해 범위 ) - 변화
    // private bool hasExploded = false;   // 로켓 폭발 여부 추적 - 자동 추적중 적이 사망할 경우 고려 

    void Start()
    {
        rocketDamage = PlayerStat.__AttackDamage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            // 충돌한 몬스터에게 피해를 줌
            damageToMonster(other);

            // 반경 내의 다른 몬스터들에게 피해를 줌
            Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D hitMonster in hitMonsters)
            {
                if (hitMonster.CompareTag("Monster") && hitMonster != other)
                {
                    damageToMonster(hitMonster);
                }
            }

            // 총알 제거
            Destroy(gameObject , 0.5f);
        }
    }

    private void damageToMonster(Collider2D monsterCollider)
    {
        Monster monster = monsterCollider.GetComponent<Monster>();
        if (monster != null)
        {
            monster.TakeDamage(rocketDamage);
        }
    }
}
*/

/*
void Start()    // 로켓 생성시 호출
{
    rocketDamage = PlayerStat.__AttackDamage;
}

void Update()
{
    if (!hasExploded)   // 자동 추적 중 적이 사망할 경우
    {
        GameObject nearestMonster = FindNearestMonster();   // 가장 가장 가까운 몬스터 탐색
        if (nearestMonster != null)
        {
            Vector3 direction = (nearestMonster.transform.position - transform.position).normalized;
            GetComponent<Rigidbody2D>().velocity = direction * 1.5f; // 총알 속도 1.5
        }
    }
}

void OnTriggerEnter2D(Collider2D other) // 로켓이 유닛과 충돌한 경우 ( 바주카포 맞은 적은 타격 피해와 폭발 피해 모두 입는 것으로 구현 )
{
    if (other.CompareTag("Monster") && !hasExploded)    // 유닛이 몬스터이며 로켓이 폭발하지 않은 경우
    {
        Monster monster = other.GetComponent<Monster>();
        if (monster != null)    
        {
            monster.TakeDamage(rocketDamage);   // 몬스터가 로켓의 데미지를 입음
            Vector3 collisionPoint = monster.transform.position;
            Explode(collisionPoint);  // 로켓 폭발
            hasExploded = true; // 로켓이 폭발했음을 표시
        }
    }
}

void Explode(Vector3 explosionPosition)
{
    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);    // 폭발 반경안에 모든 Collider 찾음
    foreach (var hitCollider in hitColliders)   // collider가 몬스터인 경우 로켓 데미지 만큼 피해
    {
        if (hitCollider.CompareTag("Monster"))
        {
            Monster monster = hitCollider.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(rocketDamage);
            }
        }
    }


    // 폭발 효과를 나타내기 위해 코루틴을 사용 -> 일정 시간 후에 오브젝트 파괴
    StartCoroutine(ExplosionCoroutine(explosionPosition));
}

IEnumerator ExplosionCoroutine(Vector3 explosionPosition)
{
    GameObject explosionEffect = Instantiate(explosionEffectPrefab, explosionPosition, Quaternion.identity);  // 폭발 이펙트를 생성
    ParticleSystem ps = explosionEffect.GetComponent<ParticleSystem>();
    if (ps != null)
    {
        ps.Play();  // 폭발 이펙트를 재생
    }
    yield return new WaitForSeconds(explosionDuration);
    Destroy(explosionEffect); // 폭발 효과 삭제
    Destroy(gameObject);  // 로켓 파괴
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
*/