/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public PlayerStat PlayerStat;
    public GameObject explosionEffectPrefab;  // ���� ����Ʈ �������� ������ ����
    public float rocketDamage;  // ���� ���ط�
    public float explosionRadius = 1.0f;    // ���� �ݰ� ( == ���� ���� ) - ��ȭ
    // private bool hasExploded = false;   // ���� ���� ���� ���� - �ڵ� ������ ���� ����� ��� ��� 

    void Start()
    {
        rocketDamage = PlayerStat.__AttackDamage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            // �浹�� ���Ϳ��� ���ظ� ��
            damageToMonster(other);

            // �ݰ� ���� �ٸ� ���͵鿡�� ���ظ� ��
            Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D hitMonster in hitMonsters)
            {
                if (hitMonster.CompareTag("Monster") && hitMonster != other)
                {
                    damageToMonster(hitMonster);
                }
            }

            // �Ѿ� ����
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
void Start()    // ���� ������ ȣ��
{
    rocketDamage = PlayerStat.__AttackDamage;
}

void Update()
{
    if (!hasExploded)   // �ڵ� ���� �� ���� ����� ���
    {
        GameObject nearestMonster = FindNearestMonster();   // ���� ���� ����� ���� Ž��
        if (nearestMonster != null)
        {
            Vector3 direction = (nearestMonster.transform.position - transform.position).normalized;
            GetComponent<Rigidbody2D>().velocity = direction * 1.5f; // �Ѿ� �ӵ� 1.5
        }
    }
}

void OnTriggerEnter2D(Collider2D other) // ������ ���ְ� �浹�� ��� ( ����ī�� ���� ���� Ÿ�� ���ؿ� ���� ���� ��� �Դ� ������ ���� )
{
    if (other.CompareTag("Monster") && !hasExploded)    // ������ �����̸� ������ �������� ���� ���
    {
        Monster monster = other.GetComponent<Monster>();
        if (monster != null)    
        {
            monster.TakeDamage(rocketDamage);   // ���Ͱ� ������ �������� ����
            Vector3 collisionPoint = monster.transform.position;
            Explode(collisionPoint);  // ���� ����
            hasExploded = true; // ������ ���������� ǥ��
        }
    }
}

void Explode(Vector3 explosionPosition)
{
    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);    // ���� �ݰ�ȿ� ��� Collider ã��
    foreach (var hitCollider in hitColliders)   // collider�� ������ ��� ���� ������ ��ŭ ����
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


    // ���� ȿ���� ��Ÿ���� ���� �ڷ�ƾ�� ��� -> ���� �ð� �Ŀ� ������Ʈ �ı�
    StartCoroutine(ExplosionCoroutine(explosionPosition));
}

IEnumerator ExplosionCoroutine(Vector3 explosionPosition)
{
    GameObject explosionEffect = Instantiate(explosionEffectPrefab, explosionPosition, Quaternion.identity);  // ���� ����Ʈ�� ����
    ParticleSystem ps = explosionEffect.GetComponent<ParticleSystem>();
    if (ps != null)
    {
        ps.Play();  // ���� ����Ʈ�� ���
    }
    yield return new WaitForSeconds(explosionDuration);
    Destroy(explosionEffect); // ���� ȿ�� ����
    Destroy(gameObject);  // ���� �ı�
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