using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMovingController : MonoBehaviour                    //NPC面向控制
{

    private string x = "TheXValue";                                //用簡單單字先代表Animator中之參數名
    private string y = "TheYValue";                                //用簡單單字先代表Animator中之參數名


    public void Face_L()
    {
        this.GetComponent<Animator>().SetBool("isMoving", false);       //關閉移動模式、停止NPC行走動畫
        this.GetComponent<Animator>().SetFloat(x,-1);                   //控制方向
        this.GetComponent<Animator>().SetFloat(y,0);
    }
    public void Face_R()
    {
        this.GetComponent<Animator>().SetBool("isMoving", false);
        this.GetComponent<Animator>().SetFloat(x, 1);
        this.GetComponent<Animator>().SetFloat(y, 0);
    }
    public void Face_U()
    {
        this.GetComponent<Animator>().SetBool("isMoving", false);
        this.GetComponent<Animator>().SetFloat(x, 0);
        this.GetComponent<Animator>().SetFloat(y, 1);
    }
    public void Face_D()
    {
        this.GetComponent<Animator>().SetBool("isMoving", false);
        this.GetComponent<Animator>().SetFloat(x, 0);
        this.GetComponent<Animator>().SetFloat(y, -1);
    }

    public void Walk_L()
    {
        this.GetComponent<Animator>().SetBool("isMoving", true);       //開啟移動模式、開啟NPC行走動畫
        this.GetComponent<Animator>().SetFloat(x, -1);
        this.GetComponent<Animator>().SetFloat(y, 0);
    }

    public void Walk_R()
    {
        this.GetComponent<Animator>().SetBool("isMoving", true);
        this.GetComponent<Animator>().SetFloat(x, 1);
        this.GetComponent<Animator>().SetFloat(y, 0);
    }

    public void Walk_U()
    {
        this.GetComponent<Animator>().SetBool("isMoving", true);
        this.GetComponent<Animator>().SetFloat(x, 0);
        this.GetComponent<Animator>().SetFloat(y, 1);
    }

    public void Walk_D()
    {
        this.GetComponent<Animator>().SetBool("isMoving", true);
        this.GetComponent<Animator>().SetFloat(x, 0);
        this.GetComponent<Animator>().SetFloat(y, -1);
    }
}
