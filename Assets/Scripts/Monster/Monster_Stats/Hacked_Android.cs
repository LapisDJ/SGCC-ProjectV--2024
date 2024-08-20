using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacked_Android : Monster
{
    public Hacked_Android() : base(250000f, 12f, 5f, WeaknessType.Blow) { } // 생성자 : 최대 체력, 공격력, 이동 속도, 약점 타입

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
