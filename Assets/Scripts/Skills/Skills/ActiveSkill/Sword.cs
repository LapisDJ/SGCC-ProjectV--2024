using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Skill
{
    float attackWidth = 3f; // 가로 범위
    float attackHeight = 1f; //세로 범위
    WeaknessType weaknessType = WeaknessType.Slash; // 공격 타임 : 참격 
    protected override void Awake()
    {
        base.Awake();
        skillName = "검";
        skillDamage = 2f;
        cooldown = 4f;
        icon = Resources.Load<Sprite>("UI/Icon/11");
    }
    public override void Activate() // 몬스터와 상호 작용 로직
    {
        Vector2 attackSize = new Vector2(attackWidth, attackHeight); // 공격 범위
        Vector2 attackPosition = player.transform.position; // 기준점 : 플레이어 위치
        Monster monster;
        float totalDamage = 0f; // 몬스터가 입는 총 데미지

        // 범위 내의 모든 콜라이더를 가져옴 (히트스캔 방식)
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(attackPosition, attackSize, 0f);

        foreach (var hitCollider in hitColliders) // 범위 안의 모든 몬스터에 대하여 반복문
        {
            if (hitCollider.CompareTag("Monster1") || hitCollider.CompareTag("Monster3") || hitCollider.CompareTag("Monster2"))
            {
                monster = hitCollider.GetComponent<Monster>();
                float weaknessMultipler = (hitCollider.CompareTag("Monster1") || hitCollider.CompareTag("Monster3")) ? 1.5f : 1f;

                DamageInfo damageInfo = new DamageInfo
                {
                    skillDamage = this.skillDamage,
                    playerDamage = player.playerStat.attackDamageByLevel,
                    weaknessMultipler = weaknessMultipler,
                    isCritical = player.playerStat.CheckCritical()
                };

                totalDamage = finalDamage(damageInfo);
                monster.TakeDamage(totalDamage);
            }

            if (level >= 8)
            {
                StartCoroutine(DoubleAttack());
            }
        }
        lastUsedTime = Time.time;
    }

    public override void LevelUp() // 검 레벨 업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch (level)
        {
            case 3:
                this.cooldown--;
                break;
            case 5:
                this.cooldown--;
                break;
            case 6:
                this.cooldown--;
                break;
            case 7: // 6->7랩: 범위 5x1로 증가
                attackWidth = 5f;
                break;
        }
    }

    private IEnumerator DoubleAttack() // 8레벨 2회 공격을 코루틴으로 구현
    {
        yield return new WaitForSeconds(0.25f); // 0.25초 대기
        Activate(); // 두 번째 공격
    }

}
