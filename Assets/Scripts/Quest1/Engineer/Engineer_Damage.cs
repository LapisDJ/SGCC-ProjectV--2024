using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� ���̺귯�� �߰�

public class Engineer_Damage : MonoBehaviour
{
    public float Engineer_HP = 100; // �����Ͼ� ���� ü�� ����
    public float maxHP = 100f;       // �ִ� ü��

    public void TakeDamage(float damage) // �÷��̾ �޴� ������ ����
    {
        Engineer_HP -= damage;
        if (Engineer_HP <= 0)
        {
            Die();
        }

    }

    public void Die() //�����Ͼ� ����� ���� ����
    {
        Destroy(this.gameObject);
        // ���� ������ ȭ������ ������!
        QuestManager.instance.currentQuest = 3;
        QuestManager.instance.CompleteQuest();
    }
}
