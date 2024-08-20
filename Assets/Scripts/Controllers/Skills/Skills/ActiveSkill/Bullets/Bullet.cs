using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool isBazukapo;
    public float explosionRadius;
    public float damage;
    public bool canPenetrate;
    public delegate void HitMonsterDelegate(GameObject hitMonster);
    public event HitMonsterDelegate OnHitMonster;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            OnHitMonster?.Invoke(collision.gameObject);

            if (!canPenetrate)
            {
                Destroy(gameObject);
            }
        }
    }
}
