using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant_Vat : MonoBehaviour
{
    [SerializeField] float Attack_Damage = 4.0f;
    [SerializeField] float HP = 200.0f;
    [SerializeField] float Speed = 3.5f;
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
