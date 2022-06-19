using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSortingOrderController : MonoBehaviour
{
    public GameObject Player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.transform.position.y > this.transform.position.y)
        {
            this.GetComponent<SpriteRenderer>().sortingOrder = Player.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }

        if (Player.transform.position.y < this.transform.position.y)
        {
            this.GetComponent<SpriteRenderer>().sortingOrder = Player.GetComponent<SpriteRenderer>().sortingOrder - 1;
        }
    }
}
