using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DialogueFunction : MonoBehaviour
{
    public RawImage characterImg;                                               //For Center Mode

    public RawImage characterLImg;                                              //For   L/R  Mode

    public RawImage characterRImg;                                              //For   L/R  Mode

    public GameObject DialogueBox;                                              //The Dialogue Box

    public Text DialogueBoxText;                                                //The Dialogue Box Text

    private int dialogueNumber = 0;                                             //Record Current Dialogue Number

    private int partNumber = 0;                                                 //Record Current Part Number(Change it when switch character)

    private DialogueManager.DialougDataBase dialoguedata;                       //Get dialoguedata for manager(current drama script)

    private void Start()                                                        
    {
        dialoguedata = DialogueManager._dialogueManager.DialougDB;              //Same as the up line.

        AdjustCharacterPosition(1);
        AdjustCharacterPosition(0);

        LoadCharacterDialogue();
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {

            dialogueNumber++; LoadCharacterDialogue(); Debug.Log("P:"+partNumber);
        }  //Click to show the next Dialogue

    }

    Texture2D LoadCharacterImage(string characterName)
    {
        #region LoadImage
        //characterLImg.texture = null;
        //characterRImg.texture = null;

        FileStream fileStream = new FileStream(Application.dataPath + "/StreamingFiles/CharacterPictures/" + characterName + ".png", FileMode.Open,FileAccess.Read);
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

    void AdjustCharacterPosition(int PNumber)
    {
        #region AdjustCharacterPosition
        if (dialoguedata.list[PNumber].talkType == DialogueManager.talkingType.LeftRightDouble)
        {
            if (dialoguedata.list[PNumber].talkPos == DialogueManager.talkingPos.Left)
            {
                characterLImg.texture = LoadCharacterImage(dialoguedata.list[PNumber].characterName);
                characterRImg.GetComponent<RawImage>().color = new Color32(75, 75, 75, 255);
                characterLImg.GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);
            }

            else if (dialoguedata.list[PNumber].talkPos == DialogueManager.talkingPos.Right)
            {
                characterRImg.texture = LoadCharacterImage(dialoguedata.list[PNumber].characterName);
                characterLImg.GetComponent<RawImage>().color = new Color32(75, 75, 75, 255);
                characterRImg.GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);
            }
        }
        #endregion
    }

    void LoadCharacterDialogue()
    {
        if (partNumber+1> dialoguedata.list.Count - 1&& dialogueNumber > dialoguedata.list[partNumber].characterDialoug.Count - 1)
        {
            Debug.Log("The part is ended");
            DialogueBox.SetActive(false);
            return;
        }

        if (dialogueNumber > dialoguedata.list[partNumber].characterDialoug.Count - 1)
        {
            partNumber++;
            Debug.Log("The character's Dialogue is ended");
            AdjustCharacterPosition(partNumber);
            dialogueNumber = 0;
        }

        DialogueBoxText.text = dialoguedata.list[partNumber].characterDialoug[dialogueNumber];
        //Debug.Log(partNumber);
    }
}
