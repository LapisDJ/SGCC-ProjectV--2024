using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCoolEffect : MonoBehaviour
{
    public Image[] weaponimages;
    public Image[] hideweaponimages;
    public static bool[] iscool = {false,false,false,false};
    public static float[] weapontimes = {3,6,9,12};//추후에 바뀌어야함(쿨타임 받아오기)
    public static float[] getweapontimes = {0,0,0,0};
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        WeaponCoolChk();
    }
    void Update()//테스트용 코드. 텐키가 아닌 숫자 1부터 4까지 누르면 해당 쿨타임이펙트가 실행됨.
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            WeaponCoolSetting(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            WeaponCoolSetting(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            WeaponCoolSetting(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            WeaponCoolSetting(3);
        }
    }
    public static void WeaponCoolSetting(int weaponnum)
    {
        getweapontimes[weaponnum] = weapontimes[weaponnum];
        iscool[weaponnum] = true;
    }
    private void WeaponCoolChk()//iscool의 여부 판단
    {
        if(iscool[0])
        {
            StartCoroutine(CoolTimeChk(0));
        }
        if(iscool[1])
        {
            StartCoroutine(CoolTimeChk(1));
        }
        if(iscool[2])
        {
            StartCoroutine(CoolTimeChk(2));
        }
        if(iscool[3])
        {
            StartCoroutine(CoolTimeChk(3));
        }
    }

    IEnumerator CoolTimeChk(int weaponnum)//스킬쿨타임 잔여량만큼 불투명한 상자로 가리는 함수. 쿨타임이 0 이하일 시 iscool도 false로 바꿔줌.
    {
        yield return null;
        while(getweapontimes[weaponnum] >= 0)
        {
            getweapontimes[weaponnum] -= Time.deltaTime;
            if(getweapontimes[weaponnum] < 0)
            {
                iscool[weaponnum] = false;
            }
            float time = getweapontimes[weaponnum]/weapontimes[weaponnum];
            hideweaponimages[weaponnum].fillAmount = time;
            yield return null;
        }
    }
}
