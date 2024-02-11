using FullSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CCGKit
{
    public sealed class GameManager
    {
        public Model model;

        private static readonly GameManager instance = new GameManager();

        GameObject initer;

        static string getSceneName(string scene) {
            return scene;

#if UNITY_ANDROID
            return scene;
#elif UNITY_STANDALONE || UNITY_WEBGL
            switch (scene) {
                case "Home": return "Home-PC";
                case "Campaign": return "Campaign-PC";
                case "Battle": return "Battle-PC";
                default: return scene;
            }
#else
            return scene;
#endif
        }

        internal static void LoadSceneAsync(string v)
        { SceneManager.LoadSceneAsync(getSceneName(v)); }

        internal static void LoadScene(string v)
        { SceneManager.LoadScene(getSceneName(v)); }

        //public MyIAPManager myIAPManager;

        public UnityAdsExample unityAdsExample;

#if UNITY_ANDROID
        private string getAndroidID()
        {
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
            string android_id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
            return android_id;
        }
#elif UNITY_IOS
        private string getIosID()
        { return UnityEngine.iOS.Device.vendorIdentifier; }
#endif

        public bool initComplite = false;

        System.Collections.IEnumerator Init()
        {
            loadModel();
            // model.loadLangBase();
            yield return model.loadUser();
            if (model.user.exp < 0)
                model.tutorialScene = Model.TUTORIAL_FINISHED;
            if (model.user != null)
            {
                // model.loadLang(model.user.lang);
            }
#if UNITY_ANDROID
            try
            {
                model.deviceID = getAndroidID();
                if ((model.deviceID == "") || (model.deviceID == null)) model.fixDeviceID();
            } catch (Exception)
            { if ((model.deviceID == "") || (model.deviceID == null)) model.fixDeviceID(); }

#elif UNITY_IOS
            model.deviceID = PreloaderScene.ID;
            if ((model.deviceID == "") || (model.deviceID == null)) model.fixDeviceID();
#else
            model.fixDeviceID();
#endif
            //myIAPManager = new MyIAPManager();
            unityAdsExample = new UnityAdsExample();
            initComplite = true;
        }

        private GameManager()
        {
            model = new Model();
            initer = new GameObject("GameManager");
            initer.AddComponent<Empty>();
            initer.GetComponent<MonoBehaviour>().StartCoroutine(Init());
            
        }

        public static GameManager Instance
        { get { return instance; } }

        private fsSerializer serializer = new fsSerializer();

        internal void loadModel()
        {
            var modelPath = Application.persistentDataPath + "/model.json";
            //Debug.Log("Load model from " + modelPath);

            if (File.Exists(modelPath) && !PreloaderScene.test)
            {
                model = Model.parseJSON<Model>(Model.FileContent(modelPath));
                model.fixDeviceID();
            }
            saveModel();
        }

        internal void saveModel()
        {
            var decksPath = Application.persistentDataPath + "/model.json";
            //Debug.Log("Save model to " + decksPath);
            fsData serializedData;
            serializer.TrySerialize(model, out serializedData).AssertSuccessWithoutWarnings();
            var file = new StreamWriter(decksPath);
            var json = fsJsonPrinter.PrettyJson(serializedData);
            file.WriteLine(json);
            file.Close();
        }

        public Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        public Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

        private AudioSource audioSource;

        public void PlaySound(AudioSource _audioSource, AudioClip _clip)
        {
            if (_clip != null)
            {
                if ((_audioSource != null) && (audioSource == null))
                {
                    UnityEngine.Object.DontDestroyOnLoad(_audioSource);
                    audioSource = _audioSource;
                }

                if (audioSource != null)
                {
                    audioSource.clip = _clip;
                    audioSource.Play();
                }
            }
        }

        public void SoundOff()
        { audioSource.volume = 0; }

        public void SoundOn()
        { audioSource.volume = 1; }

        public void StopSound()
        { if (audioSource != null) audioSource.Stop(); }

        public AudioSource music;
    }
}