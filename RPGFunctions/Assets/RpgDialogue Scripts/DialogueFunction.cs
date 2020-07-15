using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DialogueFunction : MonoBehaviour
{
    public RawImage characterCImg;                                               //For Center Mode

    public RawImage characterLImg;                                              //For   L/R  Mode

    public RawImage characterRImg;                                              //For   L/R  Mode

    public GameObject DialogueBox;                                              //The Dialogue Box

    public Text DialogueBoxText;                                                //The Dialogue Box Text

    public Text CharacterName;                                                  //The Dialogue Box Text

    public AudioSource audioPlayer;                                             //The Audio Player

    private AudioClip currentSound;                                             //The Sound that give to AudioPlayer

    private int dialogueNumber = 0;                                             //Record Current Dialogue Number

    private int partNumber = 0;                                                 //Record Current Part Number(Change it when switch character)

    private DialogueManager.DialougDataBase dialoguedata;                       //Get dialoguedata for manager(current drama script)

    private void Start()                                                        
    {
        dialoguedata = DialogueManager._dialogueManager.DialougDB;              //載入劇本資訊

        AdjustCharacterPosition(1);                                             //下一Part角色位置設定
        AdjustCharacterPosition(0);                                             //第一Part角色位置設定

        LoadCharacterDialogue();                                                //載入台詞
        StartCoroutine(LoadAudioFile("poyo"));                                  //載入聲音
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))            //Click to show the next Dialogue
        {
            NextDialogue();
        }  
        if (Input.GetKeyDown(KeyCode.X))            //Click to skip Dialogue
        {

            SkipDialogue();
        }  
        if (Input.GetKeyDown(KeyCode.C))            //Click to reset Dialogue 
        {

            ResetDialogue();
        }         
        if (Input.GetKeyDown(KeyCode.V))            //Click to change the drama
        {

            RequestChangeDrama("Dialoug_data2");

        }  

    }
    public void NextDialogue()
    {
        dialogueNumber++; LoadCharacterDialogue(); Debug.Log("P:" + partNumber);
    }
    public void LoadCharacterDialogue()
    {


        if (partNumber + 1 > dialoguedata.list.Count - 1 && dialogueNumber > dialoguedata.list[partNumber].characterDialoug.Count - 1)
        {
            Debug.Log("The Drama Part is ended");             //如果已經到劇本最後一個段落，通知劇本結束
            DialogueBox.SetActive(false);                     //隱藏對話框與角色圖
            return;
        }

        if (dialogueNumber > dialoguedata.list[partNumber].characterDialoug.Count - 1)          //如果到最後一句台詞
        {
            partNumber++;                                                                       //切到下一Part(Part指標加一)
            Debug.Log("The character's Dialogue is ended");                                     //通知該角色台詞說完了
            AdjustCharacterPosition(partNumber);                                                //重設下一Part角色位置
            dialogueNumber = 0;                                                                 //從該Part第一句台詞開始(指標歸零)
        }
        CharacterName.text = dialoguedata.list[partNumber].characterName;                       //角色名等於該Part設定之角色名
        DialogueBoxText.text = dialoguedata.list[partNumber].characterDialoug[dialogueNumber];  //對話等於該Part之指標點對話
        //Maybe in future we can Play Sound or animation here.
        //Debug.Log(partNumber);
    }

    public void ResetDialogue()         //當前劇本重置
    {
        audioPlayer.Stop();             //停止台詞聲音
        DialogueBox.SetActive(true);    //顯示對話框
        dialogueNumber = 0;             //重設對話指標(歸零)
        partNumber = 0;                 //重設Part指標(歸零)
        AdjustCharacterPosition(1);     //設定下一Part角色位置
        AdjustCharacterPosition(0);     //設定第一Part角色位置
        LoadCharacterDialogue();        //第一句台詞載入(指標0)
    }

    public void SkipDialogue()          //跳過劇情
    {
        audioPlayer.Stop();             //停止台詞聲音
        DialogueBox.SetActive(false);   //關閉對話框
        dialogueNumber = 0;             //重設對話指標(歸零)
        partNumber = 0;                 //重設Part指標(歸零)
        AdjustCharacterPosition(1);     //設定下一Part角色位置
        AdjustCharacterPosition(0);     //設定第一Part角色位置
    }

    public void RequestChangeDrama(string NextDramaName)            //要求劇本更換(日後可用於對話分支)
    {
        SkipDialogue();                                             //跳過當前劇情
        DialogueManager._dialogueManager.LoadDrama(NextDramaName);  //發送更換劇本要求
        dialoguedata = DialogueManager._dialogueManager.DialougDB;  //更新對話資料
        ResetDialogue();                                            //重置對話
    }

    protected Texture2D LoadCharacterImage(string characterName)
    {
        #region LoadImage
        //characterLImg.texture = null;
        //characterRImg.texture = null;

        FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/CharacterPictures/" + characterName + ".png", FileMode.Open,FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
        Texture2D texture = new Texture2D(600, 800);
        texture.LoadImage(bytes);

        return texture;
        #endregion
    }

    private void AdjustCharacterPosition(int PNumber)
    {
        #region AdjustCharacterPosition

        switch (dialoguedata.list[PNumber].talkType)                                            //依據Part中的狀態機判斷對話模式
        {
            case DialogueManager.talkingType.LeftRightDouble:                                   //如果是雙人左右對話狀態

                characterCImg.GetComponent<RawImage>().color = new Color32(255, 255, 255, 0);   //單人模式角色圖隱形

                switch (dialoguedata.list[PNumber].talkPos)                                     //依據Part中的狀態機判斷角色位置
                {
                    case DialogueManager.talkingPos.Left:                                       //如果在左邊，左邊相框讀取角色圖
                        characterLImg.texture = LoadCharacterImage(dialoguedata.list[PNumber].characterName);
                        characterRImg.GetComponent<RawImage>().color = new Color32(75, 75, 75, 255);       //右邊相框變灰(沒對話角色比較暗)
                        characterLImg.GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);    //左邊相框正常
                        break;
                    case DialogueManager.talkingPos.Right:                                                 //角色在右則反之
                        characterRImg.texture = LoadCharacterImage(dialoguedata.list[PNumber].characterName);
                        characterLImg.GetComponent<RawImage>().color = new Color32(75, 75, 75, 255);
                        characterRImg.GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);
                        break;
                }
                break;

            case DialogueManager.talkingType.CenterOne:                                                 //如果是單人對話模式
                characterLImg.GetComponent<RawImage>().color = new Color32(75, 75, 75, 0);              //雙人模式角色圖隱形
                characterRImg.GetComponent<RawImage>().color = new Color32(255, 255, 255, 0);           //雙人模式角色圖隱形
                characterCImg.GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);         //單人模式角色圖顯示
                characterCImg.texture = LoadCharacterImage(dialoguedata.list[PNumber].characterName);   //讀取角色圖
                break;
        }

        #endregion
    }

    private IEnumerator LoadAudioFile(string SoundName)
    {
        string path = Application.streamingAssetsPath + "/Sound/" + SoundName + ".wav";

        UnityWebRequest AudioFiles = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV);

        yield return AudioFiles.SendWebRequest();
        if (AudioFiles.isNetworkError)
        {
            Debug.Log(AudioFiles.error);
            Debug.Log(path + "isError");
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(AudioFiles);
            currentSound = clip;
            audioPlayer.clip = currentSound;
            audioPlayer.Play();
        }
    }

    private void OnGUI()
    {
        
        if (GUILayout.Button("NextLine", GUILayout.Width(300), GUILayout.Height(100))) { dialogueNumber++; LoadCharacterDialogue(); Debug.Log("P:" + partNumber); }
        if (GUILayout.Button("Skip", GUILayout.Width(300), GUILayout.Height(100))) { SkipDialogue(); }
        if (GUILayout.Button("Reset", GUILayout.Width(300), GUILayout.Height(100))) { ResetDialogue(); }
        if (GUILayout.Button("ChangeDrama", GUILayout.Width(300), GUILayout.Height(100))) { RequestChangeDrama("Dialoug_data2"); }
        if (GUILayout.Button("Exit", GUILayout.Width(300), GUILayout.Height(100))) { Application.Quit(); }

    }
}
