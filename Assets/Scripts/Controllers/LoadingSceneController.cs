using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] Slider loadingbar;
    static string nextscene;

    public static void Loadscene(string scenename)
    {
        nextscene = scenename;
        SceneManager.LoadScene("Loading");
    }

    void Start()
    {
        StartCoroutine(Loadsceneprosess());
    }
    IEnumerator Loadsceneprosess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextscene);
        op.allowSceneActivation = false;
        while(!op.isDone)
        {
            yield return null;
            if(op.progress < 0.9f)
            {
                loadingbar.value = op.progress;
            }
            else
            {
                op.allowSceneActivation = true;
                yield break;
            }
        }
    }
}
