using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [SerializeField]
    public float MaxHP = 100.0f;
    public float CurrentHP = 100.0f;
    public float __AttackDamage = 10.0f;
    public float __PlayerSpeed = 10.0f;

    public float getPlayerSpeed()
    {
        return this.__PlayerSpeed;
    }

}
