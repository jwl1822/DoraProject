using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRectCollider : MonoBehaviour
{
    private float startPosX;
    private float startPosY;

    float resolutionX, resolutionY;         //해상도 비례해서 어떤 수가 1로 계산되는 지 알수있음.
    float FirstX, FirstY;                   //캐릭터의 기본좌표

    float changeLayerX, changeLayerY;       //변하기 전과 후의 좌표값을 비교해서 레이어에 넘겨줘야함.
    float preLayerX, preLayerY;             //그때 필요한 변수들
    float defX;
    float defY;
    [SerializeField]
    int MaxR;
    [SerializeField]
    int MinR;

    private bool isBeingHeld = false;
    public bool nowset = false;
    public GameObject Target;
    public PlayerControl player;
    private void Start()
    {
        resolutionX = 1920 / 10;
        resolutionY = 1080 / 10;
        FirstX = player.transform.localPosition.x;
        FirstY = player.transform.localPosition.y;
    }
    private void Update()
    {
        /*
        */
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            isBeingHeld = true;
            //Debug.Log(isBeingHeld);
            MouseTargetRay();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            isBeingHeld = false;
            //Debug.Log(isBeingHeld);
        }

        if (isBeingHeld == true && player.isWallOn == false && !player.getIsJump())
        {
            FirstX = player.transform.localPosition.x;
            FirstY = player.transform.localPosition.y;
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            //Target.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, 0);
            //원래 여기에 무브XY들어감
            if (Target!=null)
            {
                MoveWall(mousePos);
            }
        }
    }
    private void MoveWall(Vector3 mouspos)  //두번째로 만든 작업함수
    {
        switch (Target.tag)
        {
            case "TopWall":
                //startPosY이미 레이함수에서 startPosY 지정, 기존의 preLayerY = Target.transform.position.y;삭제
                preLayerY = (Target.transform.position.y <= 0) ? (Target.transform.position.y * -1) : Target.transform.position.y;

                Target.transform.localPosition = new Vector3(Target.transform.localPosition.x,
                    Mathf.Clamp(mouspos.y - startPosY, FirstY + resolutionY, FirstY + resolutionY * MaxR)
                    , 0);
                changeLayerY = Mathf.Abs(Target.transform.position.y);
                defY = changeLayerY - preLayerY;
                player.SetLayer(player.getLayerMaskX(), defY, Target); 
                break;
            case "BottomWall":
                preLayerY = Target.transform.position.y;
                Target.transform.localPosition = new Vector3(Target.transform.localPosition.x,
                    Mathf.Clamp(mouspos.y - startPosY, FirstY - resolutionY * MaxR, FirstY - resolutionY)
                    , 0);
                changeLayerY = Target.transform.position.y;
                defY = changeLayerY - preLayerY;
                player.SetLayer(player.getLayerMaskX(), defY, Target);
                break;
            case "RightWall":
                preLayerX = (Target.transform.position.x <= 0) ? (Target.transform.position.x * -1) : Target.transform.position.x;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX + resolutionX, FirstX + resolutionX * MaxR)
                , Target.transform.localPosition.y
                , 0);
                changeLayerX = Target.transform.position.x;
                defX = changeLayerX - preLayerX;                //내가 찍은 좌표.
                player.SetLayer(defX, player.getLayerMaskY(), Target);
                break;
            case "LeftWall":
                preLayerX = Target.transform.position.x;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX - resolutionX * MaxR, FirstX - resolutionX)
                    , Target.transform.localPosition.y
                    , 0);
                changeLayerX = Target.transform.position.x;
                defX = changeLayerX - preLayerX;
                player.SetLayer(defX, player.getLayerMaskY(), Target);
                break;



            case "RightDown":
                preLayerX = Target.transform.position.x;
                preLayerY = (Target.transform.position.y <= 0) ? (Target.transform.position.y * -1) : Target.transform.position.y;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX + resolutionX, FirstX + resolutionX * MaxR)
                , Mathf.Clamp(mouspos.y - startPosY, FirstY - resolutionY * MaxR, FirstY - resolutionY)
                , 0);
                changeLayerX = Target.transform.position.x;
                changeLayerY = Target.transform.position.y;
                defX = changeLayerX - preLayerX;
                defY = changeLayerY - preLayerY;
                  player.SetLayer(defX, defY, Target);
                break;
            case "RightUp":
                preLayerX = Target.transform.position.x;
                preLayerY = (Target.transform.position.y <= 0) ? (Target.transform.position.y * -1) : Target.transform.position.y;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX + resolutionX, FirstX + resolutionX * MaxR)
                , Mathf.Clamp(mouspos.y - startPosY, FirstY + resolutionY, FirstY + resolutionY * MaxR)
                , 0);
                changeLayerX = Target.transform.position.x;
                changeLayerY = Target.transform.position.y;
                defX = changeLayerX - preLayerX;
                defY = changeLayerY - preLayerY;
                player.SetLayer(defX, defY, Target);
                break;
            case "LeftUp":
                preLayerX = Target.transform.position.x;
                preLayerY = (Target.transform.position.y <= 0) ? (Target.transform.position.y * -1) : Target.transform.position.y;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX - resolutionX * MaxR, FirstX - resolutionX)
                    , Mathf.Clamp(mouspos.y - startPosY, FirstY + resolutionY, FirstY + resolutionY * MaxR)
                    , 0);
                changeLayerX = Target.transform.position.x;
                changeLayerY = Target.transform.position.y;
                defX = changeLayerX - preLayerX;
                defY = changeLayerY - preLayerY;
                player.SetLayer(defX, defY, Target);
                break;
            case "LeftDown":
                preLayerX = Target.transform.position.x;
                preLayerY = (Target.transform.position.y <= 0) ? (Target.transform.position.y * -1) : Target.transform.position.y;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX - resolutionX * MaxR, FirstX - resolutionX)
                , Mathf.Clamp(mouspos.y - startPosY, FirstY - resolutionY * MaxR, FirstY - resolutionY)
                , 0);
                changeLayerX = Target.transform.position.x;
                changeLayerY = Target.transform.position.y;
                defX = changeLayerX - preLayerX;
                defY = changeLayerY - preLayerY;
                player.SetLayer(defX, defY, Target);
                break;
        }
    }
/*
    private void MoveXY(Vector3 mouspos)
    {
        switch (Target.tag)
        {
            case "TopWall":
                preLayerY = Target.transform.position.y;

                Target.transform.localPosition = new Vector3(Target.transform.localPosition.x,
                    Mathf.Clamp(mouspos.y - startPosY, FirstY + resolutionY, FirstY + resolutionY * 4)
                    , 0);
                changeLayerY =Target.transform.position.y;
                defY = changeLayerY - preLayerY;
                player.SetLayer(player.getLayerMaskX(), defY, Target.gameObject);
                break;
            case "BottomWall":
                preLayerY = Target.transform.position.y;
                Target.transform.localPosition = new Vector3(Target.transform.localPosition.x,
                    Mathf.Clamp(mouspos.y - startPosY, FirstY - resolutionY * 4, FirstY - resolutionY)
                    , 0);
                changeLayerY = Target.transform.position.y;
                defY = changeLayerY - preLayerY;
                player.SetLayer(player.getLayerMaskX(), defY, Target.gameObject);
                break;
            case "LeftWall":
                preLayerX = Target.transform.position.x;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX - resolutionX * 4, FirstX - resolutionX)
                    , Target.transform.localPosition.y
                    , 0) ;
                changeLayerX = transform.position.x;
                defX = changeLayerX - preLayerX;
                player.SetLayer(defX, player.getLayerMaskY(), this.gameObject);
                break;
            case "RightWall":
                preLayerX = Target.transform.position.x;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX + resolutionX, FirstX + resolutionX*4)
                , Target.transform.localPosition.y
                , 0);
                changeLayerX = transform.position.x;
                defX = changeLayerX - preLayerX;
                player.SetLayer(defX, player.getLayerMaskY(), this.gameObject);
                break;
            case "RightUp":
                preLayerX = Target.transform.position.x;
                preLayerY = Target.transform.position.y;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX + resolutionX, FirstX + resolutionX * 4)
                , Mathf.Clamp(mouspos.y - startPosY, FirstY + resolutionY, FirstY + resolutionY * 4)
                , 0);
                changeLayerX = transform.position.x;
                changeLayerY = Target.transform.position.y;
                defX = changeLayerX - preLayerX;
                defY = changeLayerY - preLayerY;
                player.SetLayer(defX, defY, this.gameObject);
                break;
            case "RightDown":
                preLayerX = Target.transform.position.x;
                preLayerY = Target.transform.position.y;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX + resolutionX, FirstX + resolutionX * 4)
                , Mathf.Clamp(mouspos.y - startPosY, FirstY - resolutionY * 4, FirstY - resolutionY)
                , 0);
                changeLayerX = transform.position.x;
                changeLayerY = Target.transform.position.y;
                defX = changeLayerX - preLayerX;
                defY = changeLayerY - preLayerY;
                player.SetLayer(defX, defY, this.gameObject);
                break;
            case "LeftUp":
                preLayerX = Target.transform.position.x;
                preLayerY = Target.transform.position.y;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX - resolutionX * 4, FirstX - resolutionX)
                    , Mathf.Clamp(mouspos.y - startPosY, FirstY + resolutionY, FirstY + resolutionY * 4)
                    , 0);
                changeLayerX = transform.position.x;
                changeLayerY = Target.transform.position.y;
                defX = changeLayerX - preLayerX;
                defY = changeLayerY - preLayerY;
                player.SetLayer(defX, defY, this.gameObject);
                break;
            case "LeftDown":
                preLayerX = Target.transform.position.x;
                preLayerY = Target.transform.position.y;
                Target.transform.localPosition = new Vector3(Mathf.Clamp(mouspos.x - startPosX, FirstX - resolutionX * 4, FirstX - resolutionX)
                , Mathf.Clamp(mouspos.y - startPosY, FirstY - resolutionY * 4, FirstY - resolutionY)
                , 0);
                changeLayerX = transform.position.x;
                changeLayerY = Target.transform.position.y;
                defX = changeLayerX - preLayerX;
                defY = changeLayerY - preLayerY;
                player.SetLayer(defX, defY, this.gameObject);
                break;
               
        }
    }
    */

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
            startPosX = MousePos.x - Target.transform.localPosition.x;
            startPosY = MousePos.y - Target.transform.localPosition.y;
        }
        else
        {
            Target = null;
            nowset = false;
        }

    }

}