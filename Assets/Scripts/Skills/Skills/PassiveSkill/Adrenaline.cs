using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adrenaline : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "Adrenaline";
        effect = 0.09f;
        cooldown = 0f;
        icon.sprite = Resources.Load<Sprite>("Assets/TeamAssets/skill_icon/5.png");
    }
        public override void LevelUp() // 아드레날린 주사기 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.03f; // 1레벨을 제외한 모든 레벨에서 효과가 3%p씩 상승.
    }
}
