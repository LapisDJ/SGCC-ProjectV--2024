using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adrenaline : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "아드레날린 주사기";
        effect = 0.09f;
        cooldown = 0f;
        icon = Resources.Load<Sprite>("UI/Icon/5");
        this.levelupguide = "공격력이 증가합니다.";
    }
        public override void LevelUp() // 아드레날린 주사기 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.03f; // 1레벨을 제외한 모든 레벨에서 효과가 3%p씩 상승.
        Player_Stat.instance.attackDamagebypassive = effect;
        this.levelupguide = "공격력 증가 " + Convert.ToString((effect - 0.03f) * 100) + "% -> " + Convert.ToString(effect * 100) + "%";
    }
}
