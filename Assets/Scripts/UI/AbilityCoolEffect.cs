using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCoolEffect : MonoBehaviour
{
    public Image abilityImage;
    public Image hideAbilityImage;
    public static bool isAbilityCool = false; // 능력 쿨타임 여부
    public static float abilityTime = 5f; // 능력 쿨타임
    public static float getAbilityTime = 0f; // 능력 사용 후 시간 체크

    private Coroutine coolTimeCoroutine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 테스트용 키코드. 스페이스를 누르면 능력쿨타임이 돌아간다.
        {
            AbilityCoolSetting();
        }
        if(isAbilityCool)
        {
            AbilityCoolChk();
        }
    }

    private void AbilityCoolChk()
    {
        if (isAbilityCool && coolTimeCoroutine == null)
        {
            coolTimeCoroutine = StartCoroutine(CoolTimeChk());
        }
    }

    public static void AbilityCoolSetting()
    {
        getAbilityTime = abilityTime;
        isAbilityCool = true;
    }

    IEnumerator CoolTimeChk() // 능력 쿨타임 잔여량만큼 불투명한 상자로 가리는 함수. 쿨타임이 0 이하일 시 isAbilityCool도 false로 바꿔줌.
    {
        while (getAbilityTime > 0)
        {
            getAbilityTime -= Time.deltaTime;
            float time = Mathf.Clamp01(getAbilityTime / abilityTime);
            hideAbilityImage.fillAmount = time;
            yield return null;
        }

        // 쿨타임 종료 처리
        isAbilityCool = false;
        hideAbilityImage.fillAmount = 0f;
        coolTimeCoroutine = null;
    }
}