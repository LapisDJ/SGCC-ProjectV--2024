using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveSkill : MonoBehaviour
{
    public string skillName; // 스킬명
    public int level; //스킬 레벨
    public float effect; // 효과량
    public float cooldown; // 쿨타임
    [SerializeField] public Image icon;//아이콘
    protected PassiveSkill() // 기본 생성자
    {
        level = 0;
    }

    // Unity의 API를 사용하는 초기화는 Awake에서 처리합니다.
    protected virtual void Awake()
    {
        
    }
    public virtual void LevelUp() // 스킬 레벨업
    {
        level++;
    }
    public float GetEffect() // 스킬 효과량 리턴
    {
        return effect;
    }

    public float GetCooldown() // 스킬 쿨타임, 각 스킬에서 오버라이딩 필요
    {
        return cooldown;
    }
}
