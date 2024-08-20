using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider_Robot : Monster
{
    public Spider_Robot() : base(80f, 4f, 6.5f, WeaknessType.Blow) { } // 생성자 : 최대 체력, 공격력, 이동 속도, 약점 타입

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
