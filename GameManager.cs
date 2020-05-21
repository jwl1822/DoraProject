using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerControl Player;
    Vector3 MousePosition;
    int Zpoint = 10;
    // Start is called before the first frame update
    public bool nowset = false;
    public bool isBeingHeld = false;
    float changeLayerX, changeLayerY;
    float preLayerX, preLayerY;
    float def;
    float chaX;
    public GameObject Target;
    void Start()
    {
   

    }



    // Update is called once per frame
    void Update()
    {
    }
    
    void MouseTargetRay()
    {
        Target = null;

        Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //MousePos에 카메라 기준 좌표로 마우스클릭위치 좌표를 넣어줌.


        //특정 Layer에만 Raycast하기. 여기서는 LightWall Layer만 Raycast하게 처리 해야함. 그렇지 않을 시 마스크에 처리가 됨.
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        RaycastHit2D hit = Physics2D.Raycast(MousePos, Vector2.zero, 0f, layerMask);


        if (hit.collider != null)       //클릭한(충돌한) 좌표에 오브젝트가 있다면
        {
            Target = hit.collider.gameObject;       //타겟에 그 오브젝트를 넣어줌.
            Debug.Log(hit.collider.name);         //타겟 확인용 디버그
            nowset = true;
        }
        else
        {
            Target = null;
            nowset = false;
        }

    }
}

