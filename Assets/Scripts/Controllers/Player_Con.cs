using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Con : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Player_Stat playerstat;
    [SerializeField] Vector3 dir;
    [SerializeField] float speed;
    void Start()
    {
        playerstat = GetComponent<Player_Stat>();
        rb = GetComponent<Rigidbody2D>();
        if(playerstat == null)
        {
            Debug.LogError("PlayerStat is missing on the Player.");
        }
        if(rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the Player.");
        }
    }

    
    void FixedUpdate()
    {
        //플레이어 이동
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"),0).normalized;
        rb.velocity =  dir * 5.0f;
    }
}
