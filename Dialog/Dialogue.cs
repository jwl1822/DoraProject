using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Dialogue
{
    [Tooltip("대사를 말하는 캐릭터 이름")]
    public string name;

    [Tooltip("대사")]
    public string[] contexts;
}
[System.Serializable]
public class DialogueEvent
{
    //이벤트 대사를 설명하는것.
    public string name;

    //엑셀의 행과 열.
    public Vector2 line;

    //불러오는 대화 배열
    public Dialogue[] dialogues;
}