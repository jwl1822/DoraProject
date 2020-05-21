using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerThorn : Obj
{
    [SerializeField]
    private bool isTouchingTrigger;
    [SerializeField]
    private float delayTime = 2;

    public Thorn thorn;
    public SpriteRenderer sr;

    [SerializeField]
    private float TriggerCheckDirection = 200.0f;

    public bool triggerOn;

    public float speed = 300.0f;//오브젝트 올라오는 속도

    Vector2 startPosition, finishPosition;//오브젝트 대기위치, 오브젝트 올라온 위치

    public float height = 300.0f;//올라오는 높이 설정

    void Start()
    {
        ObjPosX = this.transform.position.x;
        ObjPosY = this.transform.position.y;

        sr = GetComponent<SpriteRenderer>();
        thorn = GetComponent<Thorn>();
        startPosition = this.transform.position;
        finishPosition = new Vector2(startPosition.x, startPosition.y + height);
        ThronDisappear();
    }

    void Update()
    {
        CheckInRect(obj_player.RectRightUp, obj_player.RectLeftDown);
        isWalk();
    }

    protected override void isWalk()
    {
        //플레이어가 트리거에 닿았는지를 판별
        isTouchingTrigger = Physics2D.Raycast(obj_player.GetComponent<Rigidbody2D>().position, Vector3.down, TriggerCheckDirection, LayerMask.GetMask("Trigger"));

        if (SetOn && isTouchingTrigger && !triggerOn)
        {
            StartCoroutine(ThornAppear());
        }
        else if (triggerOn)
        {
            Vector2 nowposition = transform.position;
            transform.position = Vector3.MoveTowards(nowposition, finishPosition, speed * Time.deltaTime);//현재지점 ,종료지점, 속도
            if (nowposition == finishPosition) { triggerOn = false; }
        }

    }

    void ThronAppear()
    {
        sr.enabled = true;
        thorn.enabled = true;
    }
    void ThronDisappear()
    {
        sr.enabled = false;
        thorn.enabled = false;
    }

    IEnumerator ThornAppear()
    {
        yield return new WaitForSeconds(delayTime);
        //yield return new WaitWhile(() => delayTime > 0);
        triggerOn = true;
        ThronAppear();
    }


}
