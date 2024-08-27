using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : Bullet
{

    void Awake()
    {
        canPenetrate = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster1")||collision.CompareTag("Monster2")||collision.CompareTag("Monster3"))
        {
            Monster monster = collision.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);  // 계산된 데미지를 적용

                if (!canPenetrate)
                {
                    Destroy(gameObject);  // 관통하지 않는 경우 총알 제거
                }
            }
        }
    }

}
