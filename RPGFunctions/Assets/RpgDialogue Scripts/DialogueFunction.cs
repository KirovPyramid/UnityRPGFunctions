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
    public GameObject optionButtonPrefab;                                       //預製按鈕

    public GameObject optionButtonGroup;                                        //用來排列按鈕的UI母物件

    public List<GameObject> optionButtonPrefabs = new List<GameObject>();       //用來記錄場上按鈕樹的按鍵

    public RawImage characterCImg;                                              //For Center Mode

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

        ResetDialogue();                                                        //重製劇本

        LoadCharacterDialogue();                                                //載入台詞
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))            //測試用(下一句台詞)
        {
            NextDialogue();
        }  
        if (Input.GetKeyDown(KeyCode.X))            //測試用(略過劇本)
        {

            SkipDialogue();
        }  
        if (Input.GetKeyDown(KeyCode.C))            //測試用(重製劇本)
        {

            ResetDialogue();
        }
        if (Input.GetKeyDown(KeyCode.V))            //測試用(載入劇本)
        {

            RequestChangeDrama("NormalDoubleTalkDrama");
        }
    }
    public void NextDialogue()                                  //載入下一句對話
    {
        dialogueNumber++; LoadCharacterDialogue(); Debug.Log("P:" + partNumber);
    }
    public void LoadCharacterDialogue()                        //主要載入對話功能
    {


        if (partNumber + 1 > dialoguedata.list.Count - 1 && dialogueNumber > dialoguedata.list[partNumber].characterDialoug.Count - 1)
        {
            Debug.Log("The Drama Part is ended");             //如果已經到劇本最後一個段落，通知劇本結束
            DialogueBox.SetActive(false);                     //隱藏對話框與角色圖
            return;                                           //退出
        }

        if (dialogueNumber > dialoguedata.list[partNumber].characterDialoug.Count - 1)          //如果到最後一句台詞
        {
            partNumber++;                                                                       //切到下一Part(Part指標加一)
            Debug.Log("The character's Dialogue is ended");                                     //通知該角色台詞說完了
            AdjustCharacterPosition(partNumber);                                                //重設下一Part角色位置
            dialogueNumber = 0;                                                                 //從該Part第一句台詞開始(指標歸零)
        }

        StartCoroutine(LoadAudioFile(dialoguedata.list[partNumber].dialougSoundName[dialogueNumber]));
        CharacterName.text = dialoguedata.list[partNumber].characterName;                       //角色名等於該Part設定之角色名
        DialogueBoxText.text = dialoguedata.list[partNumber].characterDialoug[dialogueNumber];  //對話等於該Part之指標點對話
        
        //之後說不定可以塞LIVE2D的動作在這裡，只是要把上面照片的部分改成載入LIVE2D的Prefab
    }

    public void ResetDialogue()         //當前劇本重置
    {
        RemoveOptionButtons();          //移除按鈕
        audioPlayer.Stop();             //停止台詞聲音
        DialogueBox.SetActive(true);    //顯示對話框
        dialogueNumber = 0;             //重設對話指標(歸零)
        partNumber = 0;                 //重設Part指標(歸零)
        AdjustCharacterPosition(0);     //設定第一Part角色位置
        LoadCharacterDialogue();        //第一句台詞載入(指標0)
    }

    public void SkipDialogue()          //跳過劇情
    {

        audioPlayer.Stop();             //停止台詞聲音
        DialogueBox.SetActive(false);   //關閉對話框
        dialogueNumber = 0;             //重設對話指標(歸零)
        partNumber = 0;                 //重設Part指標(歸零)
    }

    private void RemoveOptionButtons()  //按鈕消除
    {
        if (optionButtonPrefabs == null) { return; }            
        foreach (GameObject optionButton in optionButtonPrefabs)
        {
            GameObject.Destroy(optionButton);

        }
        optionButtonPrefabs.Clear();
    }

    public void RequestChangeDrama(string NextDramaName)            //要求劇本更換(日後可用於對話分支)
    {
        SkipDialogue();                                             //跳過當前劇情
        DialogueManager._dialogueManager.LoadDrama(NextDramaName);  //發送更換劇本要求
        dialoguedata = DialogueManager._dialogueManager.DialougDB;  //更新對話資料
        ResetDialogue();                                            //重置對話
    }

    private void AdjustCharacterPosition(int PNumber)                                           //依據該Part模式調整位置
    {
        #region AdjustCharacterPosition

        switch (dialoguedata.list[PNumber].talkType)                                            //依據Part中的狀態機判斷對話模式
        {
            case DialogueManager.talkingType.LeftRightDouble:                                   //如果是雙人左右對話狀態

                if(PNumber==0)                                                                  //一開始必須先載入下一Part之照片
                {
                    SetDoubleTalkDramaObject(PNumber+1);
                }
                SetDoubleTalkDramaObject(PNumber);
                break;

            case DialogueManager.talkingType.CenterOne:                                                 //如果是單人對話模式
                SetOneTalkDramaObject(PNumber);
                break;

            case DialogueManager.talkingType.Options:                                                  //如果是選項狀態

                SetOptionDramaObject(PNumber);

                break;
        }

        #endregion
    }

    private void SetOptionDramaObject(int PNumber)                                     //選項模式角色照片設定
    {
        for (int i = 0; i < dialoguedata.list[PNumber].dramaOption.Count; i++)
        {
            GameObject newOption = Instantiate(optionButtonPrefab, optionButtonGroup.transform);
            int optionIndex = i;
            newOption.transform.GetChild(0).GetComponent<Text>().text = dialoguedata.list[PNumber].dramaOption[optionIndex];
            newOption.GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    RequestChangeDrama(dialoguedata.list[PNumber].OptionLoadDramaName[optionIndex]);
                }

            );
            optionButtonPrefabs.Add(newOption);
        }
    }

    private void SetDoubleTalkDramaObject(int PNumber)                                  //雙人模式角色照片設定
    {
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
    }

    private void SetOneTalkDramaObject(int PNumber)                                             //單人模式角色照片設定
    {
        characterLImg.GetComponent<RawImage>().color = new Color32(75, 75, 75, 0);              //雙人模式角色圖隱形
        characterRImg.GetComponent<RawImage>().color = new Color32(255, 255, 255, 0);           //雙人模式角色圖隱形
        characterCImg.GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);         //單人模式角色圖顯示
        characterCImg.texture = LoadCharacterImage(dialoguedata.list[PNumber].characterName);   //讀取角色圖
    }

    protected Texture2D LoadCharacterImage(string characterName)                                //角色照片載入(只讀PNG(可改))
    {
        #region LoadImage
        //characterLImg.texture = null;
        //characterRImg.texture = null;

        FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/CharacterPictures/" + characterName + ".png", FileMode.Open, FileAccess.Read);
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

    private IEnumerator LoadAudioFile(string SoundName)                                         //角色照片載入(只讀WAV(我不確定UNITY是不是只讀WAV，但WAV最穩))
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

    private void OnGUI()                                                                        //面板用的GUI按鈕測試
    {
        if (GUILayout.Button("NextLine", GUILayout.Width(300), GUILayout.Height(100))) { dialogueNumber++; LoadCharacterDialogue(); Debug.Log("P:" + partNumber); }
        if (GUILayout.Button("Skip", GUILayout.Width(300), GUILayout.Height(100))) { SkipDialogue(); }
        if (GUILayout.Button("Reset", GUILayout.Width(300), GUILayout.Height(100))) { ResetDialogue(); }
        if (GUILayout.Button("Exit", GUILayout.Width(300), GUILayout.Height(100))) { Application.Quit(); }
    }
}
