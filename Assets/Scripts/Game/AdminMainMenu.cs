using UnityEngine;
using UnityEngine.SceneManagement;

public class AdminMainMenu : MonoBehaviour
{
    public PreloaderAmination preloader;

    public void SwitchScene(string scene)
    {
        preloader.Deactivate();

        SceneManager.LoadScene(scene);
    }
}