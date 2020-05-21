using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutFrameObj : MonoBehaviour
{
    [SerializeField]
    GameObject Obj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Obj.transform.position;
        transform.localScale = Obj.transform.localScale;
    }
}
