using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterChooseButton : MonoBehaviour
{
    public void Confirmbutton()
    {
        SceneManager.LoadScene("Prototype");
    }
    public void Previousbutton()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
