using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCoolEffect : MonoBehaviour
{
    public Image[] weaponImages;
    public Image[] hideWeaponImages;
    public static bool[] isCool = { false, false, false, false };
    public static float[] weaponTimes = { 3, 6, 9, 12 }; // 추후에 바뀌어야 함(쿨타임 받아오기)
    public static float[] getWeaponTimes = { 0, 0, 0, 0 };

    private Coroutine[] coolTimeCoroutines = new Coroutine[4];

    void Start()
    {
        foreach (Image weapon in weaponImages)
        {
            weapon.enabled = false;
        }
        foreach (Image hide in hideWeaponImages)
        {
            hide.enabled = false;
        }
    }

    void Update()
    {
        WeaponCoolChk();
        UpdateWeaponImages();
    }

    private void UpdateWeaponImages()
    {
        for (int i = 0; i < weaponImages.Length; i++)
        {
            bool isActive = i < SkillManager.instance.activeSkills.Count;
            weaponImages[i].enabled = isActive;
            hideWeaponImages[i].enabled = isActive;
        }
    }

    void WeaponCoolChk()
    {
        for (int i = 0; i < isCool.Length; i++)
        {
            if (isCool[i] && coolTimeCoroutines[i] == null)
            {
                coolTimeCoroutines[i] = StartCoroutine(CoolTimeChk(i));
            }
        }
    }

    public static void WeaponCoolSetting(int weaponNum)
    {
        getWeaponTimes[weaponNum] = weaponTimes[weaponNum];
        isCool[weaponNum] = true;
    }

    IEnumerator CoolTimeChk(int weaponNum)
    {
        while (getWeaponTimes[weaponNum] > 0)
        {
            getWeaponTimes[weaponNum] -= Time.deltaTime;
            float time = Mathf.Clamp01(getWeaponTimes[weaponNum] / weaponTimes[weaponNum]);
            hideWeaponImages[weaponNum].fillAmount = time;
            yield return null;
        }

        isCool[weaponNum] = false;
        hideWeaponImages[weaponNum].fillAmount = 0f;
        coolTimeCoroutines[weaponNum] = null;
    }
}