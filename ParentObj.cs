using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentObj : MonoBehaviour
{
    [SerializeField]
    protected bool SetOn; //상태를 저장해줄 변수

    public PlayerControl player; //플레이어 위치 받아올 변수.

    protected float objPosX, objPosY;//오브젝트의 포지션값

    /*protected Vector3 ObjRU;       //오브젝트오른쪽위
    protected Vector3 ObjLD;       //오브젝트왼쪽아래

    protected SpriteRenderer spr;
    protected float spriteX, spriteY;*/


    void Start()
    {
        SetOn = false;

        objPosX = transform.position.x;
        objPosY = transform.position.y;

        /*spr = GetComponent<SpriteRenderer>();
        spriteX = spr.sprite.rect.width;
        spriteY = spr.sprite.rect.height;
        */
    }

    // Update is called once per frame
    void Update()
    {
        CheckInRectPCPosition(player.RectRightUp, player.RectLeftDown);
    }

    protected void CheckInRectPCPosition(Vector3 playerRU, Vector3 playerLD)
    {
        if (objPosX < playerRU.x && objPosX > playerLD.x && objPosY < playerRU.y && objPosY > playerLD.y)
        {
            SetOn = true;
           // Debug.Log(SetOn);
        }
        else
        {
            SetOn = false;
            //Debug.Log(SetOn);
        }
    }

    /*protected void SetObjTransform()
    {
        ObjRU.Set(transform.position.x + (spriteX / 20), transform.position.y + (spriteY / 20), transform.position.z);
        ObjLD.Set(transform.position.x - (spriteX / 20), transform.position.y - (spriteY / 20), transform.position.z);
    }

    protected bool CheckInRect(Vector3 playerRU, Vector3 playerLD) //현재 이 오브젝트가 Rect안에 있는지 체크. 플레이어의 우상 좌하를 가져옴
    {
        if (playerRU.x>ObjRU.x&&playerRU.y>ObjRU.y&&playerLD.x<ObjLD.x&&playerLD.y<ObjLD.y)
        {
            SetOn = true;
            Debug.Log(SetOn);
        }
        else { 
        
            SetOn = false;
            Debug.Log(SetOn);
            Debug.Log(ObjRU.x);
        }
        return SetOn;
    }*/
}