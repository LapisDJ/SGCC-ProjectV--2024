using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class MonsterController : MonoBehaviour
{

    public GameObject player;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float speed = 0.0f;
    [SerializeField] Monster monster;

    void Start()
    {
        monster = GetComponent<Monster>();
        rb = GetComponent<Rigidbody2D>();
        speed = monster.GetCurrentSpeed();
    }
        void Update()
    {
        Vector3 PlayerPosition = player.transform.position;
        Vector3 MonsterPosition = transform.position;
        rb.velocity = (PlayerPosition - MonsterPosition).normalized * speed;
    }
}
