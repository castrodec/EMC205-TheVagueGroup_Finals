using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //Setup
    //Step 1: Place this script on the parent Canvas.
    //Setp 2: Create a button and place it as a child of the Canvas.
    //Step 3: In the button's OnClick() event, drag the Canvas into the object field and select MainMenuManager -> LoadOnClick(Object) as the function.
    //Step 4: In the button's OnClick() event, drag the scene you want to load into the object field.
    
        public void LoadOnClick(Object sceneToLoad)
    {
        if (sceneToLoad != null)
            SceneManager.LoadScene(sceneToLoad.name);
    }

    public void QuitOnClick()
    {
        Application.Quit();
    }
}
