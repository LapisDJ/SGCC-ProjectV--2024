using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : Player
{
    

    void Awake() // �ʱ�ȭ: ����, ��Ʈ�ѷ� �ҷ�����
    {
        playerStat = GetComponent<Player_Stat>(); // �θ� Ŭ������ stat ����
        playerController = GetComponent<Player_Controller>(); // �θ� Ŭ������ controller ����
    }

    
    /*
    public void TakeDamage(float damage)
    {
        base.TakeDamage(damage); // �θ� Ŭ������ TakeDamage �޼��带 ȣ��

        if (playerStat.HPcurrent <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("�����Ͼ �׾����ϴ�.");
        // ����Ʈ ���� ���� �߰�
        Destroy(gameObject);
    }
    */
}