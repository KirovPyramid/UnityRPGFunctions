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
        _dialogueManager.LoadDrama("Dialoug_data");
    }
    //-----------------------------------------------------------
    public DialougDataBase DialougDB;

    public void SaveDialogues()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(DialougDataBase));
        FileStream fileStream = new FileStream(Application.dataPath+"/StreamingFiles/XML/Dialoug_data.xml",FileMode.Create);
        serializer.Serialize(fileStream, DialougDB);
        fileStream.Close();
    }

    public void LoadDrama(string dramaScriptName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(DialougDataBase));
        FileStream fileStream = new FileStream(Application.dataPath + "/StreamingFiles/XML/"+ dramaScriptName + ".xml", FileMode.Open);
        DialougDB = serializer.Deserialize(fileStream) as DialougDataBase;
        fileStream.Close();

        Debug.Log(DialougDB.list[0].characterName);
        Debug.Log(DialougDB.list[1].characterName);
    }

    [System.Serializable]
    public class DialougEntry
    {
        public string characterName;
        public string soundName;
        public talkingType talkType;
        public talkingPos talkPos;
        public List<string> characterDialoug = new List<string>();
    }
    [System.Serializable]
    public class DialougDataBase
    {
        public List<DialougEntry> list = new List<DialougEntry>();
    }

    public enum talkingType
    {
       CenterOne,
       LeftRightDouble,
    }

    public enum talkingPos
    {
        Left,
        Right,
    }
}
