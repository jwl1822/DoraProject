using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCamParallaxing : MonoBehaviour
{
    public GameObject[] BackGround;
    private Vector3 lastScreenPosition;
    public float smoothTime = 0.4f;
    float parall;
    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        BackGround = FindObjectOfType<Parallaxing>().getBackground();

        lastScreenPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void LateUpdate()
    {
        foreach (GameObject obj in BackGround)
        {
            float parallexSpeed = 1 - Mathf.Clamp01(Mathf.Abs(transform.position.z / obj.transform.position.z));
            float difference = transform.position.x - lastScreenPosition.x;
            obj.transform.position = Vector3.Lerp(obj.transform.position, obj.transform.position + Vector3.right * difference * (parallexSpeed + 1), smoothTime);
            //Debug.Log(obj.transform.position[0]);
        }
        lastScreenPosition = transform.position;
    }
}
