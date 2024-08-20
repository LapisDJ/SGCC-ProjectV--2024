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
}

public class Monster : MonoBehaviour
{
    private float attackRange = Mathf.PI * 2; // 공격 범위
    [SerializeField] protected float currentHP; // 현재 체력
    [SerializeField] protected float attackDamage; // 공격력 
    [SerializeField] protected float speed; // 이동속도
    [SerializeField] protected float lastAttacktime; // 최근 공격 시각
    [SerializeField] protected float attackCooldown; // 공격 쿨타임
    protected WeaknessType weakness; // 약점 타입

    public Monster(float currentHP, float attackDamage, float speed, WeaknessType weakness)
    {
        this.currentHP = currentHP;
        this.attackDamage = attackDamage;
        this.speed = speed;
        this.weakness = weakness;
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
            Die();
        }
    }

    public void Die()
    {
        Destroy(this);
    }
}
