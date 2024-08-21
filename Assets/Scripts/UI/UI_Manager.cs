using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] GameObject LayoverUI;
    [SerializeField] GameObject SkillChoose;
    [SerializeField] GameObject MissionUI;
    [SerializeField] GameObject ExitSceneUI;
    [SerializeField] SkillManager skillmanager;
    public static bool isskillchoose = false;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        //레이오버를 제외한 모든 UI씬 비활성화
        LayoverUI.SetActive(true);
        SkillChoose.SetActive(false);
        MissionUI.SetActive(false);
        ExitSceneUI.SetActive(false);
    }
    public void SkillChooseStart()//진행중인 게임을 일시정지하고 레벨업할 스킬을 선택하는 창으로 진입.
    {
        Time.timeScale = 0;
        skillmanager.LevelUp();
        SkillChoose.SetActive(true);
    }
    public int tempskillchoice;
    //몇번째 버튼인지에 따라 임시값을 설정하고 확인버튼을 눌렀을 때 skillchoice변수를 변형한다.
    public void firstskillchooosebutton()
    {
        tempskillchoice = 0;
    }
    public void secondskillchooosebutton()
    {
        tempskillchoice = 1;
    }
    public void thirdskillchooosebutton()
    {
        tempskillchoice = 2;
    }
    public void fourthskillchooosebutton()
    {
        tempskillchoice = 3;
    }
    public void confirmskillchoosebutton()
    {
        SkillManager.skillchoice = tempskillchoice;
        SkillChoose.SetActive(false);
        Time.timeScale = 1.0f;
        isskillchoose = false;
        SkillManager.trigger = true;
    }
    void FixedUpdate()
    {
        if(isskillchoose)
        {
            SkillChooseStart();
            isskillchoose = false;
        }
    }
}
