using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant_Rat : MonoBehaviour
{
    [SerializeField] float Attack_Damage = 2.0f;
    [SerializeField] float HP = 100.0f;
    [SerializeField] float Speed = 2.0f;
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
