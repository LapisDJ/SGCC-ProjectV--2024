using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : Player
{
    // �����Ͼ� ���� ü�� ����
    [SerializeField] private float engineerBaseHP = 100.0f; // �����Ͼ� �⺻ HP


    public void TakeDamage(float damage)
    {
        Debug.Log($"�����Ͼ {damage} �������� �޾ҽ��ϴ�. ���� ü��: {engineerBaseHP}");
    }

    public void Die()
    {
        Debug.Log("�����Ͼ ����߽��ϴ�.");
    }
}
 