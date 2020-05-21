using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mouse : Obj
{
    private Animator anim;

    //일정 거리 움직임
    private Vector3[] wayPoints;
    private int wayPointIndex = 0;
    private Vector3 nowPosition;//속도를 준다고 했을 때 

    //좌우 포인트 설정
    public float leftWayX = 300.0f;
    public float rightWayX = 300.0f;
    
    //움직이는 속도
    [SerializeField]
    private float speed = 300.0f;

    //bool 값
    public bool isGoahead = true;
    public bool isTouchingPC;//플레이어와 쥐의 충돌체크

    void Start()
    {
        ObjPosX = this.transform.position.x;
        ObjPosY = this.transform.position.y;

        anim = GetComponent<Animator>();

        anim.enabled = false;

        wayPoints = new Vector3[3];

        wayPoints[0] = new Vector3(ObjPosX - leftWayX, ObjPosY, 0);  //왼쪽포인트
        wayPoints[1] = new Vector3(ObjPosX, ObjPosY, 0);             //중앙포인트(시작점)
        wayPoints[2] = new Vector3(ObjPosX + rightWayX, ObjPosY, 0); //오른쪽포인트
    }

    void Update()
    {
        CheckInRect(obj_player.RectRightUp, obj_player.RectLeftDown);
        if (SetOn)
        {
            anim.enabled = true;
            if (isGoahead)
            {
                isWalk();
                if (isTouchingPC)
                {
                    //obj_player.PCDie();
                    Debug.Log("플레이어 죽음");
                }
            }
            else
            {
                goBack();
                if (isTouchingPC)
                {
                    //obj_player.PCDie();
                    Debug.Log("플레이어 죽음");
                }
            }
        }
        else
        {
            anim.enabled = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
            isTouchingPC = true;

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
            isTouchingPC = false;

    }

    protected override void isWalk()
    {
        nowPosition = transform.position;
        if (wayPointIndex < wayPoints.Length)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(nowPosition, wayPoints[wayPointIndex], step);//현재지점 ,종료지점, 속도
            if (Vector3.Distance(wayPoints[wayPointIndex], nowPosition) == 0f)
            {
                wayPointIndex++;
            }
        }
        if (wayPointIndex == wayPoints.Length)
        {
            isGoahead = false;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void goBack()
    {
        nowPosition = transform.position;
        if (wayPointIndex > 0)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(nowPosition, wayPoints[wayPointIndex - 1], step);
            if (Vector3.Distance(wayPoints[wayPointIndex - 1], nowPosition) == 0f)
            {
                wayPointIndex--;
            }
        }
        if (wayPointIndex == 0)
        {
            isGoahead = true;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

}
