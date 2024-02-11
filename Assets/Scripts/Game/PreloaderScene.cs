using CCGKit;
using System.Collections;
using UnityEngine;

#if !STEAM
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreloaderScene : MonoBehaviour
{
    public PreloaderAmination preloader;

    [SerializeField]
    LoadingLine loadingLine;

    bool process = false;

    public static bool auth = false;

    public static bool tutor = false;

    public static string ID;

    public static bool test = false;

    bool choose = false;

	IEnumerator Start ()
    {
        //while(!choose)
           // yield return true; 
            
        if(!test)
            ID = SystemInfo.deviceUniqueIdentifier;
        preloader.Activate();
        yield return new WaitForSeconds(1);
        string sceneName = "Home";

        if (Input.GetKey(KeyCode.T) && tutor == false)
        {
            Debug.LogError("RELOAD");
            tutor = true;
            System.IO.Directory.Delete(Application.persistentDataPath, true);
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

#if UNITY_IOS
         while(!Social.localUser.authenticated){
             if(!process){
                 process = true;
                 Social.localUser.Authenticate(IosAuthorisation);
                 
             }
             while(process)
             yield return true;
            }
#endif

#if !UNITY_STANDALONE
        sceneName = "Home";
#else
                 sceneName = "Home";
#endif

#if STEAM
         sceneName = "Home";
#else
        sceneName = "Home";
#endif

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        bool b = GameManager.Instance.initComplite;
        while (asyncOperation.progress < 0.9f)
        {
            yield return true;
            loadingLine.SetValue(asyncOperation.progress);
        }
        while (!loadingLine.IsLineEnd() && !GameManager.Instance.initComplite)
            yield return true;
        asyncOperation.allowSceneActivation = true;
        SceneManager.LoadScene("Home");
    }

    void Update()
    {
        if (Input.touchCount > 1 || Input.GetKeyDown(KeyCode.N))
        {
            ID = SystemInfo.deviceUniqueIdentifier + Random.Range(0, 1000).ToString();
            test = true;
        }
    }

    public void Normal()
    { choose = true; }

    public void Reset()
    {
        choose = true;
        ID = SystemInfo.deviceUniqueIdentifier + Random.Range(0,1000).ToString();
        test = true;
    }

    void IosAuthorisation(bool b)
    {
        process = false;
        auth=b;
        if(b)
        {
            //Debug.Log("Done");
            if(!test)
            ID = Social.localUser.id;
        }
    }
}