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
            Debug.LogWarning("PlayerController 스크립트가 오브젝트에 존재하지 않습니다.");
        }

        if (characterMovement == null)
        {
            Debug.LogWarning("CharacterMovement 스크립트가 오브젝트에 존재하지 않습니다.");
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
