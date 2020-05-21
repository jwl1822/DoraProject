using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    public GameObject[] BackGround;
    private Vector3 lastScreenPosition;
    public float XsmoothTime = 0.4f;
    public float YsmoothTime = 0.4f;
    float parall;
    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        lastScreenPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject[] getBackground()
    {
        return BackGround;
    }

    private void LateUpdate()
    {
        foreach(GameObject obj in BackGround)
        {
            float parallexSpeed = 1 - Mathf.Clamp01(Mathf.Abs(transform.position.z / obj.transform.position.z));
            float difference = transform.position.x - lastScreenPosition.x;
            obj.transform.position = Vector3.Lerp(obj.transform.position, obj.transform.position + Vector3.right * difference * (parallexSpeed+1) , XsmoothTime);
            //Debug.Log(obj.transform.position[0]);
        }
        lastScreenPosition = transform.position;
    }
}
