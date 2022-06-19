using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovingTest : MonoBehaviour

{
    public static PlayerMovingTest _PlayerMovingTest;

    public enum PlayerMovingState
    {
        idle,
        moving,
        autoMoving,
        talkingFreeze,
    }
    public PlayerMovingState playerNowSate;
    public PlayerMovingState PlayerNowSate
    {
        get { return playerNowSate; }
        set { playerNowSate = value; }
    }
    private bool PlayerMoving;
    private Animator NachelleAnim;
    public Vector2 LastMove;
    private Vector2 OldPos;
    private Vector2 currentPos;

    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    private float moveSpeed;
    private float walkSpeed = 3;
    private float runSpeed = 5;

    private float XAxisSpeed = 0;
    private float YAxisSpeed = 0;

    public void ChangeToNextState(PlayerMovingState nextState)
    {
        if (playerNowSate == nextState) { return; }

        playerNowSate = nextState;
        switch (nextState)
        {
            case PlayerMovingState.idle:
                break;
            case PlayerMovingState.moving:
                break;
            case PlayerMovingState.talkingFreeze:

                break;
            case PlayerMovingState.autoMoving:
                //ResetAnimParameter();
                break;
        
        }
    }

    private void Awake()
    {
        
        _PlayerMovingTest = this;
        playerNowSate = PlayerMovingState.moving;
    }

    void Start()
    {
        moveSpeed = walkSpeed;
        NachelleAnim = this.gameObject.GetComponent<Animator>();
        ChangeToN();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.Z)) { ChangeToN(); }
        //if (Input.GetKeyDown(KeyCode.X)) { ChangeToD(); }
        //transform.Translate(0f, 0.5f * moveSpeed * Time.deltaTime, 0f);

        PlayerMoving = false;
        CAutoMoving();
        CharacterMoving();
    }
    private void Update()
    {
        //Debug.Log(playerNowSate);
        
        SetAnimParameter();
        
        
    }

    void CAutoMoving()
    {
        if (playerNowSate != PlayerMovingState.autoMoving) { return; }

        //Debug.Log(currentPos);

        currentPos = transform.TransformPoint(Vector2.zero);
   
        PlayerMoving = true;
       
        XAxisSpeed = currentPos.x - OldPos.x < 0 ?  -1 : 1;
        YAxisSpeed = currentPos.y - OldPos.y < 0 ?  -1 : 1;
        
        if (currentPos.x - OldPos.x == 0) { XAxisSpeed = 0; }
        if (currentPos.y - OldPos.y == 0) { YAxisSpeed = 0; }

        if (currentPos-OldPos!=Vector2.zero) { LastMove.x = XAxisSpeed; LastMove.y = YAxisSpeed; }
        if (currentPos-OldPos==Vector2.zero) { PlayerMoving = false;}

        OldPos = this.transform.position;
        //Debug.LogWarning(OldPos);
    }

    void CharacterMoving()
    {
        //Debug.Log(playerNowSate);
        if (playerNowSate!=PlayerMovingState.moving) { return; }

        if (Input.GetKey(KeyCode.LeftShift)) { moveSpeed = runSpeed; NachelleAnim.speed = 2.5f; }
        else{ moveSpeed = walkSpeed; NachelleAnim.speed = 1; }

        if (Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f)
        {
            
            
            transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime, 0f, 0f));

            PlayerMoving = true;

            

            LastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
        }

        if (Input.GetAxisRaw("Vertical") > 0.5f || Input.GetAxisRaw("Vertical") < -0.5f)
        {


            transform.Translate(new Vector3(0f, Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime, 0f));

            PlayerMoving = true;

            

            LastMove = new Vector2(0f, Input.GetAxisRaw("Vertical"));
        }
    }
    void SetAnimParameter()
    {
        if (playerNowSate == PlayerMovingState.moving)
        {
            NachelleAnim.SetFloat("MoveX", Input.GetAxisRaw("Horizontal"));
            NachelleAnim.SetFloat("MoveY", Input.GetAxisRaw("Vertical"));
        }

        if (playerNowSate == PlayerMovingState.autoMoving)
        {
            NachelleAnim.SetFloat("MoveX", XAxisSpeed);
            NachelleAnim.SetFloat("MoveY", YAxisSpeed);
        }
        
            NachelleAnim.SetBool("PlayerMoving", PlayerMoving);

        if (playerNowSate != PlayerMovingState.talkingFreeze)
        {
            NachelleAnim.SetFloat("LastMoveX", LastMove.x);
            NachelleAnim.SetFloat("LastMoveY", LastMove.y);
        }
        
    }

    public void ChangeToN()
    {
        //this.GetComponent<SpriteRenderer>().color = new Color(0.62f, 0.62f, 0.62f, 1);
        this.GetComponent<Animator>().Play("NachelleFace");
    }

    public void ChangeToD()
    {
        this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        this.GetComponent<Animator>().Play("NachelleFace(Day)");
    }

    public void NfaceDown()

    {
        PlayerMoving = false;
        LastMove = new Vector2(0,-1);
        NachelleAnim.SetFloat("LastMoveX",0);
        NachelleAnim.SetFloat("LastMoveY", -1);

    }
    public void NfaceUp()
    {
        PlayerMoving = false;
        LastMove = new Vector2(0, 1);
        NachelleAnim.SetFloat("LastMoveX", 0);
        NachelleAnim.SetFloat("LastMoveY", 1);
    }
    public void NfaceLeft()
    {
        PlayerMoving = false;
        LastMove = new Vector2(-1, 0);
        NachelleAnim.SetFloat("LastMoveX", -1);
        NachelleAnim.SetFloat("LastMoveY", 0);
    }
    public void NfaceRight()
    {
        PlayerMoving = false;
        LastMove = new Vector2(1, 0);
        NachelleAnim.SetFloat("LastMoveX", 1);
        NachelleAnim.SetFloat("LastMoveY", 0);
    }

    public void resetParameter()
    {
        XAxisSpeed = 0;
        YAxisSpeed = 0;
        LastMove = Vector2.zero;
    }

    //public void AutoMoveSC() { StartCoroutine(ChangeToAutoMove()); }
    /*IEnumerator ChangeToAutoMove()
    {
        yield return new WaitUntil(() => DialogueFunction._dialogueFunction.NowState== DialogueFunction.DramaPlayingState.dramaclosing);
		Debug.Log("AUTO movingTest");
		ChangeToNextState(PlayerMovingState.autoMoving);
    }*/

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "MapID")
        {
            //Para_nachelle.nowMap = collider.gameObject.name.Replace("ID ","");
            

            //NowMap nowMapTrigger = collider.GetComponent<NowMap>();

            /*switch (nowMapTrigger.setPlayerColor)
            {
                case "D":
                    ChangeToD();
                    break;
                case "N":
                    ChangeToN();
                    break;
            }*/

            //Debug.Log("Move to " + Para_nachelle.nowMap);
        }

    }
}
