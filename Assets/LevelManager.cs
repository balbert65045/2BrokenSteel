using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    // Use this for initialization
    public float autoLoadNextLevelAfter;

    void Start()
    {
        if (autoLoadNextLevelAfter <= 0)
        {
            Debug.Log("Auto Load Disabled, use a positive number in seconds");
        } else
        {
            Invoke("LoadNextLevel", autoLoadNextLevelAfter);
        }
    }


	public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
        Debug.Log("Level load requested for: " + name);
    }

    public void QuitRequest()
    {
       Application.Quit();
        Debug.Log("Quit Game");
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
