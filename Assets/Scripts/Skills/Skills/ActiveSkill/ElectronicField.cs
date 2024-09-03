using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicField : Skill
{
    protected override void Awake()
    {
        base.Awake();
        skillName = "전기장";
        skillDamage = 0.25f;
        cooldown = 0.5f;
        icon = Resources.Load<Sprite>("UI/Icon/9");
        levelupguide = "플레이어를 중심으로 전기장을 생성합니다";
    }
    private float fieldRadius = 1.5f; // 원 범위 반지름
    WeaknessType weaknessType = WeaknessType.Blow;

    // 각 몬스터의 마지막 데미지 시간을 저장하는 딕셔너리
    private Dictionary<Collider2D, float> lastDamageTime = new Dictionary<Collider2D, float>();

    public override void Activate() // 몬스터와 상호 작용 로직
    {
        Monster monster;
        float totalDamage = 0f; // 몬스터가 입는 총 데미지

        // 범위 내의 모든 콜라이더를 가져옴 (히트스캔 방식)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.transform.position, fieldRadius);

        foreach (var hitCollider in hitColliders) // 범위 안의 모든 몬스터에 대하여 반복문
        {
            if (hitCollider.CompareTag("Monster1") || hitCollider.CompareTag("Monster3") || hitCollider.CompareTag("Monster2"))
            {
                // 현재 시간과 마지막 데미지 받은 시간 비교
                if (lastDamageTime.ContainsKey(hitCollider) && (Time.time - lastDamageTime[hitCollider] < 0.5f))
                {
                    continue; // 0.5초가 지나지 않았으면 데미지 입히지 않음
                }

                monster = hitCollider.GetComponent<Monster>();
                float weaknessMultipler = (hitCollider.CompareTag("Monster2") || hitCollider.CompareTag("Monster3")) ? 1.5f : 1f;

                DamageInfo damageInfo = new DamageInfo
                {
                    skillDamage = this.skillDamage,
                    playerDamage = Player_Stat.instance.attackDamageByLevel,
                    weaknessMultipler = weaknessMultipler,
                    isCritical = Player_Stat.instance.CheckCritical()
                };

                totalDamage = finalDamage(damageInfo);
                monster.TakeDamage(totalDamage);

                // 현재 시간을 마지막 데미지 시간으로 저장
                lastDamageTime[hitCollider] = Time.time;
            }
        }
        lastUsedTime = Time.time;
    }

    public override void LevelUp() // 전자 필드(원형 범위) 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 0.25f * 1f;

        switch (level)
        {
            case 3: // 2->3랩: 반지름 증가
                this.fieldRadius = 1.75f;
                break;
            case 5: // 4->5랩: 반지름 증가
                this.fieldRadius = 2f;
                break;
            case 8: //7->8랩: 반지름 증가
                this.fieldRadius = 2.5f;
                break;
        }
    }
}
