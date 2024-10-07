using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePlayer : GimmickOutput
{

    public GameObject controller;

    public bool isFreeze;

    public void Act()
    {
        var playerController = controller.GetComponent<PlayerController>();
        var characterMovement = controller.GetComponent<CharacterMovement>();

        if (playerController == null)
        {
            Debug.LogWarning("PlayerController ��ũ��Ʈ�� ������Ʈ�� �������� �ʽ��ϴ�.");
        }

        if (characterMovement == null)
        {
            Debug.LogWarning("CharacterMovement ��ũ��Ʈ�� ������Ʈ�� �������� �ʽ��ϴ�.");
        }

        if (isFreeze)
        {
            if (playerController != null) playerController.enabled = false;
            if (characterMovement != null) characterMovement.enabled = false;
        }
        else
        {
            if (playerController != null) playerController.enabled = true;
            if (characterMovement != null) characterMovement.enabled = true;
        }

        isDone = true;
    }

}
