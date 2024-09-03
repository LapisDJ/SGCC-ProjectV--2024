using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mutant_Zombie : Monster
{
    protected override void Awake()
    {
        stats = new MonsterStats
        {
            initialHP = 1000000f,
            initialAttackDamage = 20f,
            initialSpeed = 4f
        };
        InitializeStats();
        weakness = WeaknessType.Slash;
        key = "MutantZombie";
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
