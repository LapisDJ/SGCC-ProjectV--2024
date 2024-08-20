using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant_Rat : Monster
{
    public Giant_Rat() : base(100f, 2f, 2f, WeaknessType.All) { } // 생성자 : 최대 체력, 공격력, 이동 속도, 약점 타입

    public float getAttackDamage()
    {
        return this.attackDamage;
    }
    public float getHP()
    {
        return this.currentHP;
    }
    public float getSpeed()
    {
        return this.speed;
    }
}
