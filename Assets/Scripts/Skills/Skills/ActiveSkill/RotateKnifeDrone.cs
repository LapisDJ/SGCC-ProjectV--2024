using System.Collections.Generic;
using UnityEngine;

public class RotateKnifeDrone : Skill
{
    private List<float> knifeCooldowns = new List<float>(); // 각 드론별 쿨다운 타이머

    protected override void Awake()
    {
        base.Awake();
        skillName = "회전형 칼날 드론";
        skillDamage = 30f;
        cooldown = 0f;
        icon = Resources.Load<Sprite>("UI/Icon/17");
        levelupguide = "플레이어 주변을 회전하는 드론을 생성합니다";
    }

    public GameObject knifeDronePrefab; // 칼날 드론 프리팹
    public float rotationSpeed = 90f; // 1랩 칼날 회전 속도
    public float knifeRadius = 2f; // 1랩 칼날 회전 반지름
    private List<GameObject> knifeDrones = new List<GameObject>(); // 칼날 개수 리스트
    private int knifeCount = 2; // 1랩 칼날 개수

    private float currentAngle = 0f; // 각도
    public float damageCooldown = 1f; // 데미지 적용 쿨다운

    private void Update()
    {
        RotateKnives();
        Activate();
        UpdateCooldowns();
    }

    public override void Activate()
    {
        // 몬스터와 상호 작용하는 로직은 그대로 유지
        for (int i = 0; i < knifeDrones.Count; i++)
        {
            if (knifeCooldowns[i] > 0) continue; // 쿨다운이 남아있으면 패스

            GameObject knife = knifeDrones[i];
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(knife.transform.position, 1f); // 칼날 크기 맞추어 수정 필요
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Monster1") || hitCollider.CompareTag("Monster2") || hitCollider.CompareTag("Monster3"))
                {
                    Debug.Log("충돌 감지: " + hitCollider.name);
                    Monster monster = hitCollider.GetComponent<Monster>();
                    float totalDamage; // 몬스터가 입는 총 데미지

                    float weaknessMultipler = (hitCollider.CompareTag("Monster1") || hitCollider.CompareTag("Monster3")) ? 1.5f : 1f;
                    if (monster != null)
                    {
                        DamageInfo damageInfo = new DamageInfo
                        {
                            skillDamage = this.skillDamage,
                            playerDamage = Player_Stat.instance.attackDamageByLevel,
                            weaknessMultipler = weaknessMultipler,
                            isCritical = Player_Stat.instance.CheckCritical()
                        };

                        totalDamage = finalDamage(damageInfo);
                        monster.TakeDamage(totalDamage);

                        knifeCooldowns[i] = damageCooldown; // 쿨다운 설정
                    }
                }
            }
        }
        lastUsedTime = Time.time;
    }

    public override void LevelUp()
    {
        base.LevelUp();
        this.skillDamage += 1f;

        switch (level)
        {
            case 1:
                GenerateKnives(this.knifeCount); // 칼날 2개 생성
                break;
            case 3:
                this.knifeCount += 2;
                GenerateKnives(this.knifeCount); // 칼날 4개 생성
                break;
            case 4:
                this.rotationSpeed *= 2; // 회전 속도 2배 증가
                break;
            case 6:
                this.knifeCount += 2;
                GenerateKnives(this.knifeCount); // 칼날 6개 생성
                break;
            case 7:
                this.knifeCount += 2;
                GenerateKnives(this.knifeCount); // 칼날 8개 생성
                break;
            case 8:
                this.rotationSpeed *= 2; // 회전 속도 2배 증가
                break;
        }
    }

    void GenerateKnives(int knifeCount)
    {
        DestroyKnives(); // 기존 칼날 제거

        for (int i = 0; i < knifeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / knifeCount;
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * knifeRadius;
            GameObject knife = Instantiate(knifeDronePrefab, transform.position + position, Quaternion.identity);
            knifeDrones.Add(knife);
            knifeCooldowns.Add(0f); // 쿨다운 초기화
        }
    }

    void DestroyKnives()
    {
        foreach (GameObject knife in knifeDrones)
        {
            Destroy(knife);
        }
        knifeDrones.Clear();
        knifeCooldowns.Clear(); // 쿨다운 리스트도 초기화
    }

    void RotateKnives()
    {
        if (knifeDrones.Count == 0) return;

        currentAngle += rotationSpeed * Time.deltaTime;
        if (currentAngle >= 360f)
        {
            currentAngle -= 360f; // 360도를 넘으면 각도를 초기화
        }

        float angleOffset = 360f / knifeDrones.Count;

        for (int i = 0; i < knifeDrones.Count; i++)
        {
            float angle = currentAngle + i * angleOffset;
            float radian = angle * Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0) * knifeRadius;
            knifeDrones[i].transform.position = player.transform.position + position; // 플레이어 중심으로 드론 위치 설정
        }
    }

    void UpdateCooldowns()
    {
        for (int i = 0; i < knifeCooldowns.Count; i++)
        {
            if (knifeCooldowns[i] > 0)
            {
                knifeCooldowns[i] -= Time.deltaTime;
            }
        }
    }
}
