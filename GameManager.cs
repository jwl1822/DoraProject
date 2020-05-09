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
        Debug.Log(this.gameObject.transform.position.x);
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
    void inputMouseWall_1()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Input.mousePosition;
            worldPoint.z = Zpoint;
            worldPoint = Camera.main.ScreenToWorldPoint(worldPoint);

            Vector3 diffPos = worldPoint - MousePosition;
            diffPos.z = 0f;

            MousePosition = Input.mousePosition;
            MousePosition.z = Zpoint;
            MousePosition = Camera.main.ScreenToWorldPoint(MousePosition);
            MouseTargetRay();
            if (nowset)
            {
                switch (Target.tag)
                {
                    case "TopWall":
                        Debug.Log("in Top!!!!");
                        //여기서부터 
                        preLayerY = (transform.position.y <= 0) ? (transform.position.y * -1) : transform.position.y;

                        transform.position = new Vector3(transform.position.x,
                           Mathf.Clamp(transform.position.y + diffPos.y, Player.transform.position.y + (Player.LightRadius.y / 2), Player.transform.position.y + ((Player.LightRadius.y / 2) + Player.LightRadius.y))
                           , transform.position.z);

                        changeLayerY = Mathf.Abs(transform.position.y);
                        def = changeLayerY - preLayerY;

                        Player.SetLayer(Player.getLayerMaskX(), def, this.gameObject);

                        //여기까지가 레이어 마스크 크기조절
                        break;
                    case "BottomWall":

                        preLayerY = Mathf.Abs(transform.position.y);
                        transform.position = new Vector3(transform.position.x,
                            Mathf.Clamp(transform.position.y + diffPos.y, Player.transform.position.y - ((Player.LightRadius.y / 2) + Player.LightRadius.y), Player.transform.position.y - (Player.LightRadius.y / 2))
                            , transform.position.z);

                        changeLayerY = (transform.position.y < 0) ? transform.position.y : (transform.position.y * -1);

                        def = changeLayerY + preLayerY;

                        Player.SetLayer(Player.getLayerMaskX(), def, this.gameObject);

                        break;
                    case "LeftWall":                                                                                                             //늘일때    //늘일때  //줄일때
                        preLayerX = transform.position.x;                                                                                       //-10        12        -10


                        transform.position = new Vector3(Mathf.Clamp(transform.position.x + diffPos.x, Player.transform.position.x - (Player.LightRadius.x * 3), Player.transform.position.x - Player.LightRadius.x)
                        , transform.position.y, transform.position.z);

                        changeLayerX = transform.position.x;                                                                                    //-14       8          -6

                        def = changeLayerX - preLayerX;                                                                                          //-4        -4         4
                        Player.SetLayer(def, Player.getLayerMaskY(), this.gameObject);
                        Debug.Log("Transform X : " + transform.position.x + " Player.Transform X :" + Player.transform.position.x);
                        break;



                    case "RightWall":                                                                                                                   //늘일때    //늘일때  //줄일때


                        preLayerX = (transform.position.x <= 0) ? (transform.position.x * -1) : transform.position.x;                                    //10            -6(6)    -10


                        transform.position = new Vector3(Mathf.Clamp(transform.position.x + diffPos.x, Player.transform.position.x + Player.LightRadius.x, Player.transform.position.x + (Player.LightRadius.x * 3))
                           , transform.position.y, transform.position.z);
                        changeLayerX = Mathf.Abs(transform.position.x);                                                                                //13(13)        -3(3)    -13

                        def = changeLayerX - preLayerX;                                                                                                  //3             3  
                        Player.SetLayer(def, Player.getLayerMaskY(), this.gameObject);

                        Debug.Log("Transform X : " + transform.position.x + " Player.Transform X :" + Player.transform.position.x);
                        break;
                }
                Target = null;
            }
        }
    }

  
  
}

