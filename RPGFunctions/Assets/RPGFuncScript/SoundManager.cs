using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using System;

public class SoundManager : MonoBehaviour
{

    public static SoundManager _SoundManager;
    public AudioSource dialoguePlayer, bgmPlayer, fxPlayer;
    

    // Start is called before the first frame update
    private void Awake()
    {
        _SoundManager = this;
        // 初始化三個 Player 的 Volume
        //SettingDB settingDB = load_settingDB();

        //dialoguePlayer.volume = settingDB.vocalValue * 0.2f;
        //bgmPlayer.volume = settingDB.bgmValue * 0.2f;
        //fxPlayer.volume = settingDB.bgmValue * 0.2f;
        
    }

    void Start()
    {
        
    }

    //使用說明

    //在你想要用的Day腳本 你想觸發的地方 透過 

    //StartCoroutine(SoundManager._SoundManager.LoadAudioFile("聲音檔名", 指定的AudioSource, 隨便一個空的AudioClip 填充用 只要有宣告就好));

    //角色台詞還是用原本的XML配DialogueFunction做控制

    //控制台詞、效果音跟BGM的AudioSource我會包在Prefab裡，可能在DialogueFunction的台詞聲音播放器參考要重放一次

    //日後此腳本可能還會擴充用於控制音量大小，也有可能我會另寫一個腳本

    public IEnumerator LoadAudioFile(string SoundName, AudioSource certainSource,AudioClip certainClip, string type= ".MP3", AudioType audioType = AudioType.MPEG)
    {
        if (SoundName == "null") { yield break; }
        string path = Application.streamingAssetsPath + "/Sound/" + SoundName + type;
        

        UnityWebRequest AudioFiles = UnityWebRequestMultimedia.GetAudioClip(path, audioType);

        yield return AudioFiles.SendWebRequest();
        if (AudioFiles.isNetworkError)
        {
            Debug.Log(AudioFiles.error);
            Debug.Log(path + "isError");
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(AudioFiles);
            if(certainSource.name == "BGMPlayer")
            {
                clip.name = SoundName;
            }
            certainClip = clip;
            certainSource.clip = certainClip;
            certainSource.Play();
        }
    }


    /*private SettingDB load_settingDB()
    {
        SettingDB settingDB = new SettingDB();
        string fileName = Application.streamingAssetsPath + "/LoadDB/Setting";
        // load json
        string readJson;
        try
        {
            readJson = System.IO.File.ReadAllText(fileName + ".json");
            settingDB = JsonUtility.FromJson<SettingDB>(readJson);
        }
        catch
        {
            Debug.Log("NO FILE " + fileName);
        }
        return settingDB;
    }*/
}
