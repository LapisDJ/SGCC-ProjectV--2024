using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [SerializeField] float Attack_Damage = 1.0f;
    [SerializeField] float HP = 80.0f;
    [SerializeField] float Speed = 1.0f;
    public float GetAD()
    {
        return this.Attack_Damage;
    }
    public float GetHP()
    {
        return this.HP;
    }
    public float GetSpeed()
    {
        return this.Speed;
    }
}
