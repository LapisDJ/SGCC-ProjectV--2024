using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillController : MonoBehaviour
{
    public GameObject knifeDronePrefab; // 프리펩 설정
    public float rotationSpeed = 100f; // 회전 속도 설정
    public float knifeRadius = 2f; // 칼날 회전 반지름 설정
    public float fanAttackRadius = 1.5f; // 부채꼴 반지름
    public float fanStartAngle = 30f; // 부채꼴 시작 각도
    public float fanEndAngle = 120f; // 부채꼴 끝 각도
    public float circleAttackRadius = 2f; // 원 범위 반지름
    public Vector2 rectangleAttackSize = new Vector2(3f, 1f); // 직사각형 크기
    public KeyCode rotateKnifeKey = KeyCode.X; // 칼날 회전 키
    public KeyCode fanAttackKey = KeyCode.C; // 부채꼴 공격 키
    public KeyCode circleAttackKey = KeyCode.V; // 원 범위 공격 키
    public KeyCode rectangleAttackKey = KeyCode.B; // 직사각형 범위 공격 키
    public LayerMask enemyLayer; // 적 레이어 마스크
    public Color highlightColor = new Color(1, 0, 0, 0.5f); // 색칠할 색상

    private List<GameObject> knifeDrones = new List<GameObject>(); // 프리펩 인스턴스 리스트
    private int knifeCount = 2; // 초기 칼날 개수
    private int skillLevel = 1; // 초기 스킬 레벨

    void Update()
    {
        // 칼날 회전 스킬
        if (Input.GetKeyDown(rotateKnifeKey))
        {
            if (knifeDrones.Count == 0)
            {
                GenerateKnives();
            }
            else
            {
                DestroyKnives();
            }
        }

        // 칼날 회전
        if (knifeDrones.Count > 0)
        {
            RotateKnives();
        }

        // 부채꼴 공격 스킬
        if (Input.GetKeyDown(fanAttackKey))
        {
            PerformFanShapeAttack();
        }

        // 원 범위 공격 스킬
        if (Input.GetKeyDown(circleAttackKey))
        {
            PerformCircleAttack();
        }

        // 직사각형 범위 공격 스킬
        if (Input.GetKeyDown(rectangleAttackKey))
        {
            PerformRectangleAttack();
        }
    }

    // 공통 공격 로직
    void PerformAttack(Collider2D[] hitEnemies)
    {
        foreach (Collider2D enemy in hitEnemies)
        {
            // 적에게 데미지 주기 등의 로직 추가
            Debug.Log("Enemy hit: " + enemy.name);
        }
    }

    void PerformFanShapeAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, fanAttackRadius, enemyLayer);
        List<Collider2D> enemiesInFan = new List<Collider2D>();

        foreach (Collider2D enemy in hitEnemies)
        {
            Vector2 directionToEnemy = enemy.transform.position - transform.position;
            float angleToEnemy = Vector2.SignedAngle(transform.up, directionToEnemy);

            if (IsWithinFanShape(angleToEnemy))
            {
                enemiesInFan.Add(enemy);
            }
        }

        PerformAttack(enemiesInFan.ToArray());
        StartCoroutine(ShowFanShape());
    }

    bool IsWithinFanShape(float angle)
    {
        float normalizedAngle = (angle + 360) % 360;
        float normalizedStartAngle = (fanStartAngle + 360) % 360;
        float normalizedEndAngle = (fanEndAngle + 360) % 360;

        if (normalizedStartAngle < normalizedEndAngle)
        {
            return normalizedAngle >= normalizedStartAngle && normalizedAngle <= normalizedEndAngle;
        }
        else
        {
            return normalizedAngle >= normalizedStartAngle || normalizedAngle <= normalizedEndAngle;
        }
    }

    void PerformCircleAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, circleAttackRadius, enemyLayer);
        PerformAttack(hitEnemies);
        StartCoroutine(ShowCircle());
    }

    void PerformRectangleAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(transform.position, rectangleAttackSize, 0f, enemyLayer);
        PerformAttack(hitEnemies);
        StartCoroutine(ShowRectangle());
    }

    IEnumerator ShowFanShape()
    {
        GameObject fanShape = new GameObject("FanShape");
        fanShape.transform.position = transform.position;
        MeshFilter meshFilter = fanShape.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = fanShape.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = highlightColor;

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        int segments = 50;
        float angleStep = (fanEndAngle - fanStartAngle) / segments;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        vertices.Add(Vector3.zero); // 중심점

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = fanStartAngle + angleStep * i;
            Vector3 vertex = Quaternion.Euler(0, 0, currentAngle) * Vector3.up * fanAttackRadius;
            vertices.Add(vertex);
            if (i > 0)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        yield return new WaitForSeconds(1f);
        Destroy(fanShape);
    }

    IEnumerator ShowCircle()
    {
        GameObject circle = new GameObject("Circle");
        circle.transform.position = transform.position;
        SpriteRenderer spriteRenderer = circle.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite(circleAttackRadius, highlightColor);

        yield return new WaitForSeconds(1f);
        Destroy(circle);
    }

    Sprite CreateCircleSprite(float radius, Color color)
    {
        int resolution = 256;
        Texture2D texture = new Texture2D(resolution, resolution);
        Color[] colors = new Color[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dx = x - resolution / 2;
                float dy = y - resolution / 2;
                float distance = Mathf.Sqrt(dx * dx + dy * dy) / (resolution / 2);
                colors[y * resolution + x] = distance <= 1 ? color : Color.clear;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        Rect rect = new Rect(0, 0, resolution, resolution);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }

    IEnumerator ShowRectangle()
    {
        GameObject rectangle = new GameObject("Rectangle");
        rectangle.transform.position = transform.position;
        SpriteRenderer spriteRenderer = rectangle.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateRectangleSprite(rectangleAttackSize, highlightColor);

        yield return new WaitForSeconds(1f);
        Destroy(rectangle);
    }

    Sprite CreateRectangleSprite(Vector2 size, Color color)
    {
        int width = (int)(size.x * 100);
        int height = (int)(size.y * 100);
        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colors[y * width + x] = color;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        Rect rect = new Rect(0, 0, width, height);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }

    void GenerateKnives()
    {
        for (int i = 0; i < knifeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / knifeCount;
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * knifeRadius;
            GameObject knife = Instantiate(knifeDronePrefab, transform.position + position, Quaternion.identity);
            knife.transform.SetParent(transform);
            knifeDrones.Add(knife);
        }
    }

    void DestroyKnives()
    {
        foreach (GameObject knife in knifeDrones)
        {
            Destroy(knife);
        }
        knifeDrones.Clear();
    }

    void RotateKnives()
    {
        foreach (GameObject knife in knifeDrones)
        {
            knife.transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fanAttackRadius);

        Vector3 startDirection = Quaternion.Euler(0, 0, fanStartAngle) * transform.up;
        Vector3 endDirection = Quaternion.Euler(0, 0, fanEndAngle) * transform.up;

        Gizmos.DrawLine(transform.position, transform.position + startDirection * fanAttackRadius);
        Gizmos.DrawLine(transform.position, transform.position + endDirection * fanAttackRadius);

        DrawFanShapeGizmo();

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, circleAttackRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, rectangleAttackSize);
    }

    void DrawFanShapeGizmo()
    {
        int segments = 50;
        float angleStep = (fanEndAngle - fanStartAngle) / segments;
        Vector3 previousPoint = transform.position + (Quaternion.Euler(0, 0, fanStartAngle) * transform.up * fanAttackRadius);

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = fanStartAngle + angleStep * i;
            Vector3 currentPoint = transform.position + (Quaternion.Euler(0, 0, currentAngle) * transform.up * fanAttackRadius);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

    // 스킬 레벨 설정 메서드
    public void SetSkillLevel(int level)
    {
        skillLevel = level;
        knifeCount = Mathf.Max(2, skillLevel + 1); // 예: 스킬 레벨에 따라 칼날 개수를 설정
        if (knifeDrones.Count > 0)
        {
            DestroyKnives();
            GenerateKnives();
        }
    }
}
