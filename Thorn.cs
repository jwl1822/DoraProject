using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorn : Obj
{
    PlayerControl player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //플레이어 hp감소
            Debug.Log("Hp Down");
        }
    }
}
