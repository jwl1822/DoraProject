using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    [SerializeField] string csv_FileName;

    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>();

    public static bool isFinish = false;

    private void Awake()
    {
        if (instance ==null)
        {
            instance = this;
            DialogueParser theParser = GetComponent<DialogueParser>();
            Dialogue[] dialogues = theParser.Parse(csv_FileName);
            for (int i=0; i<dialogues.Length;i++)
            {
                dialogueDic.Add(i+1, dialogues[i]);
            }

            isFinish = true;
        }
    }

    public Dialogue[] GetDialogue(int _startNum, int _EndNum)        
    {                                                                
        List<Dialogue> dialogueList = new List<Dialogue>();

        for (int i =0; i<= _EndNum - _startNum;i++)
        {
            dialogueList.Add(dialogueDic[_startNum + i]);
        }
        return dialogueList.ToArray();
    }
}
