using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aged_Robot : Monster
{
    public Aged_Robot() : base(25000f, 8f, 3.5f, WeaknessType.Blow, "OldRobot") { } // 생성자 : 최대 체력, 공격력, 이동 속도, 약점 타입

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
