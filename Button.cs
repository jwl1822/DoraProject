using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Obj
{
    [SerializeField]
    private bool isTouchingButton = false;
    [SerializeField]
    private float delayTime = 10;
    [SerializeField]
    private bool isButtonWorking = false;

    private float ButtonCheckDirection = 50.0f;
    public GameObject box;

    void Start()
    {
        ObjPosX = this.transform.position.x;
        ObjPosY = this.transform.position.y;
    }

    void Update()
    {
        CheckInRect(obj_player.RectRightUp, obj_player.RectLeftDown);
        isWalk();
    }

    protected override void isWalk()
    {
        //플레이어와 버튼이 충돌하고 있을 때
        //isTouchingButton = true;
        //이때 버튼 오브젝트 colider2D값을 trigger로 줘야함
        isTouchingButton = Physics2D.Raycast(obj_player.GetComponent<Rigidbody2D>().position, Vector3.down, ButtonCheckDirection, LayerMask.GetMask("Button"));

        if (SetOn == true && isTouchingButton)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))//shift키를 누르면
            {
                delayTime = 10;
                isButtonWorking = true;
                StartCoroutine(MakeGroundMove(box));//작동한다.
            }
        }

        if (SetOn && isButtonWorking)
        {
            delayTime -= Time.deltaTime;
        }
    }
    IEnumerator MakeGroundMove(GameObject box)
    {
        box.GetComponent<MoveGround>().enabled = true;
        Debug.Log("MoveGround 활성화");

        yield return new WaitWhile(() => delayTime > 0);

        box.GetComponent<MoveGround>().enabled = false;
        Debug.Log("MoveGround 비활성화");

        isButtonWorking = false;
    }

}
