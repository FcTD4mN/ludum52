using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Make sure Scene are in the right order
        GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadNextScene();
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
