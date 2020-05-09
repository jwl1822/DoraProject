using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonObj : ParentObj
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
        objPosX = transform.position.x;
        objPosY = transform.position.y;
    }
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
            isTouchingButton = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player")
            isTouchingButton = false;
    }*/
    void Update()
    {
        CheckInRectPCPosition(player.RectRightUp, player.RectLeftDown);
        ButtonUp();
    }

    void ButtonUp()
    {
        //플레이어와 버튼이 충돌하고 있을 때
        //isTouchingButton = true;
        //이때 버튼 오브젝트 colider값 trigger로 줘야함
        isTouchingButton = Physics2D.Raycast(player.GetComponent<Rigidbody2D>().position, Vector3.down, ButtonCheckDirection, LayerMask.GetMask("Button"));

        if (SetOn == true && isTouchingButton)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))//shift키를 누르면
            {
                delayTime = 10;
                isButtonWorking = true;
                StartCoroutine(MakeGroundMove(box));//작동한다.
            }
        }

        if(SetOn && isButtonWorking)
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
