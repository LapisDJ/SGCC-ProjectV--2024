using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public enum WeaknessType
{
    Slash, // 참격
    Blow, // 타격
    All // 참격, 타격
};

public struct MonsterStats
{
    public float initialHP;        // 초기 체력
    public float initialAttackDamage;  // 초기 공격력
    public float initialSpeed;     // 초기 이동속도
}

public class Monster : MonoBehaviour
{
    private float attackRange = Mathf.PI * 2; // 공격 범위
    protected MonsterStats stats;
    [SerializeField] protected float currentHP; // 현재 체력
    [SerializeField] protected float attackDamage; // 공격력 
    [SerializeField] protected float speed; // 이동속도
    [SerializeField] protected float lastAttacktime; // 최근 공격 시각
    [SerializeField] protected float attackCooldown; // 공격 쿨타임
    [SerializeField] Animator animator;
    [SerializeField] public string key;
    protected WeaknessType weakness; // 약점 타입
    public float fadeDuration = 1.0f; // 알파값이 줄어드는 시간 (초)
    protected SpriteRenderer spriteRenderer;
    protected virtual void Awake()
    {
        // 초기화
        InitializeStats();
    }

    public float GetCurrentSpeed()
    {
        return this.speed;
    }
    void Attack()
    {
        UnityEngine.Vector2 attackPosition = transform.position; // 몬스터의 현재 위치를 공격 중심으로 설정
        attackPosition.y += 0.5f;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPosition, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Player player = hitCollider.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(attackDamage); // 몬스터의 공격력만큼 피해를 줌
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (Time.time >= lastAttacktime + attackCooldown)
        {
            Attack();
            lastAttacktime = Time.time; // 마지막 근접 공격 시간 업데이트
        }

    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            this.speed = 0;
            this.attackDamage = 0;
            animator.SetTrigger("isDie");
            StartCoroutine(FadeOutAndDie());
            RealtimeManager.instance.Monsterkill();
        }
    }

    private IEnumerator FadeOutAndDie()
    {
        yield return FadeOutCoroutine();
        Die();
    }

    public void Die()
    {
        InitializeStats();
        gameObject.SetActive(false);
        SpawnManager.instance.objectPools[this.key].Enqueue(gameObject);
    }
    IEnumerator FadeOutCoroutine()
    {
        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            color.a = newAlpha;
            spriteRenderer.color = color;
            yield return null;
        }

        // 최종적으로 알파값을 0으로 설정
        color.a = 0f;
        spriteRenderer.color = color;
    }

    protected void InitializeStats()
    {
        // 구조체에 정의된 초기 값을 멤버 변수에 할당
        currentHP = stats.initialHP;
        attackDamage = stats.initialAttackDamage;
        speed = stats.initialSpeed;
        lastAttacktime = 0f; // 초기 쿨타임 시간은 0으로 설정
    }

}
