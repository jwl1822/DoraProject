using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatSystem : Obj
{
    /*
    public Queue<Dialogue[]> sentences;
    //public Queue<string> sentences;
    public Dialogue[] currentSentence;
    public TextMeshPro text;
    public GameObject quad;
    private bool nowEnd;        //끝났음을 알림.

    private int count;
    private void Start()
    {
        count = 0;
        obj_player = FindObjectOfType<PlayerControl>();//하나있는 캐릭터 찾아줌
    }


    public bool getnowEnd()
    {
        return nowEnd;
    }

    public Queue<string> ClearQueue(Queue<string> q)
    {
        q.Clear();
        return q;
    }

    public void Ondialogue(Dialogue[] lines,Transform chatPoint)
    {
        nowEnd = false;
        transform.position = chatPoint.position;
        sentences = new Queue<Dialogue[]>();
        //sentences = new Queue<string>();
        sentences.Clear();
        sentences.Enqueue(lines);

        
        foreach (var line in lines)
        {
            sentences.Enqueue(line);
        }
        Debug.Log("Ondial에서 실행함");
        DialogueFlow(chatPoint);
        //StartCoroutine(DialogueFlow(chatPoint));
    }

    public void DialogueFlow(Transform chatPoint)
    {
        if (sentences.Count!=0)
        {
            currentSentence = sentences.Dequeue();
            //currentSentence = sentences.Dequeue();
            Debug.Log(sentences.Count);
            text.text = currentSentence[0].contexts[count];
            count++;
            float x = text.preferredWidth;
            x = (x > 300) ? 300 : x + 30f;
            quad.transform.localScale = new Vector2(x, text.preferredHeight + 50f);
            transform.position = new Vector2(chatPoint.position.x, chatPoint.position.y + text.preferredHeight / 2);
        }
        else
        {
            Debug.Log("파괴함");
            Destroy(gameObject);
            nowEnd = true;
            if (obj_player.getNowTalk())
            {
                obj_player.setNowTalk(false);
                obj_player.resetnpcChat();
            }
        }
    }


    IEnumerator DialogueFlow(Transform chatPoint)
    {
        yield return null;
        while (sentences.Count>0)
        {
           currentSentence = sentences.Dequeue();
            text.text = currentSentence;
            float x = text.preferredWidth;
            x = (x > 3) ? 3 : x + 0.3f;
            quad.transform.localScale = new Vector2(x, text.preferredHeight + 0.3f);
            transform.position = new Vector2(chatPoint.position.x, chatPoint.position.y + text.preferredHeight / 2);
            yield return new WaitForSeconds(3f);
        }
        Destroy(gameObject);
        nowEnd = true;
    }*/

    Dialogue[] dialogues;
    public TextMeshPro text;
    bool isDialogue = false;//대화중일 경우 true
    bool isNext = false;    //특정 키 입력 대기
    public GameObject quad;
    int lineCount = 0;// 대화 카운트
    int contextCount = 0;//대사 카운트

    private void Start()
    {
        obj_player = FindObjectOfType<PlayerControl>();//하나있는 캐릭터 찾아줌
    }

    private void Update()
    {
        if (isDialogue)
        {
            if (isNext)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    isNext = false;
                    text.text = "";
                    if (++contextCount < dialogues[lineCount].contexts.Length)
                    {
                        StartCoroutine(TypeWriter());
                    }
                    else
                    {
                        contextCount = 0;
                        if (++lineCount < dialogues.Length)
                        {
                            StartCoroutine(TypeWriter());
                        }
                        else
                        {
                            EndDialogue();
                        }
                    }
                }
            }
        }
    }

    public void EndDialogue()
    {
        isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;
        obj_player.resetnpcChat();
        obj_player.setNowTalk(false);
        Destroy(gameObject);
    }

    public void Ondialogue(Dialogue[] p_dialogues,Transform chatPoint)
    {
        isDialogue = true;
        transform.position = chatPoint.position;
        dialogues = p_dialogues;
        float x = text.preferredWidth;
        x = (x > 500) ? 5000 : x + 500f;
        quad.transform.localScale = new Vector2(x, text.preferredHeight + 200f);
        transform.position = new Vector2(chatPoint.position.x, chatPoint.position.y + text.preferredHeight / 2);
        StartCoroutine(TypeWriter());
    }
    IEnumerator TypeWriter()
    {
        string t_ReplaceText = dialogues[lineCount].contexts[contextCount];
        t_ReplaceText = t_ReplaceText.Replace("'", ",");

        for (int i =0;i<= dialogues[lineCount].contexts[contextCount].Length; i++)
        {
            text.text = t_ReplaceText.Substring(0,i);
            yield return new WaitForSeconds(0.1f);
        }
        isNext = true;

    }
}
