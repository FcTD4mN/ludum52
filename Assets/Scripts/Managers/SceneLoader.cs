using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /*
    * Used for scene transition
    * Using next Scene order
    */
    public void LoadNextScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /*
    * Used for scene transition
    * Using next Scene order
    */
    public void LoadPreviousScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void LoadScene(int whichScene)
    {
        // Load next scene
        SceneManager.LoadScene(whichScene);
    }

    /*
    * Used for scene transition
    * Using next Scene order
    */
    public void LoadNextScene(string sName)
    {
        LoadScene(sName);
    }

    public void LoadScene(string whichScene)
    {
        // Load next scene
        SceneManager.LoadScene(whichScene);
        SceneManager.LoadScene(whichScene, LoadSceneMode.Single);
    }
}
