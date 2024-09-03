using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aged_Robot : Monster
{
    protected override void Awake()
    {
        stats = new MonsterStats
        {
            initialHP = 25000f,
            initialAttackDamage = 8f,
            initialSpeed = 3.5f
        };
        InitializeStats();
        weakness = WeaknessType.Blow;
        key = "OldRobot";
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
