using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    //중경 원경 특정오브젝트와의 줌 등을 관리해줄 카메라 매니져.
    //각 카메라를 만들어놓고 특정 트리거를 지나갈 시에  오





    bool nowCamera;
    public CinemachineVirtualCamera VCamera;
    // Start is called before the first frame update
    void Start()
    {
        VCamera = GetComponent<CinemachineVirtualCamera>();
        nowCamera = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))//shift키를 누르면
        {
            if (nowCamera)
            {
                VCamera.Priority = 101;
                nowCamera = false;
            }
            else
            {
                VCamera.Priority = 1;
                nowCamera = true;
            }

        }
    }
}
