using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    PlayerStat playerStat;
    PlayerController playerController;
    void Awake()
    {
        playerStat = GetComponent<PlayerStat>();
        playerController = GetComponent<PlayerController>();
    }


    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (playerStat != null)
        {
            playerStat.CurrentHP -= damage;
            if (playerStat.CurrentHP <= 0)
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
