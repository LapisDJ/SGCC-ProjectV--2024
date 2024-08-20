using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    Player_Stat playerstat;
    Player_Con playerController;

    void Awake()
    {
        playerstat = GetComponent<Player_Stat>();
        playerController = GetComponent<Player_Con>();
    }


    public void TakeDamage(float damage)
    {
        if (playerstat != null)
        {
            playerstat.HPcurrent -= damage;
            if (playerstat.HPcurrent <= 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
