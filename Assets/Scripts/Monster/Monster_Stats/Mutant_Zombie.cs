using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mutant_Zombie : Monster
{
    public Mutant_Zombie() : base(1000000f, 20f, 4f, WeaknessType.Slash) { } // 생성자 : 최대 체력, 공격력, 이동 속도, 약점 타입

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
