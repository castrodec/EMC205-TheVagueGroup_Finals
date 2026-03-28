using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    //Setup
    //Step 1: Place this script on the parent Canvas.
    //Step 2: Create a button and place it as a child of the Canvas.
    //Step 3: In the button's OnClick() event, drag the Canvas into the object field and select LoadScene -> LoadOnClick(String) as the function.
    //Step 4: In the button's OnClick() event, drag the scene you want to load into the object field.

     public void LoadOnClick(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Time.timeScale = 1f; // Reset BEFORE loading
            SceneManager.LoadScene(sceneName);
            Debug.Log("Loading scene: " + sceneName);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor   
#endif
    }
}