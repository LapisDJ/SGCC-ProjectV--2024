using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Dog : Monster
{
    protected override void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        stats = new MonsterStats
        {
            initialHP = 200f,
            initialAttackDamage = 6f,
            initialSpeed = 5f
        };
        InitializeStats();
        weakness = WeaknessType.Slash;
        key = "ZombieDog";
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
