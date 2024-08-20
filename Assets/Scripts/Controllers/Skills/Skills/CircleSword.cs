using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSword : Skill
{
    public CircleSword() : base("CircleSword", 2f, 4f) { } // 생성자 : 스킬명, 1랩 데미지, 1랩 쿨타임

    private float circleAttackRadius = 2f; // 원 범위 반지름
    public override void Activate(GameObject target) // 몬스터와 상호 작용 로직
    {

    }

    public override void LevelUp() // 칼날 의수(원형 참격) 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch(level)
        {
            case 3: // 2->3랩: 쿨타임 1초 감소
                this.cooldown--; 
                break;
            case 5: // 3->4랩: 반지름 2->2.25
                this.circleAttackRadius = 2.25f;
                break;
            case 6: //5->6랩: 쿨타임 1초 감소
                this.cooldown--;
                break;
            case 7: //6->7랩: 반지름 2->2.25
                this.circleAttackRadius = 2.5f;
                break;
            case 8: //7->8랩: 스킬 데미지 2배 증가
                this.skillDamage *= 2;
                break;

        }
    }
}
