using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{
    protected override void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        stats = new MonsterStats
        {
            initialHP = 80f,
            initialAttackDamage = 1f,
            initialSpeed = 1f
        };
        InitializeStats();
        weakness = WeaknessType.Slash;
        key = "Slime";
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
