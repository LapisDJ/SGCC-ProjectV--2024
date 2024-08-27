using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class BaseballBat : Skill
{  
    protected override void Awake()
    {
        base.Awake();
        skillName = "BaseballBat";
        skillDamage = 2f;
        cooldown = 4f;
        icon = Resources.Load<Sprite>("UI/Icon/12.png");
    }

    private float angle = Mathf.PI / 8; // 부채꼴 범위의 반각
    private float radius = 1.5f; // 반지름
    private Vector2 dir; // 플레이어 시선 방향
    private Vector2 lastdir;
    WeaknessType weaknessType = WeaknessType.Blow;
    public override void Activate() // 몬스터와 상호 작용 로직
    {
        Monster monster;
        float totalDamage = 0f; // 몬스터가 입는 총 데미지
        dir = player.GetComponent<Rigidbody2D>().velocity.normalized; // 이동 방향 단위 벡터
        if(dir == Vector2.zero) dir = lastdir;
        else lastdir = dir;

        // 범위 내의 모든 콜라이더를 가져옴 (히트스캔 방식)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.transform.position, radius);

        foreach (var hitCollider in hitColliders) // 범위 안의 모든 몬스터에 대하여 반복문
        {
            Vector2 directionToCollider = (hitCollider.transform.position - player.transform.position).normalized;
            float angleToCollider = Vector2.Angle(dir, directionToCollider);

            // 부채꼴 범위 안에 있는지 확인
            if (angleToCollider <= angle * Mathf.Rad2Deg) 
            {
                if (hitCollider.CompareTag("Monster1") || hitCollider.CompareTag("Monster3") || hitCollider.CompareTag("Monster2"))
                {
                    monster = hitCollider.GetComponent<Monster>();
                    float weaknessMultipler = (hitCollider.CompareTag("Monster2") || hitCollider.CompareTag("Monster3")) ? 1.5f : 1f;

                    DamageInfo damageInfo = new DamageInfo
                    {
                        skillDamage = this.skillDamage,
                        playerDamage = player.playerStat.attackDamageByLevel,
                        weaknessMultipler = weaknessMultipler,
                        isCritical = player.playerStat.CheckCritical()
                    };

                    totalDamage = finalDamage(damageInfo);
                    monster.TakeDamage(totalDamage);
                }
            }
        }
        if(level == 8)
        {
            StartCoroutine(DoubleAttack());
        }
    }

    public override void LevelUp() // 야구 방망이 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.skillDamage += 1f;

        switch (level)
        {
            case 3: // 2->3랩: 쿨타임 1초 감소
                this.cooldown--;
                break;
            case 4: // 3->4랩: 반지름 2->2.25
                this.radius = 2f;
                break;
            case 6: //5->6랩: 쿨타임 1초 감소
                this.cooldown--;
                break;
            case 7: //6->7랩: 범위 증가
                angle = Mathf.PI / 3;
                break;
            case 8: //2회 공격
                this.skillDamage *= 2;
                break;

        }
    }
    private IEnumerator DoubleAttack() // 8레벨 2회 공격을 코루틴으로 구현
    {
        yield return new WaitForSeconds(0.25f); // 0.25초 대기
        Activate(); // 두 번째 공격
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // 공격 범위의 중심점(플레이어 위치) 지정
        Vector3 center = player.transform.position;

        // 공격 범위(원형) 그리기
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius);

        // 부채꼴 범위 그리기
        Vector2 direction = dir == Vector2.zero ? lastdir : dir;

        // 부채꼴의 시작점과 끝점을 계산
        float angleOffset = angle * Mathf.Rad2Deg;
        Vector2 leftBoundary = Quaternion.Euler(0, 0, -angleOffset) * direction;
        Vector2 rightBoundary = Quaternion.Euler(0, 0, angleOffset) * direction;

        #if UNITY_EDITOR
        // Handles을 사용하여 부채꼴을 채움
        Handles.color = new Color(1, 0, 0, 0.2f); // 반투명 빨간색
        Handles.DrawSolidArc(center, Vector3.forward, leftBoundary, angleOffset * 2, radius);
        #endif

        // 부채꼴의 선 그리기
        Gizmos.DrawLine(center, center + new Vector3(leftBoundary.x, leftBoundary.y, 0) * radius);
        Gizmos.DrawLine(center, center + new Vector3(rightBoundary.x, rightBoundary.y, 0) * radius);

        // 부채꼴의 내부를 채우기 위한 작은 선들
        int segments = 20; // 부채꼴을 나누는 작은 선의 개수
        for (int i = 0; i <= segments; i++)
        {
            float segmentAngle = Mathf.Lerp(-angleOffset, angleOffset, i / (float)segments);
            Vector2 segmentDir = Quaternion.Euler(0, 0, segmentAngle) * direction;
            Gizmos.DrawLine(center, center + new Vector3(segmentDir.x, segmentDir.y, 0) * radius);
        }
    }

}
