using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFacePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public GameObject NPC;
    private float udvector;
    private float rfvector;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        udvector = Vector2.Dot(Vector2.up, new Vector2(NPC.transform.position.x - player.transform.position.x, NPC.transform.position.y - player.transform.position.y));
        rfvector = Vector2.Dot(Vector2.right, new Vector2(NPC.transform.position.x - player.transform.position.x, NPC.transform.position.y - player.transform.position.y));
        if (udvector > 0)
        {
            if (Mathf.Abs(udvector) - Mathf.Abs(rfvector) > 0) { NPC.GetComponent<NpcMovingController>().Face_D(); }

        }

        if (udvector < 0)
        {
            if (Mathf.Abs(udvector) - Mathf.Abs(rfvector) > 0) { NPC.GetComponent<NpcMovingController>().Face_U(); }

        }

        if (rfvector > 0)
        {
            if (Mathf.Abs(rfvector) - Mathf.Abs(udvector) > 0) { NPC.GetComponent<NpcMovingController>().Face_L(); }

        }

        if (rfvector < 0)
        {
            if (Mathf.Abs(rfvector) - Mathf.Abs(udvector) > 0) { NPC.GetComponent<NpcMovingController>().Face_R(); }

        }
    }
}
