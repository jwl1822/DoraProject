using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private float movementInputDirection;
    public float movementSpeed = 10.0f;
    public float JumpForce = 10.0f;
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isBottomWall;
    private bool canJump;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isOverlapRect;
    private Rigidbody2D rb;

    private Animator anim;

    private SpriteRenderer sprite;

    [SerializeField]
    private float variableJumpHeightMultiplier = 0.5f;
    public float groundCheckRadius;
    public float wallCheckDistance; //벽체크 레이 길이
    public float wallSlideSpeed;    //벽에서 미끄러지는 속도
    public float OverlapCheckDistance;//오버랩체크 길이
    public float wallHopForce;
    public float wallJumpForce;//벽 점프 힘
    public float movementForceInAir;//공중에서 힘을 주는 변수
    public float airDragMultiplier = 0.95f;

    private int amountOfJumps = 1;  //점프가능횟수
    private int amountOfJumpsLeft;
    private int facingDirection = 1;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Vector3 LightRadius;
    private Vector3 def;

    //상하좌우 좌표
    public Vector3 RectUp = new Vector3();
    public Vector3 RectDown = new Vector3();
    public Vector3 RectLeft = new Vector3();
    public Vector3 RectRight = new Vector3();
    //대각선좌표 
    public Vector3 RectLeftDown = new Vector3();
    public Vector3 RectRightDown = new Vector3();
    public Vector3 RectLeftUp = new Vector3();
    public Vector3 RectRightUp = new Vector3();

    //레이어계산시 이전의 값을 기억해줄 값.
    Vector3 startLayerScale;
    Vector3[] startRectScale = new Vector3[4];


    public Transform groundCheck;
    public Transform wallCheck;
    public Transform OverlapCheck;

    public LayerMask WhatIsGround;
    public LayerMask WhatIsOverlap;
    public LayerMask WhatIsBottom;

    public GameObject Target;
    public GameObject layerMask;
    public GameObject[] RectColliderPos = new GameObject[4];        //빛사각형에 충돌체크해줄 사각형4개,1부터 상하좌우
    public GameObject[] Rectdiagonal = new GameObject[4];           //대각선쪽 사각형4개
    public bool isWallOn = true;
    public bool isChangeWallOn = false;
    public bool isMoveWall = true;
    public bool initial = false;         //true면 이전의 벽이동 위치를 기억하고 벽이 따라다님. false면 초기화된 상태로 따라다님.

    private float FrameSizeX;
    private float FrameSizeY;

    public CheckPoint reCheckPoint;


    //바뀐요소
    //ifCanJump() 에 상단 if 문 2개 추가
    //checkInput() 에 coyoteJump(), JumpBuffering() 추가
    //Jumo() 에서 isJumping 불리언 값 할당
    //ApplyMovement() 에 맨 위 조건식에 isOnSlope, isJumping 을 추가 / else if 문 1개 추가
    //FixedUpdate()에 SlopeCheck() 추가

    //밑에는 구현하면서 추가한 변수들


    //경사 이동에 쓴 변수들
    private CapsuleCollider2D cc;
    private Vector2 coliderSize;
    public float slopeCheckDistance;//raycasthit을 계산하는 막대의 길이,, 인데 막대 크기가 변하지 않는 이유는 무엇일까.
    private Vector2 slopeNormoalPerp;//캐릭터 기준으로 y축에 해당하는 down 인 hit 레이케스트에 대한 수직 막대 표현을 위해 넣음.

    private float slopeDownAngle;//캐릭터 기준으로 y축에 해당하는 down 인 hit 레이케스트
    private float slopeDownAngleOld;//update이전의 angle값. 위 DownAngle값과 비교하여 isOnSlope값 결정
    private float slopeSideAngle;
    public float maxSlopeAngle;//올라갈 수 있는 경사 허용 범위 설정. 사용가능 미지수.

    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D fullFriction;//경사에서 캐릭터가 미끄러지지 않도록

    private bool isOnSlope;//경사에 있는 중인지 확인
    private bool isJumping;//점프하는 중인지 확인
    private bool canWalkOnSlope;
    private bool isHit;


    //코요테 타임에 쓴 변수들
    public int coyote_counter = 0;
    public int coyote_max = 6;

    //버퍼에 쓴 변수들
    public int buffer_counter = 0;
    public int buffer_max = 4;


    //jump inspector 요구사항
    public float MinimumJumpPower = 0.5f;
    public float MaximumJumpPower = 1.5f;

    //frameCoroutine
    private float frameInitTime = 0.0f;
    Coroutine FrameCoroutine;

    //climb
    public Transform ledgeCheck;
    private bool isTouchingLedge;
    [SerializeField]
    private bool canClimbLedge = false;
    [SerializeField]
    private bool ledgeDetected;

    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;

    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;

    private void Start()
    {
        //reCheckPoint = GetComponent<CheckPoint>();
        cc = GetComponent<CapsuleCollider2D>();
        coliderSize = cc.size;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
        //레이어 마스크 크기 할당
        InitLayer();

        //충돌체크할 라이팅레이어 초기화
        InitializeLight();
        //처음 충돌체크할 빛의 벽 위치 설정.
        OnRectCheck();
        InitializeRect();
    }

    private void Update()
    {

        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallsliding();

        CheckLedgeClimb();

        //레이 테스트
        MoveLayer();
        InitializeLight();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurrounding();
        SlopeCheck();//물리적 영역이라 이곳에 넣는 것 같다.
    }

    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;
            if (isFacingRight)
            {//floor 는 정수값으로 내림하는 함수
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);//x로 wallCheckdirection만큼 오른쪽, ledgeClimb만큼 왼쪽. ledgeclimb만큼 위로 
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            else
            {//ceil는 정수값으로 올림하는 함수
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }

            anim.SetBool("canClimbLedge", canClimbLedge);
            Debug.Log("애니메이션 작동");
        }
        if (canClimbLedge)
        {
            transform.position = ledgePos1;
            Debug.Log("ledgePos1");
        }
    }

    public void FinishLedgeClimb()
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        Debug.Log("정상");
        ledgeDetected = false;
        anim.SetBool("canClimbLedge", canClimbLedge);

    }

    public void setRGBA(float color)
    {
        sprite.color = new Color(color, color, color, color);
    }
    public void setRb(float set)
    {
        rb.gravityScale = set;
    }
    //캐릭터 죽을시 리스폰
    public void PCDie()
    {
        anim.SetTrigger("isHit");
        rb.gravityScale = 0;
        Invoke("GoToCheckPoint", 1f);
    }
    public void GoToCheckPoint()
    {
        sprite.color = new Color(0, 0, 0, 0);
        reCheckPoint.vCamCall();
    }

    IEnumerator OneSecondCount()
    {
        yield return new WaitForSeconds(1.0f);
    }


    IEnumerator PlusFrameTime()
    {
        for (int i = 0; i < 4; i++)
        {
            frameInitTime++;
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, coliderSize.y / 2);//캐릭터 CapsuleCollider의 최하단 위치값. 


        SlopeCheckHorizontal(checkPos);//캐릭터의 바닥과 플랫폼이 맞붙는 각도에 따라 판별하는 함수들이기 때문에 Collider의 최하단 값 필요
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)//캐릭터가 움직임에 따라 (앞||뒤) 로 Raycast에 닿는 오브젝트의 위치에 따른 변수 isOnSlope 과 slopeSideAngle 값 입력.
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, WhatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, WhatIsGround);

        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else//평지에 있을때 || 공중에 있을 때
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }
    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, WhatIsGround);

        if (hit)//캐릭터가 바닥에 붙어있니
        {
            slopeNormoalPerp = Vector2.Perpendicular(hit.normal).normalized;//hit 레이케스트에 대한 수직 막대 표현을 위해 넣음.

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);//캐릭터와 바닥 각도

            if (slopeDownAngle != slopeDownAngleOld)//캐릭터와 바닥의 각도가 변화하면, 경사를 올라가는 중이다.
            {
                isOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormoalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)//max 앵글보다 더 큰 값에서는 경사를 올라갈 수 없음
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && movementInputDirection == 0.0f && canWalkOnSlope)//경사 위에 있고 움직임이 없으며 max 앵글값보다 작은 값의 경사 위에 있을때.
        {//이 변화가 1초쯤 늦게 작동되서 캐릭터가 자꾸 미끄러지는 현상 발생. 
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }

    private void CharactorHit()//캐릭터와 데미지가 있는 오브젝트 충돌처리
    {
        //데미지가 있는 물체와 충돌 체크
        //물체의 데미지 체크
        //현재 PC Hp - 데미지
        //이벤트 체크
        //디폴트라면 HitTime동안 무적(인스펙터), PC 이미지 블링크
        //지형 오브젝트에 의한 경우 RespawnPoint에 PC 재배치, HitTime동안 무적, PC이미지 블링크
    }

    private void CharactorDie()//캐릭터 사망 시 처리
    {
        //if PC_Hp 0
        //캐릭터 위치 = 가까운 리스폰 위치
        //레이어 크기 및 위치 초기화(플레이어 중심)
    }









    public void InitLayer()
    {
        layerMask.transform.position = transform.position;
        layerMask.transform.localScale = new Vector3(LightRadius.x, LightRadius.y, 0);
    }
    public float getLayerMaskY()
    {
        return layerMask.transform.position.y;
    }
    public float getLayerMaskX()
    {
        return layerMask.transform.position.x;
    }
    public void SetLayer(float x, float y, GameObject obj)
    {
        FrameSizeY = 108;
        FrameSizeX = 192;
        switch (obj.tag)
        {

            case "TopWall":
                layerMask.transform.position = new Vector3(layerMask.transform.position.x, layerMask.transform.position.y + (y / 2), 0);
                layerMask.transform.localScale = new Vector3(layerMask.transform.localScale.x, layerMask.transform.localScale.y + (y / 216), 0);
                //Right 상자 객체
                RectColliderPos[2].transform.position = new Vector3(RectColliderPos[2].transform.position.x, RectColliderPos[2].transform.position.y + (y / 2), 0);
                RectColliderPos[2].transform.localScale = new Vector3(RectColliderPos[2].transform.localScale.x, RectColliderPos[2].transform.localScale.y + (y / 216), 0);
                //Left 상자 객체
                RectColliderPos[3].transform.position = new Vector3(RectColliderPos[3].transform.position.x, RectColliderPos[3].transform.position.y + (y / 2), 0);
                RectColliderPos[3].transform.localScale = new Vector3(RectColliderPos[3].transform.localScale.x, RectColliderPos[3].transform.localScale.y + (y / 216), 0);

                //LeftUp상자 객체
                Rectdiagonal[0].transform.position = new Vector3(Rectdiagonal[0].transform.position.x, Rectdiagonal[0].transform.position.y + y, 0);
                //RightUp 상자 객체
                Rectdiagonal[2].transform.position = new Vector3(Rectdiagonal[2].transform.position.x, Rectdiagonal[2].transform.position.y + y, 0);
                break;
            case "BottomWall":
                layerMask.transform.position = new Vector3(layerMask.transform.position.x, layerMask.transform.position.y + (y / 2), 0);
                layerMask.transform.localScale = new Vector3(layerMask.transform.localScale.x, layerMask.transform.localScale.y - (y / 216), 0);
                //Right 상자 객체
                RectColliderPos[2].transform.position = new Vector3(RectColliderPos[2].transform.position.x, RectColliderPos[2].transform.position.y + (y / 2), 0);
                RectColliderPos[2].transform.localScale = new Vector3(RectColliderPos[2].transform.localScale.x, RectColliderPos[2].transform.localScale.y - (y / 216), 0);
                //Left 상자 객체
                RectColliderPos[3].transform.position = new Vector3(RectColliderPos[3].transform.position.x, RectColliderPos[3].transform.position.y + (y / 2), 0);
                RectColliderPos[3].transform.localScale = new Vector3(RectColliderPos[3].transform.localScale.x, RectColliderPos[3].transform.localScale.y - (y / 216), 0);
                //LeftDown 객체
                Rectdiagonal[1].transform.position = new Vector3(Rectdiagonal[1].transform.position.x, Rectdiagonal[1].transform.position.y + y, 0);
                //RightDown 객체
                Rectdiagonal[3].transform.position = new Vector3(Rectdiagonal[3].transform.position.x, Rectdiagonal[3].transform.position.y + y, 0);
                break;
            case "RightWall":
                layerMask.transform.position = new Vector3(layerMask.transform.position.x + (x / 2), layerMask.transform.position.y, 0);
                //Debug.Log((((rightPos.x + leftPos.x) / 2) / FrameSizeX)); 이 값이 늘어난 만큼의 값임.
                layerMask.transform.localScale = new Vector3(layerMask.transform.localScale.x + (x / 384), layerMask.transform.localScale.y, 0);

                //Up 상자 객체
                RectColliderPos[0].transform.position = new Vector3(RectColliderPos[0].transform.position.x + (x / 2), RectColliderPos[0].transform.position.y, 0);
                RectColliderPos[0].transform.localScale = new Vector3(RectColliderPos[0].transform.localScale.x + (x / 384), RectColliderPos[0].transform.localScale.y, 0);
                //down상자 객체
                RectColliderPos[1].transform.position = new Vector3(RectColliderPos[1].transform.position.x + (x / 2), RectColliderPos[1].transform.position.y, 0);
                RectColliderPos[1].transform.localScale = new Vector3(RectColliderPos[1].transform.localScale.x + (x / 384), RectColliderPos[1].transform.localScale.y, 0);

                //RightUp 객체
                Rectdiagonal[2].transform.position = new Vector3(Rectdiagonal[2].transform.position.x + x, Rectdiagonal[2].transform.position.y, 0);
                //RightDown 객체
                Rectdiagonal[3].transform.position = new Vector3(Rectdiagonal[3].transform.position.x + x, Rectdiagonal[3].transform.position.y, 0);
                break;
            case "LeftWall":
                layerMask.transform.position = new Vector3(layerMask.transform.position.x + (x / 2), layerMask.transform.position.y, 0);
                layerMask.transform.localScale = new Vector3(layerMask.transform.localScale.x + (-x / 384), layerMask.transform.localScale.y, 0);

                //Up 상자 객체
                RectColliderPos[0].transform.position = new Vector3(RectColliderPos[0].transform.position.x + (x / 2), RectColliderPos[0].transform.position.y, 0);
                RectColliderPos[0].transform.localScale = new Vector3(RectColliderPos[0].transform.localScale.x - (x / 384), RectColliderPos[0].transform.localScale.y, 0);
                //down상자 객체
                RectColliderPos[1].transform.position = new Vector3(RectColliderPos[1].transform.position.x + (x / 2), RectColliderPos[1].transform.position.y, 0);
                RectColliderPos[1].transform.localScale = new Vector3(RectColliderPos[1].transform.localScale.x - (x / 384), RectColliderPos[1].transform.localScale.y, 0);
                //LeftDown 객체
                Rectdiagonal[1].transform.position = new Vector3(Rectdiagonal[1].transform.position.x + x, Rectdiagonal[1].transform.position.y, 0);
                //LeftUp상자 객체
                Rectdiagonal[0].transform.position = new Vector3(Rectdiagonal[0].transform.position.x + x, Rectdiagonal[0].transform.position.y, 0);
                break;


            //    //이제부터는 대각선. 모든 상자를 건드려야한다. 일부 상자는 크기와 위치를 모두, 어떤상자는 크기만, 어떤상자는 위치만 변경시킨다.
            case "RightDown":
                layerMask.transform.position = new Vector3(layerMask.transform.position.x + (x / 2), layerMask.transform.position.y + (y / 2), 0);
                layerMask.transform.localScale = new Vector3(layerMask.transform.localScale.x + (x / 384), layerMask.transform.localScale.y - (y / 216), 0);

                //Up 상자 객체
                RectColliderPos[0].transform.position = new Vector3(RectColliderPos[0].transform.position.x + (x / 2), RectColliderPos[0].transform.position.y, 0);
                RectColliderPos[0].transform.localScale = new Vector3(RectColliderPos[0].transform.localScale.x + (x / 384), RectColliderPos[0].transform.localScale.y, 0);
                //down상자 객체
                RectColliderPos[1].transform.position = new Vector3(RectColliderPos[1].transform.position.x + (x / 2), RectColliderPos[1].transform.position.y + y, 0);
                RectColliderPos[1].transform.localScale = new Vector3(RectColliderPos[1].transform.localScale.x + (x / 384), RectColliderPos[1].transform.localScale.y, 0);
                //Right 상자 객체
                RectColliderPos[2].transform.position = new Vector3(RectColliderPos[2].transform.position.x + x, RectColliderPos[2].transform.position.y + (y / 2), 0);
                RectColliderPos[2].transform.localScale = new Vector3(RectColliderPos[2].transform.localScale.x, RectColliderPos[2].transform.localScale.y - (y / 216), 0);
                //Left 상자 객체
                RectColliderPos[3].transform.position = new Vector3(RectColliderPos[3].transform.position.x, RectColliderPos[3].transform.position.y + (y / 2), 0);
                RectColliderPos[3].transform.localScale = new Vector3(RectColliderPos[3].transform.localScale.x, RectColliderPos[3].transform.localScale.y - (y / 216), 0);

                //RightUp 객체
                Rectdiagonal[2].transform.position = new Vector3(Rectdiagonal[2].transform.position.x + x, Rectdiagonal[2].transform.position.y, 0);
                //LeftDown 객체
                Rectdiagonal[1].transform.position = new Vector3(Rectdiagonal[1].transform.position.x, Rectdiagonal[1].transform.position.y + y, 0);
                break;

            case "RightUp":
                layerMask.transform.position = new Vector3(layerMask.transform.position.x + (x / 2), layerMask.transform.position.y + (y / 2), 0);
                layerMask.transform.localScale = new Vector3(layerMask.transform.localScale.x + (x / 384), layerMask.transform.localScale.y + (y / 216), 0);

                //Up 상자 객체
                RectColliderPos[0].transform.position = new Vector3(RectColliderPos[0].transform.position.x + (x / 2), RectColliderPos[0].transform.position.y + y, 0);
                RectColliderPos[0].transform.localScale = new Vector3(RectColliderPos[0].transform.localScale.x + (x / 384), RectColliderPos[0].transform.localScale.y, 0);
                //down상자 객체
                RectColliderPos[1].transform.position = new Vector3(RectColliderPos[1].transform.position.x + (x / 2), RectColliderPos[1].transform.position.y, 0);
                RectColliderPos[1].transform.localScale = new Vector3(RectColliderPos[1].transform.localScale.x + (x / 384), RectColliderPos[1].transform.localScale.y, 0);
                //Right 상자 객체
                RectColliderPos[2].transform.position = new Vector3(RectColliderPos[2].transform.position.x + x, RectColliderPos[2].transform.position.y + (y / 2), 0);
                RectColliderPos[2].transform.localScale = new Vector3(RectColliderPos[2].transform.localScale.x, RectColliderPos[2].transform.localScale.y + (y / 216), 0);
                //Left 상자 객체
                RectColliderPos[3].transform.position = new Vector3(RectColliderPos[3].transform.position.x, RectColliderPos[3].transform.position.y + (y / 2), 0);
                RectColliderPos[3].transform.localScale = new Vector3(RectColliderPos[3].transform.localScale.x, RectColliderPos[3].transform.localScale.y + (y / 216), 0);

                //LeftUp상자 객체
                Rectdiagonal[0].transform.position = new Vector3(Rectdiagonal[0].transform.position.x, Rectdiagonal[0].transform.position.y + y, 0);
                //RightDown 객체
                Rectdiagonal[3].transform.position = new Vector3(Rectdiagonal[3].transform.position.x + x, Rectdiagonal[3].transform.position.y, 0);
                break;
            case "LeftUp":
                layerMask.transform.position = new Vector3(layerMask.transform.position.x + (x / 2), layerMask.transform.position.y + (y / 2), 0);
                layerMask.transform.localScale = new Vector3(layerMask.transform.localScale.x + (-x / 384), layerMask.transform.localScale.y + (y / 216), 0);

                //Up 상자 객체
                RectColliderPos[0].transform.position = new Vector3(RectColliderPos[0].transform.position.x + (x / 2), RectColliderPos[0].transform.position.y + y, 0);
                RectColliderPos[0].transform.localScale = new Vector3(RectColliderPos[0].transform.localScale.x - (x / 384), RectColliderPos[0].transform.localScale.y, 0);
                //down상자 객체
                RectColliderPos[1].transform.position = new Vector3(RectColliderPos[1].transform.position.x + (x / 2), RectColliderPos[1].transform.position.y, 0);
                RectColliderPos[1].transform.localScale = new Vector3(RectColliderPos[1].transform.localScale.x - (x / 384), RectColliderPos[1].transform.localScale.y, 0);
                //Right 상자 객체
                RectColliderPos[2].transform.position = new Vector3(RectColliderPos[2].transform.position.x, RectColliderPos[2].transform.position.y + (y / 2), 0);
                RectColliderPos[2].transform.localScale = new Vector3(RectColliderPos[2].transform.localScale.x, RectColliderPos[2].transform.localScale.y + (y / 216), 0);
                //Left 상자 객체
                RectColliderPos[3].transform.position = new Vector3(RectColliderPos[3].transform.position.x + x, RectColliderPos[3].transform.position.y + (y / 2), 0);
                RectColliderPos[3].transform.localScale = new Vector3(RectColliderPos[3].transform.localScale.x, RectColliderPos[3].transform.localScale.y + (y / 216), 0);
                //RightUp 객체
                Rectdiagonal[2].transform.position = new Vector3(Rectdiagonal[2].transform.position.x, Rectdiagonal[2].transform.position.y + y, 0);
                //LeftDown 객체
                Rectdiagonal[1].transform.position = new Vector3(Rectdiagonal[1].transform.position.x + x, Rectdiagonal[1].transform.position.y, 0);

                break;
            case "LeftDown":
                layerMask.transform.position = new Vector3(layerMask.transform.position.x + (x / 2), layerMask.transform.position.y + (y / 2), 0);
                layerMask.transform.localScale = new Vector3(layerMask.transform.localScale.x + (-x / 384), layerMask.transform.localScale.y - (y / 216), 0);
                //Up 상자 객체
                RectColliderPos[0].transform.position = new Vector3(RectColliderPos[0].transform.position.x + (x / 2), RectColliderPos[0].transform.position.y, 0);
                RectColliderPos[0].transform.localScale = new Vector3(RectColliderPos[0].transform.localScale.x - (x / 384), RectColliderPos[0].transform.localScale.y, 0);
                //down상자 객체
                RectColliderPos[1].transform.position = new Vector3(RectColliderPos[1].transform.position.x + (x / 2), RectColliderPos[1].transform.position.y + y, 0);
                RectColliderPos[1].transform.localScale = new Vector3(RectColliderPos[1].transform.localScale.x - (x / 384), RectColliderPos[1].transform.localScale.y, 0);
                //Right 상자 객체
                RectColliderPos[2].transform.position = new Vector3(RectColliderPos[2].transform.position.x, RectColliderPos[2].transform.position.y + (y / 2), 0);
                RectColliderPos[2].transform.localScale = new Vector3(RectColliderPos[2].transform.localScale.x, RectColliderPos[2].transform.localScale.y - (y / 216), 0);
                //Left 상자 객체
                RectColliderPos[3].transform.position = new Vector3(RectColliderPos[3].transform.position.x + x, RectColliderPos[3].transform.position.y + (y / 2), 0);
                RectColliderPos[3].transform.localScale = new Vector3(RectColliderPos[3].transform.localScale.x, RectColliderPos[3].transform.localScale.y - (y / 216), 0);
                //LeftUp상자 객체
                Rectdiagonal[0].transform.position = new Vector3(Rectdiagonal[0].transform.position.x + x, Rectdiagonal[0].transform.position.y, 0);
                //RightDown 객체
                Rectdiagonal[3].transform.position = new Vector3(Rectdiagonal[3].transform.position.x, Rectdiagonal[3].transform.position.y + y, 0);
                break;
        }
        isChangeWallOn = true;
        initial = true;
        RectLeftDown.Set(RectColliderPos[3].transform.position.x, RectColliderPos[1].transform.position.y, 0);
        RectLeftUp.Set(RectColliderPos[3].transform.position.x, RectColliderPos[0].transform.position.y, 0);
        //벽이 변해있다는뜻
        RectRightDown.Set(RectColliderPos[2].transform.position.x, RectColliderPos[1].transform.position.y, 0);
        RectRightUp.Set(RectColliderPos[2].transform.position.x, RectColliderPos[0].transform.position.y, 0);
        ////Debug.Log("RectLeftDown X : "+ RectLeftDown .x+ " RectLeftDown Y : "+ RectLeftDown.y);
        //// Debug.Log("RectRightUP X : " + RectRightUp.x + " RectRightUP Y : " + RectRightUp.y);
        //Debug.Log("Mask X : " + layerMask.transform.position.x + "Mask Y : " + layerMask.transform.position.y);
    }
    void MoveLayer()//원래 isWall과 isChange의 관계를 놓고 layer.position를 잡는 함수였는데 그렇게 되면 레이어가 형변환 한 후에도 따라오지 못해서 isChange를 isMove로 바꿈.
    {
        if (isChangeWallOn)
        {
            def = new Vector3(transform.position.x - RectColliderPos[0].transform.position.x,//RectColliderPos[0].transform.position.x
                transform.position.y - RectColliderPos[3].transform.position.y, 0);//딱 벽 바꾼 후 차이값
            //여기에 RectColliderPos 위치값 넣을 변수 하나 넣어서 밑에 넣어주는 것도.
        }


        if (isWallOn && !isChangeWallOn && isMoveWall && !initial)
        {
            layerMask.transform.position = transform.position;
            layerMask.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        }
        else if (isWallOn && !isChangeWallOn && !isMoveWall && initial)
        {
            //벽을 변환시킨 상태며 따라오지 않음.
            layerMask.transform.position = layerMask.transform.position;
        }
        else if (isWallOn && !isChangeWallOn && isMoveWall && initial)
        {
            //벽을 변환시킨상태이며 따라옴. 벽 캐릭터중심에에 고정
            //layerMask.transform.position = transform.position;
            //벽을 변환시킨상태이며 따라옴. 벽 월드에 고정
            layerMask.transform.position = transform.position - def;
        }
    }

    //벽움직임 iswall이 켜져있다면 캐릭터를 상자가 따라가도록 설정해주는 함수
    void OnRectCheck()
    {
        if (isWallOn && !isChangeWallOn)
        {
            for (int i = 0; i < RectColliderPos.Length; i++)
            {
                //  RectColliderPos[i].SetActive(true);
                switch (i)
                {
                    case 0://위쪽 상자.
                        RectColliderPos[i].transform.position = RectUp;
                        break;
                    case 1://아래쪽 상자.
                        RectColliderPos[i].transform.position = RectDown;
                        break;
                    case 2://오른쪽 상자.
                        RectColliderPos[i].transform.position = RectRight;
                        break;
                    case 3://왼쪽 상자.
                        RectColliderPos[i].transform.position = RectLeft;
                        break;
                }
                RectColliderPos[i].transform.localScale = new Vector3(LightRadius.x, LightRadius.y, 0);
            }
        }
    }

    void RectScaleInit()
    {
        for (int i = 0; i < 4; i++)
        {
            RectColliderPos[i].transform.localScale = new Vector3(2.0f, 2.0f, 0);
            Rectdiagonal[i].transform.localScale = new Vector3(2.0f, 2.0f, 0);
        }
    }
    private void InitializeRect()
    {
        if (isWallOn && !isChangeWallOn && isMoveWall && !initial)//제일처음 초기의 상태
        {
            //여기서 더하고 뺀 숫자는 레이어마스크 스프라이트의 x,y축/10
            RectUp.Set(transform.position.x, transform.position.y + ((1080 / 10) * LightRadius.y), 0);
            RectDown.Set(transform.position.x, transform.position.y - ((1080 / 10) * LightRadius.y), 0);
            RectLeft.Set(transform.position.x - ((1920 / 10) * LightRadius.x), transform.position.y, 0);
            RectRight.Set(transform.position.x + ((1920 / 10) * LightRadius.x), transform.position.y, 0);

            RectRightUp.Set(transform.position.x + ((1920 / 10) * LightRadius.x) + 10, transform.position.y + ((1080 / 10) * LightRadius.y) + 10, 0);
            RectRightDown.Set(transform.position.x + ((1920 / 10) * LightRadius.x) + 10, transform.position.y - ((1080 / 10) * LightRadius.y) - 10, 0);
            RectLeftUp.Set(transform.position.x - ((1920 / 10) * LightRadius.x) - 10, transform.position.y + ((1080 / 10) * LightRadius.y) + 10, 0);
            RectLeftDown.Set(transform.position.x - ((1920 / 10) * LightRadius.x) - 10, transform.position.y - ((1080 / 10) * LightRadius.y) - 10, 0);

            RectScaleInit();
        }
        else if (isWallOn && !isChangeWallOn && !isMoveWall && initial)//벽을 변환시킨 상태이며 벽이 따라오지 않음.
        {
            RectUp = RectColliderPos[0].transform.position;
            RectDown = RectColliderPos[1].transform.position;
            RectRight = RectColliderPos[2].transform.position;
            RectLeft = RectColliderPos[3].transform.position;
        }
        else if (isWallOn && !isChangeWallOn && isMoveWall && initial)//벽을 변환시킨 상태이며 벽이 따라옴
        {
            //임시로 가져온 캐릭터를 따라다니는 벽
            RectUp.Set(layerMask.transform.position.x, layerMask.transform.position.y + layerMask.transform.localScale.y * 108, 0);
            RectDown.Set(layerMask.transform.position.x, (layerMask.transform.position.y - (layerMask.transform.localScale.y) * 108), 0);
            RectRight.Set((layerMask.transform.position.x + (layerMask.transform.localScale.x) * 192), layerMask.transform.position.y, 0);
            RectLeft.Set((layerMask.transform.position.x - (layerMask.transform.localScale.x) * 192), layerMask.transform.position.y, 0);
            //레이어위치에 높이나 넓이는 레이어 스케일을 계산
            /*
            RectUp.Set(transform.position.x, transform.position.y + layerMask.transform.localScale.y * 108, 0);
            RectDown.Set(transform.position.x, transform.position.y - layerMask.transform.localScale.y * 108, 0);
            RectRight.Set(transform.position.x + layerMask.transform.localScale.x * 192, transform.position.y, 0);
            RectLeft.Set(transform.position.x - layerMask.transform.localScale.x * 192, transform.position.y, 0);
            */
            RectRightUp.Set(RectRight.x, RectUp.y, 0);
            RectRightDown.Set(RectRight.x, RectDown.y, 0);
            RectLeftUp.Set(RectLeft.x, RectUp.y, 0);
            RectLeftDown.Set(RectLeft.x, RectDown.y, 0);
        }
    }


    //마스크 캐릭터를 따라가게 만드는 함수
    private void InitializeLight()
    {
        if (isWallOn)
        {
            //빛 위치 조절.
            //LightPos.transform.position = ThisTransform.transform.position;
            //Light2D.Init(new Vector3(-10, -5), new Vector3(10, -5), new Vector3(10, 5), new Vector3(-10, 5));
            InitializeRect();
            moveLightWall();
        }
    }

    //벽들이 캐릭터를 따라가게 만드는 함수
    private void moveLightWall()
    {
        for (int i = 0; i < RectColliderPos.Length; i++)
        {
            //RectColliderPos[i].SetActive(true);
            switch (i)
            {
                case 0://위쪽 상자.
                    RectColliderPos[i].transform.position = RectUp;
                    break;
                case 1://아래쪽 상자.
                    RectColliderPos[i].transform.position = RectDown;
                    break;
                case 2://오른쪽 상자.
                    RectColliderPos[i].transform.position = RectRight;
                    break;
                case 3://왼쪽 상자.
                    RectColliderPos[i].transform.position = RectLeft;
                    break;
            }
        }
        for (int i = 0; i < Rectdiagonal.Length; i++)//꼭짓점 작은 상자
        {
            //RectColliderPos[i].SetActive(true);
            switch (i)
            {
                case 0://LU
                    Rectdiagonal[i].transform.position = RectLeftUp;
                    break;
                case 1://LD
                    Rectdiagonal[i].transform.position = RectLeftDown;
                    break;
                case 2://Ru
                    Rectdiagonal[i].transform.position = RectRightUp;
                    break;
                case 3://RD
                    Rectdiagonal[i].transform.position = RectRightDown;
                    break;
            }
        }
    }





    private void CheckIfWallsliding()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }
    private void CheckIfCanJump()
    {
        //밑의로 2개 if문 임의로 추가하였음 원래 checkground 밑으로 들어가는데 점프라 여기 넣음//추후 수정할 예정
        if (rb.velocity.y <= 0.01f)
        {
            isJumping = false;
        }
        if (isGrounded && !isJumping)//이부분 제어하면 좋은데 약간 버벅거리는 오류 남. && slopeDownAngle <= maxSlopeAngle) //땅 위에 가만히 있으면서 바닥과 캐릭터의 각도가 max각도보다 작을때. 
        {//오류 고치지 못하면 반복되기 때문에 지워도 ㄱㅊ을듯
            canJump = true;
        }
        if ((isGrounded && rb.velocity.y <= 0) || isWallSliding)
        {
            amountOfJumpsLeft = amountOfJumps;
        }
        if (amountOfJumpsLeft <= 0 || !isGrounded)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
        if (!isGrounded && isTouchingWall && isWallSliding)
        {
            canJump = true;
        }
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (rb.velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void CoyoteJump()
    {
        if (!isGrounded)//땅에 있지 않을 때
        {
            if (coyote_counter > 0)//카운터가 0이 아니면서
            {
                coyote_counter -= 1;

                if (!canJump)
                {
                    if (Input.GetButtonDown("Jump"))//점프 버튼을 누른다면
                    {
                        canJump = true;
                        Jump();//점프하시오
                    }
                }

            }
        }
        else
        {
            coyote_counter = coyote_max;//아닐시 초기화
        }
    }


    private void JumpBuffering()
    {
        if (Input.GetButtonDown("Jump"))
        {
            buffer_counter = buffer_max;//점프 버튼 눌리면 초기화함
            Debug.Log(buffer_counter);
        }

        if (buffer_counter > 0)//떨어지고 있는 중,,,
        {
            buffer_counter -= 1;
            Debug.Log(buffer_counter);

            if (isGrounded)//땅에 닿자마자 점프
            {
                buffer_counter = 0;
                Jump();
            }
        }
    }

    private void CheckInput()
    {
        CoyoteJump();
        JumpBuffering();
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);//여기서 시간차 제어,,, 빨리 떼면 적게 뜀.

        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isWallOn && isGrounded && canJump && !isBottomWall)
            {
                isWallOn = false;
                startLayerScale = layerMask.transform.localScale;
                for (int i = 0; i < 4; i++)
                {
                    startRectScale[i] = RectColliderPos[i].transform.localScale;
                }
            }
            if (frameInitTime <= 3)
            {
                initial = false;
                StopCoroutine(PlusFrameTime());
                frameInitTime = 0;
                FrameCoroutine = null;
            }
            else
            {
                if (FrameCoroutine != null)
                {
                    StopCoroutine(PlusFrameTime());
                    frameInitTime = 0;
                    FrameCoroutine = null;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isWallOn = true;
            if (isChangeWallOn == true)
            {
                isChangeWallOn = false;
            }
            if (frameInitTime == 4)
            {
                if (FrameCoroutine != null)
                {
                    StopCoroutine(PlusFrameTime());
                    frameInitTime = 0;
                    FrameCoroutine = null;
                }
            }
            else
            {
                FrameCoroutine = StartCoroutine(PlusFrameTime());
            }
            //코루틴 멈춤
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (isMoveWall && isGrounded && canJump)
            {
                isMoveWall = false;
            }
            else
            {
                isMoveWall = true;
            }
        }
    }
    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVeloctiy", rb.velocity.y);
    }


    private void CheckSurrounding()
    {
        isBottomWall = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, WhatIsBottom);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, WhatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, WhatIsGround);
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, WhatIsGround);

        isOverlapRect = Physics2D.Raycast(OverlapCheck.position, transform.right, OverlapCheckDistance, WhatIsOverlap);
        if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }

    }

    private void Jump()
    {
        if (canJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            //rb.velocity = new Vector2(rb.velocity.x, JumpForce * Physics2D.gravity.y * Time.deltaTime);
            amountOfJumpsLeft--;
            Debug.Log("평지점프");
        }
        else if ((isWallSliding || isTouchingWall) && canJump)
        {
            if (isFacingRight) movementInputDirection = -1;
            else if (!isFacingRight) movementInputDirection = 1;//자동 방향 전환
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            Debug.Log("벽점프");

        }
    }
    private void ApplyMovement()
    {
        if (isGrounded && !isOnSlope && !isJumping)//평지 위에 가만히
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
        else if (isGrounded && isOnSlope && !isJumping)// && canWalkOnSlope)//경사 위에 가만히. canWalkOnSlope로 max앵글값 조정. 만약 canWalkOnSlope생략할 시 max앵글값에 상관 없이 모든 경사 오르기 가능//큰일 canWalkOnSlope를 넣으면 wallJump시 캐릭터가 벽에 붙어서 못움직임.
        {
            rb.velocity = new Vector2(movementSpeed * slopeNormoalPerp.x * -movementInputDirection, movementSpeed * slopeNormoalPerp.y * -movementInputDirection);
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection != 0)
        {           //땅에 있지 않고 슬라이딩중도 아니며, 움직임 입력은 있을때  == 즉 공중에서 캐릭터 제어
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0); //입력방향+ForceInAir,y축에는 힘 안줌
            rb.AddForce(forceToAdd);

            if (Mathf.Abs(rb.velocity.x) > movementSpeed)//속도제한
            {
                rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
            }
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {           //땅에 있지 않고 슬라이딩중도 아니며, 움직임 입력도 없을때  == 즉 공중에서 캐릭터 제어
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void Flip()
    {
        if (!isWallSliding)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        //Raycast확인(벽 닿았는지)
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));

        Gizmos.DrawLine(OverlapCheck.position, new Vector3(OverlapCheck.position.x + OverlapCheckDistance, OverlapCheck.position.y + 0.7f, OverlapCheck.position.z));
    }
}
