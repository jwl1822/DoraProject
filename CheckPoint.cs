using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CheckPoint : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public CinemachineVirtualCamera main;
    public PlayerControl player;
    // Start is called before the first frame update
    void Start()
    {
        main = GameObject.Find("MainCam").GetComponent<CinemachineVirtualCamera>();
        player = FindObjectOfType<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public CheckPoint GetCheckPoint()
    {
        return this;
    }

    public void vCamCall()
    {
        vCam.Priority = main.Priority + 1;
        main.Priority=- 1;                    
        Invoke("movePC", 2f);
    }
    public void mainCall()
    {
        player.setRGBA(1);
        vCam.Priority = main.Priority - 1;
        main.Priority = 20;
    }
    public void movePC()
    {
        player.transform.position = transform.position;
        player.setRb(4);
        mainCall();
    }
    public Vector3 getTransform()
    {
        return transform.position;
    }
}
