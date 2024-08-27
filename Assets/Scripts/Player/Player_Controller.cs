using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Player_Stat playerstat;
    [SerializeField] public Vector3 dir;
    [SerializeField] float speed;
    public Quest1 Quest1;
    public Quest2 Quest2;
    public Quest3 Quest3;


    public bool isInteracting = false; // �� ��ü ��ȣ�ۿ� �Ϸ� ����
    public float interactionTime = 0f; // ��ȣ�ۿ����� �ð�
    public float requiredInteractionTime = 10.0f; // �ʿ��� ��ȣ�ۿ� �ð�
    public float interactionRange = 5.0f; // ��ȣ�ۿ� ���� �Ÿ�                                     // Quest1 ��ȣ�ۿ� �Ÿ� ( ��� �� �����ϰ� ����� ���� )
    public bool canInteracting = false; // �����Ͼ�� ��ȣ�ۿ� ���� ����
    public float distanceBetweenPlayer;  // �����Ͼ�� �÷��̾� ���� �Ÿ�
    public bool isInteractionStarted = false; // ��ȣ�ۿ��� ���۵Ǿ����� ����
    public Vector3 playerFirstPosition = new Vector3(0.5f, -3f, 0f);//new Vector3(29.5f, -3.5f, 0f); // �ʿ��� �÷��̾� ���� ��ġ
    public Vector3 interactPlayerPosition; // ��ȣ�ۿ�� �÷��̾� ��ġ
    public float hp_temp;   // ��ȣ�ۿ�� hp
    public float hp_Cur; // ���� hp
    public float distanceBetweenFirstPosition = 0f; // �ʿ��� �÷��̾� ���� ��ġ�� ���� ��ġ ������ �Ÿ�
    public bool isBossAppear = false;
    public bool isBossDead = false;
    public bool isWithinXRange = false;     // Quest2 �������� ���� ���� ���
    public bool isWithinYRange = false;     // Quest2 �������� ���� ���� ���
    public Engineer_Controller engineerController;
    // �� ��ȯ bool ����
    public bool isMap1 = true;   // map1 �϶� Quest1 ��ũ��Ʈ ��� ����
    public bool isMap2 = false;   // map2 ���� ����                                                                       
    public bool isMap3 = false;   // map3 ���� ����
    void Start()
    {
        engineerController = GetComponent<Engineer_Controller>();
        if (engineerController == null)
        {
            Debug.LogError("engineerController is missing on the Player.");
        }


        transform.position = playerFirstPosition;
        playerstat = GetComponent<Player_Stat>();
        rb = GetComponent<Rigidbody2D>();
        Quest1 = GetComponent<Quest1>();
        Quest2 = GetComponent<Quest2>();
        Quest3 = GetComponent<Quest3>();
        if (playerstat == null)
        {
            Debug.LogError("PlayerStat is missing on the Player.");
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the Player.");
        }
        if (Quest1 == null)
        {
            Debug.LogError("Quest1 is missing on the Player.");
        }
        if (Quest2 == null)
        {
            Debug.LogError("Quest2 is missing on the Player.");
        }
        if (Quest3 == null)
        {
            Debug.LogError("Quest3 is missing on the Player.");
        }
    }

    void Update()
    {
        if (isMap2)
        {
            // x ��ǥ�� -0.5���� 2.5 �������� Ȯ��
            isWithinXRange = transform.position.x >= -0.5f && transform.position.x <= 2.5f;
            // y ��ǥ�� 34���� 36 �������� Ȯ��
            isWithinYRange = transform.position.y >= 34f && transform.position.y <= 36f;
        }
        // �÷��̾ ��ȣ�ۿ� ���� ���� �̵��� ����
        if (!isInteractionStarted)
        {
            // �÷��̾� �̵� ����
            dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
            speed = playerstat.speedAdd * playerstat.speedMulti;
            rb.velocity = dir * speed;
        }
        else
        {
            // �÷��̾ ��ȣ�ۿ� ���̶�� �������� ���ߵ��� ��
            rb.velocity = Vector2.zero;
        }

        if (isMap1)
        {
            Quest1.Map1();
        }
        if (isMap2)
        {
            Quest2.Map2();
        }
        if (isMap3)
        {
            Quest3.Map3();
        }
    }
}