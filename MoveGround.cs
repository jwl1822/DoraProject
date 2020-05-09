using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGround : Obj
{
    public Vector3[] wayPoints;

    private Vector3 nowPosition;
    private int wayPointIndex = 0;
    private float speed = 300.0f;
    bool isGoahead = true;

    private SpriteRenderer ground;
    private float def;

    public PlayerControl Player = null;


    void Start()
    {
        def = 
        wayPoints = new Vector3[3];

        wayPoints[0] = new Vector3(741,-181,0);
        wayPoints[1] = new Vector3(1051, -181, 0);
        wayPoints[2] = new Vector3(1051, -100, 0);
        SetObjTransform();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGoahead)
        {
            isWalk();
            if (Player != null)//이경우 플레이어가 ground 포지션에 끼임.
            {
                Player.transform.position = nowPosition;
                if (wayPointIndex < wayPoints.Length)
                {
                    float step = speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(Player.transform.position, wayPoints[wayPointIndex], step);
                    if (Vector3.Distance(wayPoints[wayPointIndex], Player.transform.position) == 0f)
                    {
                        wayPointIndex++;
                    }
                }
                if (wayPointIndex == wayPoints.Length)
                {
                    isGoahead = false;
                }
            }
        }
        else
        {
            goBack(); 
            if (Player != null)
            {
                Player.transform.position = nowPosition;
                if (wayPointIndex > 0)
                {
                    float step = speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(Player.transform.position, wayPoints[wayPointIndex - 1], step);
                    if (Vector3.Distance(wayPoints[wayPointIndex - 1], Player.transform.position) == 0f)
                    {
                        wayPointIndex--;
                    }
                }
                if (wayPointIndex == 0)
                {
                    isGoahead = true;
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            //Player값을 넣어줌
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            //Player값을 null로 설정
        }
    }

    protected override void isWalk()
    {
      
        nowPosition = transform.position;
        if (wayPointIndex<wayPoints.Length)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(nowPosition, wayPoints[wayPointIndex], step);
            if (Vector3.Distance(wayPoints[wayPointIndex], nowPosition) == 0f)
            {
                wayPointIndex++;
            }
        }
        if (wayPointIndex == wayPoints.Length)
        {
            isGoahead = false;
        }
    }

    private void goBack()
    {
        nowPosition = transform.position;
        if (wayPointIndex > 0)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(nowPosition, wayPoints[wayPointIndex-1], step);
            if (Vector3.Distance(wayPoints[wayPointIndex-1], nowPosition) == 0f)
            {
                wayPointIndex--;
            }
        }
        if (wayPointIndex == 0)
        {
            isGoahead = true;
        }
    }
}
