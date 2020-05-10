using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj : MonoBehaviour
{
    [SerializeField]
    protected bool SetOn;

    [SerializeField]
    protected PlayerControl obj_player;

    protected float ObjPosX, ObjPosY;

    void Start()
    {
        SetOn = false;

        ObjPosX = transform.position.x;
        ObjPosY = transform.position.y;
    }
    void Update()
    {
        //CheckInRect(obj_player.RectRightUp, obj_player.RectLeftDown);
    }

    protected void CheckInRect(Vector3 PlayerRU, Vector3 PlayerLD)
    {
        if (ObjPosX < PlayerRU.x && ObjPosX > PlayerLD.x && ObjPosY < PlayerRU.y && ObjPosY > PlayerLD.y)
        {
            SetOn = true;
        }
        else
        {
            SetOn = false;
        }
    }

    protected virtual void isWalk()
    {
        Debug.Log("im parent");
    }
}
