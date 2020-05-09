using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj : MonoBehaviour
{
    [SerializeField]
    protected bool SetOn; //상태를 저장해줄 변수
    protected PlayerControl player; //플레이어 위치 받아올 변수.

    protected Vector3 ObjRU;       //오브젝트오른쪽위
    protected Vector3 ObjLD;       //오브젝트왼쪽아래
    protected Vector3 ObjPos;


    protected SpriteRenderer sprite;
    protected float spriteX , spriteY;
    // Start is called before the first frame update
    void Start()
    {
        //여기서 모든 값 초기화
        StartFunc();
    }

    // Update is called once per frame
    void Update()
    {
        //SetObjTransform();
        CheckInRect(player.RectRightUp,player.RectLeftDown);
        Debug.Log(spriteX);
        Debug.Log(ObjRU);
    }

   /* protected void CheckInRect(Vector3 playerRU, Vector3 playerLD) //현재 이 오브젝트가 Rect안에 있는지 체크. 플레이어의 우상 좌하를 가져옴
    {
        if (playerRU.x>ObjRU.x&&playerRU.y>ObjRU.y&&playerLD.x<ObjLD.x&&playerLD.y<ObjLD.y)
        {
            SetOn = true;
        }
        else { 
        
            SetOn = false;
        }
    }*/
    protected void CheckInRect(Vector3 playerRU, Vector3 playerLD) //현재 이 오브젝트가 Rect안에 있는지 체크. 플레이어의 우상 좌하를 가져옴
    {
        if (playerRU.x > ObjPos.x && playerRU.y > ObjPos.y && playerLD.x < ObjPos.x && playerLD.y < ObjPos.y)
        {
            SetOn = true;
        }
        else
        {

            SetOn = false;
        }
    }

    protected void SetObjTransform()
    {
        ObjRU.Set(transform.position.x + (spriteX / 10), transform.position.y + (spriteY / 10), transform.position.z);
        ObjLD.Set(transform.position.x - (spriteX / 10), transform.position.y - (spriteY / 10), transform.position.z);
    }

    protected void StartFunc()
    {
        ObjPos = transform.position;
        sprite = GetComponent<SpriteRenderer>();
        spriteX = sprite.sprite.rect.width;
        spriteY = sprite.sprite.rect.height;
        SetOn = false;
    }

    protected virtual void isWalk()
    {
        Debug.Log("i'm parents");
    }
}