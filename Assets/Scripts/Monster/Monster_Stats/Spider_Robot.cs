using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider_Robot : Monster
{
    protected override void Awake()
    {
        stats = new MonsterStats
        {
            initialHP = 80f,
            initialAttackDamage = 4f,
            initialSpeed = 6.5f
        };
        InitializeStats();
        weakness = WeaknessType.Blow;
        key = "SmallSpiderRobot";
    }
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
