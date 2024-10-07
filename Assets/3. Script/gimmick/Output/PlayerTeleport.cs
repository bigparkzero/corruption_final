using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : GimmickOutput
{
    public GameObject controller;
    public Transform PlayerTeleportPosition;
    public void Act()
    {
        controller.transform.position = PlayerTeleportPosition.position;
        isDone = true;
    }

   
}
