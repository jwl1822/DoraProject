using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcSentence : Obj
{
    //public Dialogue[] sentences;
    //public string[] sentences;
    public Transform chatTr;
    public GameObject chatBoxPrefab;
    public ChatSystem CSystem;
    public GameObject Go;
    public bool nowtalk;

    private float emotion;

    void Start()
    {
        ObjPosX = transform.position.x;
        ObjPosY = transform.position.y;
    }
    private void Update()
    {
        //SetOn체크
        CheckInRect(obj_player.RectRightUp, obj_player.RectLeftDown);
        //매프레임마다 애니메이션 체크
        UpdateAnimations();
    }

    //감정변환 함수
    private void Emotion(int p_emotion)
    {
        //여기서 파싱해온 감정값에 따라 emotion값을 변경해 줄것.

    }

    //애니메이션 체크 함수.
    private void UpdateAnimations()
    {
        //여기에 각 애니메이션(idle, surprised)등 bool변수를 넣고 setBool해주면 됨.

    }

    public bool getnow()
    {
        return nowtalk;
    }
    public void TalkNpc()
    {
        
        Go = Instantiate(chatBoxPrefab);
        Go.GetComponent<ChatSystem>().Ondialogue(this.GetComponent<InteractionEvent>().GetDialogue(),chatTr);
    }
    public void EndTalkNpc()
    {
        Go.GetComponent<ChatSystem>().EndDialogue();
    }

}
