using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class GimmickManager : Singleton<GimmickManager>
{
    #region ī�޶� ��ȯ
    public CinemachineFreeLook freeLookCamera;
    private List<CinemachineVirtualCameraBase> allCameras = new List<CinemachineVirtualCameraBase>();
    private int activeCameraPriority = 10;
    private int inactiveCameraPriority = 0;

    private CinemachineBrain brain;
    #endregion
   
    void Start()
    {
        // ���� ī�޶� CinemachineBrain�� �־�� �մϴ�.
        brain = Camera.main.GetComponent<CinemachineBrain>();

        // ���� ��� Ȱ��ȭ�� ī�޶� ã�� ��Ȱ��ȭ�մϴ�.
        foreach (var cam in FindObjectsOfType<CinemachineVirtualCameraBase>())
        {
            if (cam.gameObject.activeSelf)
            {
                allCameras.Add(cam);
                cam.gameObject.SetActive(false);
            }
        }

        // ������ ī�޶� Ȱ��ȭ�մϴ�.
        if (freeLookCamera != null)
        {
            freeLookCamera.gameObject.SetActive(true);
        }
    }

    // ī�޶� ��ȯ�ϴ� �Լ�
    public void SwitchToCamera(CinemachineVirtualCameraBase newCamera)
    {

        // ������ ī�޶� ��Ȱ��ȭ�մϴ�.
        if (freeLookCamera != null)
        {
            freeLookCamera.gameObject.SetActive(false);
        }

        // ��� ī�޶��� �켱������ ���߰� ��Ȱ��ȭ�մϴ�.
        foreach (var cam in allCameras)
        {
            cam.gameObject.SetActive(false);
            cam.Priority = inactiveCameraPriority;
        }

        // ���ο� ī�޶� Ȱ��ȭ�ϰ� �켱������ ���Դϴ�.
        if (newCamera != null)
        {
            newCamera.gameObject.SetActive(true);
            newCamera.Priority = activeCameraPriority;
        }
    }

   
}



