using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Monster
{
    protected override void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        stats = new MonsterStats
        {
            initialHP = 5000f,
            initialAttackDamage = 5f,
            initialSpeed = 2.5f
        };
        InitializeStats();
        weakness = WeaknessType.All;
        key = "Zombie";
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
