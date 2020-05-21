using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGround : Obj
{
    public Vector3[] wayPoints;

    private Vector3 nowPosition;//속도를 준다고 했을 때 
    private int wayPointIndex = 0;
    private float speed = 300.0f;
    bool isGoahead = true;

    //1. 플레이어에게 velositiy값 넣어주기. 발상은 아래링크에서. 
    //https://answers.unity.com/questions/736531/moving-rigidbodies-2d-with-moving-platforms.html
    //2. 문제점이 많았음(fixed에서 돌려서 싱크도 안맞고 애니메이션도 안맞고 등등등,,,) 
    //3. 그래서 플레이어 포지션 값에 박스가 움직이는 만큼의 포지션값을 더해주니까 문제 해결함.
    //만약에 충돌처리를 OnCollider가 아니고 레이캐스트로 주면 layermask값을 특정 값("moveGround" 이런,,,)으로 변경해야 되는데
    //그렇게 되면 ground기반으로 제작된 점프 등 플레이어 컨트롤 관련 부분을 수행하지 못한다. 더 배워서 해결할 수 있으면,,, 할 ,,예정,,,

    public bool isPCTouchingMoveGround;//플레이어와 땅과의 충돌체크

    Vector2 PcPositon;//플레이어 포지션값을 받아오고 입력하기 위한 변수

    void Start()
    {
        ObjPosX = this.transform.position.x;
        ObjPosY = this.transform.position.y;

        wayPoints = new Vector3[3];

        wayPoints[0] = new Vector3(ObjPosX, ObjPosY, 0);
        wayPoints[1] = new Vector3(ObjPosX+500, ObjPosY, 0);
        wayPoints[2] = new Vector3(ObjPosY+1000, ObjPosY+1000, 0);
    }

    void Update()
    {
        CheckInRect(obj_player.RectRightUp, obj_player.RectLeftDown);
        if (SetOn)
        {
            if (isGoahead)
            {
                isWalk();
                if (isPCTouchingMoveGround)
                {
                    AddPlayerPosition_Front();
                }
            }
            else
            {
                goBack();
                if (isPCTouchingMoveGround)
                {
                    AddPlayerPosition_Back();
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
            isPCTouchingMoveGround = true;

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
            isPCTouchingMoveGround = false;

    }

    void AddPlayerPosition_Front()
    {
        PcPositon = obj_player.transform.position;
        if (isGoahead && nowPosition.x < wayPoints[wayPointIndex].x)// 왼쪽에서 오른쪽으로 움직일때
        {
            PcPositon.x += speed * Time.deltaTime;
        }
        else if (isGoahead && nowPosition.y < wayPoints[wayPointIndex].y)//아래에서 위로 움직일때
        {
            PcPositon.y += speed * Time.deltaTime;
        }
        obj_player.transform.position = PcPositon;
    }

    void AddPlayerPosition_Back()
    {
        PcPositon = obj_player.transform.position;
        if (!isGoahead && nowPosition.y < wayPoints[wayPointIndex].y)//위에서 아래로 내려갈때
        {
            PcPositon.y -= speed * Time.deltaTime;
        }
        else if (!isGoahead && nowPosition.x < wayPoints[wayPointIndex].x)//오른쪽에서 왼쪽으로 움직일때//(!Goahead && nowPosition.x>wayPoints[wayPointIndex].x)원래 포지션 비교값이 이런 식이어야 하는데 goBack에서 인덱스 값에 -1을 하는 바람에,,,
        {
            PcPositon.x -= speed * Time.deltaTime;
        }
        obj_player.transform.position = PcPositon;
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
        }
    }
}
