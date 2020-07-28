using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager _dialogueManager;

    private void Awake()
    {
        _dialogueManager = this;
        //SaveDialogues();
        _dialogueManager.LoadDrama("OptionDrama");
    }
    //-------------------------------------------------------------------------------------------------------
    public DialougDataBase DialougDB;

    public void SaveDialogues()                                                 //僅用於創建基礎XML資料架構
    {
        XmlSerializer serializer = new XmlSerializer(typeof(DialougDataBase));
        FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/XML/Dialoug_data.xml",FileMode.Create);
        serializer.Serialize(fileStream, DialougDB);
        fileStream.Close();
    }

    public void LoadDrama(string dramaScriptName)                               //用於載入XML劇本
    {
        XmlSerializer serializer = new XmlSerializer(typeof(DialougDataBase));
        FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/XML/"+ dramaScriptName + ".xml", FileMode.Open);
        DialougDB = serializer.Deserialize(fileStream) as DialougDataBase;
        fileStream.Close();
    }
    //--------------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class DialougEntry                                                   //段落腳本
    {
        public string characterName;
        //public string soundName;                                              //"這裡本來是要做台詞聲音，但是台詞有很多句，以後可能能用在BGM?"
        public talkingType talkType;
        public talkingPos talkPos;
        public List<string> characterDialoug = new List<string>();
        public List<string> dialougSoundName = new List<string>();
        public List<string> dramaOption = new List<string>();
        public List<string> OptionLoadDramaName = new List<string>();
    }

    [System.Serializable]
    public class DialougDataBase                                                //總腳本
    {
        public List<DialougEntry> list = new List<DialougEntry>();
    }

    public enum talkingType                                                     //對話模式
    {
       CenterOne,
       LeftRightDouble,
       Options,
    }

    public enum talkingPos                                                      //人物位置
    {
        Left,
        Right,
    }

}
