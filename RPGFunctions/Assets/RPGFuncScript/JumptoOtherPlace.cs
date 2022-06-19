using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumptoOtherPlace : MonoBehaviour       //人物跳躍腳本
{
    public GameObject Player;                       //人物(玩家)
    public GameObject MainCamera;                   //跟隨玩家之照相機
    public GameObject playerJumpPoint;              //玩家要跳躍到的位置
    public GameObject cameraJumpPoint;              //照相機要跳躍到的位置(用於照相機並不是榜定在玩家身上的特殊情況)
    public GameObject monsterJumpPoint;             //怪物、NPC跳躍位置(用於跟隨、追逐使用)
    public GameObject monster;                      //怪物(也能放NPC)
    public bool isNeedClick;                        //用來檢查是否要先點及特定按鍵(預設Z)，才能進行跳躍
    public bool isJumpFade;                         //跳躍時，是否需要淡入淡出特效

    public bool isFadeON;                           //淡入淡出是否開啟
    public GameObject FadeObj;                      //淡入淡出物件
    private float FadeObja = 0;                     //控制淡入淡出黑幕透明度
    private bool isNextPlace = false;               //是否到下一地點
    private bool isfade = false;                    //是否開始淡入

    public bool isFaceNeed;                         //是否需要特定面向
    public PlayerFacing needFacing;                 //用於儲存特定面向

    public bool isTurn;                             //在移動到目標後是否需要轉到特定面向
    public PlayerFacing TurnPosition;               //用於儲存特定面向

    private PlayerFacing nowFacing;                 //角色現在面向
    public DoorType theDoorType;                    //如果互動物件是門、是什麼門

    public enum PlayerFacing
    {
        front,              //朝下(面向玩家)
        back,               //朝上(背對玩家)
        left,               //朝左
        right               //朝右
    }
    public enum DoorType
    {
        normal,                     //普通門
        secondfloor,                //二頭大門
        basement,                   //地下室
    }
    public bool isDoor;                             //此腳本是不是掛在門類型的物件上
    public bool isToBaseMent;                       //特殊情況使用
    public bool isOutBaseMent;                      //特殊情況使用
    public bool isDay;                              //特殊情況使用
    public bool isToD6NRoom;                        //特殊情況使用
    private bool DoorOpenSoundIsPlayed;             //特殊情況使用

    public AudioSource bgmPlayer;                   //聲音控制相關
    private AudioClip bgmClip;
    public AudioSource fxPlayer;
    private AudioClip fxClip;

    private Vector2[] facing = new Vector2[4];      //面向相關

    void Start()
    {
        facing[0] = new Vector2(0, -1);
        facing[1] = new Vector2(0, 1);
        facing[2] = new Vector2(-1, 0);
        facing[3] = new Vector2(1, 0);

    }

    // Update is called once per frame
    void Update()
    {
        switch (PlayerMovingTest._PlayerMovingTest.LastMove.x)      //面向判定(透過Animator)
        {
            case 0:
                break;
            case 1:
                nowFacing = PlayerFacing.right;
                break;
            case -1:
                nowFacing = PlayerFacing.left;
                break;
        }
        switch (PlayerMovingTest._PlayerMovingTest.LastMove.y)
        {
            case 0:
                break;
            case 1:
                nowFacing = PlayerFacing.back;
                break;
            case -1:
                nowFacing = PlayerFacing.front;
                break;
        }

        if (isfade == true) { FadeToDark(); }               //淡入控制
        if (isNextPlace == true)                            //淡入到一定程度啟動，換為淡出，可透過不讓此Bool為True達到暫時不淡出效果
        {
            PlayerMovingTest._PlayerMovingTest.ChangeToNextState(PlayerMovingTest.PlayerMovingState.talkingFreeze);
            FadeToLight();

        }
        //Debug.Log(nowFacing);
    }
    void MoveToGameRoom(Vector3 TargetPosition, Vector3 CameraPosition, Vector3 MonsterPosition)        //移動控制(包含玩家、怪獸、照相機位置的移動)
    {
        MainCamera.SetActive(false);
        Player.transform.position = TargetPosition;      
        MainCamera.transform.position = CameraPosition;
        MainCamera.SetActive(true);
        if (monster.activeSelf) { monster.transform.position = MonsterPosition; }
    }
    private void OnTriggerEnter2D(Collider2D collision)                                                 //單次觸發(暫時廢棄，因為只做一次判斷有時會造成無法成功觸發)
    {
        /*if (isNeedClick == true) { return; }
        
        if(isJumpFade == true && nowFacing == needFacing || (isFaceNeed == false && isJumpFade == true))
        {
            Player.GetComponent<BoxCollider2D>().enabled = false;
            isfade = true;
            PlayerMovingTest._PlayerMovingTest.ChangeToNextState(PlayerMovingTest.PlayerMovingState.talkingFreeze);
            
        }
        else if (isFaceNeed == true && nowFacing == needFacing || (isFaceNeed == false && isJumpFade == false))
        {
            MoveToGameRoom(playerJumpPoint.transform.position, cameraJumpPoint.transform.position, monsterJumpPoint.transform.position);
        }*/

    }
    private void OnTriggerStay2D(Collider2D collision)                                                                  //持續觸發
    {
        /*----*/
        if (isNeedClick == false)                                                                                       //如果已進觸發範圍不需要點擊
        {
            if (isJumpFade == true && nowFacing == needFacing || (isFaceNeed == false && isJumpFade == true))           //條件判斷
            {
                Player.GetComponent<BoxCollider2D>().enabled = false;                                                   //暫時關閉玩家碰撞(避免移動不完全或重複觸發)
                isfade = true;                                                                                          //淡入開啟
                PlayerMovingTest._PlayerMovingTest.ChangeToNextState(PlayerMovingTest.PlayerMovingState.talkingFreeze); //更改玩家狀態

            }
            else if (isFaceNeed == true && nowFacing == needFacing || (isFaceNeed == false && isJumpFade == false))     //如果不需要淡入
            {
                MoveToGameRoom(playerJumpPoint.transform.position, cameraJumpPoint.transform.position, monsterJumpPoint.transform.position);
            }
            return;

        }
        /*----*/
        if (isFaceNeed == true && nowFacing == needFacing || isFaceNeed == false)                                   //如果需要點擊按鈕
        {
            if (Input.GetKey(KeyCode.Z))//GetKeyDownBefore
            {
                if (isfade == true) { return; }
                if (isDoor==true && DoorOpenSoundIsPlayed==false)                                                   //如果是門(因為門一定要點擊，所以擺這)
                {
                    switch (theDoorType)
                    {
                        case DoorType.normal:
                            StartCoroutine(SoundManager._SoundManager.LoadAudioFile("NormalOpendoor", fxPlayer, fxClip));
                            break;
                        case DoorType.secondfloor:
                            StartCoroutine(SoundManager._SoundManager.LoadAudioFile("2FDoor", fxPlayer, fxClip));
                            break;
                        case DoorType.basement:
                            StartCoroutine(SoundManager._SoundManager.LoadAudioFile("BaseMentDoor", fxPlayer, fxClip));
                            break;
                    }
                    DoorOpenSoundIsPlayed = true;
                }
                //if (isfade == true) { return; }
                if (isFadeON == false)
                {
                    MoveToGameRoom(playerJumpPoint.transform.position, cameraJumpPoint.transform.position, monsterJumpPoint.transform.position);
                }
                else
                {
                    // Start fade
                    Player.GetComponent<BoxCollider2D>().enabled = false;
                    isfade = true;
                    PlayerMovingTest._PlayerMovingTest.ChangeToNextState(PlayerMovingTest.PlayerMovingState.talkingFreeze);
                }
            }
        }
        
    }

    void FadeToDark()//淡入控制
    {
        try
        {
            //monster.GetComponent<MonsterMove>().monsterStop = true;
        }
        catch
        {
            //Debug.Log("No Monster");
        }
     
        PlayerMovingTest._PlayerMovingTest.ChangeToNextState(PlayerMovingTest.PlayerMovingState.talkingFreeze);
        FadeObja = FadeObja + Time.deltaTime * 1f;
        FadeObj.GetComponent<Image>().color = new Color(0, 0, 0, FadeObja);

        if (FadeObja > 1)
        {
            isNextPlace = true;
            MoveToGameRoom(playerJumpPoint.transform.position, cameraJumpPoint.transform.position, monsterJumpPoint.transform.position);
            PlayerMovingTest._PlayerMovingTest.ChangeToNextState(PlayerMovingTest.PlayerMovingState.talkingFreeze);
            if (isTurn == true)
            {
                switch (TurnPosition)
                {
                    case PlayerFacing.back:
                        PlayerMovingTest._PlayerMovingTest.NfaceUp();
                        break;
                    case PlayerFacing.front:
                        PlayerMovingTest._PlayerMovingTest.NfaceDown();
                        break;
                    case PlayerFacing.left:
                        PlayerMovingTest._PlayerMovingTest.NfaceLeft();
                        break;
                    case PlayerFacing.right:
                        PlayerMovingTest._PlayerMovingTest.NfaceRight();
                        break;
                }
            }
            isfade = false;

            if (isToBaseMent == true)
            {
                StartCoroutine(SoundManager._SoundManager.LoadAudioFile("BaseMent", bgmPlayer, bgmClip));
            }

            if (isOutBaseMent == true)
            {
                if (isDay == true) { StartCoroutine(SoundManager._SoundManager.LoadAudioFile("1FHallDay", bgmPlayer, bgmClip)); }
                if (isDay == false) { StartCoroutine(SoundManager._SoundManager.LoadAudioFile("night", bgmPlayer, bgmClip)); }

            }
            if (isToD6NRoom == true)
            {
                StartCoroutine(SoundManager._SoundManager.LoadAudioFile("2FNight_D6NRoom", bgmPlayer, bgmClip));
            }

        }
    }
    void FadeToLight()//淡出控制
    {
        
        FadeObja = FadeObja - Time.deltaTime * 0.8f;
        FadeObj.GetComponent<Image>().color = new Color(0, 0, 0, FadeObja);
        PlayerMovingTest._PlayerMovingTest.ChangeToNextState(PlayerMovingTest.PlayerMovingState.talkingFreeze);
        if (FadeObja < 0)
        {
            isNextPlace = false;
            Player.GetComponent<BoxCollider2D>().enabled = true;
            DoorOpenSoundIsPlayed = false;
            try
            {
                //monster.GetComponent<MonsterMove>().monsterStop = false;
            }
            catch
            {
                Debug.Log("No Monster");
            }

        
            FadeObj.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            PlayerMovingTest._PlayerMovingTest.ChangeToNextState(PlayerMovingTest.PlayerMovingState.moving);
        }
        //FadeObj.SetActive(false);
    }
}
