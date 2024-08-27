using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public struct DamageInfo
{
    public float skillDamage; // 스킬 데미지
    public float playerDamage; // 플레이어 데미지
    public float weaknessMultipler; // 약점 계수
    public bool isCritical; // 치명타 여부
}
public abstract class Skill : MonoBehaviour
{
    public string skillName; // 스킬명
    public int level; // 스킬 레벨
    public float skillDamage; // 데미지
    public float cooldown; // 쿨타임
    public Image icon;//아이콘
    [SerializeField] protected Player__ player;

    // 생성자에서는 데이터를 초기화하지 않습니다. 대신 Awake 또는 Start를 사용
    protected Skill() // 기본 생성자
    {
        level = 0;
    }

    // Unity의 API를 사용하는 초기화는 Awake에서 처리
    protected virtual void Awake()
    {
        player = FindObjectOfType<Player__>();
        if (player == null)
        {
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다. 씬에 Player 오브젝트가 존재하는지 확인하세요.");
        }
    }

    public virtual void LevelUp() // 스킬 레벨업
    {
        level++;
        // 레벨업에 따른 스킬 데미지나 쿨다운 조정 로직 추가 가능
    }

    public float GetDamage() // 스킬 데미지 리턴
    {
        return skillDamage;
    }

    public float GetCooldown() // 스킬 쿨타임, 각 스킬에서 오버라이딩 필요
    {
        return cooldown;
    }

    // 몬스터와 상호작용하는 추상 메서드
    public abstract void Activate();

    // 데미지 계산을 위한 메서드
    protected float finalDamage(DamageInfo damageInfo)
    {
        float basicDamage = (damageInfo.playerDamage + damageInfo.skillDamage) * damageInfo.weaknessMultipler;
        if (damageInfo.isCritical)
            return basicDamage * 1.5f;
        else
            return basicDamage;
    }

}
