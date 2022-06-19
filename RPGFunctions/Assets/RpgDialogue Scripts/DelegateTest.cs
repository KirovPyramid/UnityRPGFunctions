using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateTest : MonoBehaviour
{
    delegate void Mydelegate(int num);

    Mydelegate mydelegate;

    void Start()
    {
        mydelegate += AOI;
        mydelegate += AHOY;
        mydelegate(50);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AOI(int num) { Debug.Log(num*2); }

    public void AHOY(int num) { Debug.Log(num * 3); }

}
