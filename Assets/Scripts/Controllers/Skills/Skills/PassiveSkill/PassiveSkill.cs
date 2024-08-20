using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : MonoBehaviour
{
    public string skillName; // 스킬명
    public int level; //스킬 레벨
    public float effect; // 효과량
    public float cooldown; // 쿨타임
    public PassiveSkill(string name, float baseDamage, float baseCooldown) // 스킬 생성자, 모든 스킬을 레벨 0으로 하여 스킬 리스트에 스킬 객체 삽입(스킬 매니저에 존재)
    {
        this.skillName = name;
        this.effect = baseDamage;
        this.cooldown = baseCooldown;
        this.level = 0;
    }
    public virtual void LevelUp() // 스킬 레벨업
    {
        level++;
    }
    public float GetEffect() // 스킬 효과량 리턴
    {
        return effect;
    }

    public float GetCooldown() // 스킬 쿨타임, 각 스킬에서 오버라이딩 필요
    {
        return cooldown;
    }
}
