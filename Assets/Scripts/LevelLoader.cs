using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader S;
    public float crossfadeTime = 1f;
    public Animator crossfade;

    private void Awake()
    {
        S = this;
    }

    public void LoadNextLevel(int lvl)
    {
        StartCoroutine(LoadLevel(lvl));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        crossfade.SetTrigger("Start");

        yield return new WaitForSeconds(crossfadeTime);

        SceneManager.LoadScene(levelIndex);
    }

}
