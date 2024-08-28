using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player_Stat playerstat;
    private Rigidbody2D rb;
    private Vector3 dir;
    float speed;
    public bool isInteractionStarted = false; // 상호작용이 시작되었는지 여부
    private void Start()
    {
        playerstat = GetComponent<Player_Stat>();
        if (playerstat == null)
        {
            Debug.LogError("PlayerStat is missing on the Player.");
        }
        rb = GetComponent<Rigidbody2D>();
        transform.position = QuestManager.instance.GetCurrentQuest() switch
        {
            1 => new Vector3(29.5f, -3.5f, 0),
            2 => new Vector3(0.5f, -3f, 0),
            3 => new Vector3(2f, 24f, 0),
             => transform.position
        };
    }

    private void Update()
    {
        if (!isInteractionStarted)
        {
            speed = playerstat.speedAdd * playerstat.speedMulti;
            dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
            rb.velocity = dir * speed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

    }
    /*
    private void FixedUpdate()
    {
        if (!isInteractionStarted)
        {
            rb.velocity = dir * speed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
    */
}