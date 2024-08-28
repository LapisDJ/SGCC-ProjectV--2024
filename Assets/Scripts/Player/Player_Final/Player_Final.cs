using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Final : MonoBehaviour
{
    public Player_Stat playerStat;
    public Player_Controller_Final playerController;

    void Awake() // �ʱ�ȭ: ����, ��Ʈ�ѷ� �ҷ�����
    {
        playerStat = GetComponent<Player_Stat>();
        playerController = GetComponent<Player_Controller_Final>();
    }


    public void TakeDamage(float damage) // �÷��̾ �޴� ������ ����
    {
        if (playerStat != null)
        {
            playerStat.HPcurrent -= damage;
            if (playerStat.HPcurrent <= 0)
            {
                Die();
            }
        }
    }

    public void Die() // �÷��̾� ��� ���� ����
    {
        Destroy(this);
    }
}