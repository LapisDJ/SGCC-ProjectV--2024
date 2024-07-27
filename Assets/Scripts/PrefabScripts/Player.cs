using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    Player_Stat playerstat;
    Player_Con playerController;
    [SerializeField] float CurrentHP;

    void Awake()
    {
        playerstat = GetComponent<Player_Stat>();
        playerController = GetComponent<Player_Con>();
        CurrentHP = playerstat.GetCurrentHP();
    }


    void FixedUpdate()
    {
        
    }

    public void TakeDamage(float damage)
    {
        CurrentHP = playerstat.GetCurrentHP();
        if (playerstat != null)
        {
            CurrentHP -= damage;
            if (CurrentHP <= 0)
            {
                Die();
            }
            playerstat.Getdamage(damage);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
