using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPressKey_Gimmick : GimmickTrigger
{
    [Header("체크할 키 설정")]
    public KeyCode ChecKkeyCode;
    [Header("체크하면 아무키 체크")]
    public bool isanykey;
    public KeyCode ChecKeyCode;
    [Header("체크하면 눌렀을때 함수 실행 체크 해제하면 눌렀다 땔때 함수 실행")]
    public bool getkeydown;

    private void Update()
    {
        if (getkeydown)
        {
            if (Input.GetKeyDown(ChecKkeyCode) && !isanykey)
            {
                InvokeEventRunOnTrigger();
            }
            if (Input.anyKeyDown && isanykey)
            {
                InvokeEventRunOnTrigger();
            }
            return;
        }
        else
        {
            if (Input.GetKeyUp(ChecKkeyCode) && !isanykey)
            {
                InvokeEventRunOnTrigger();
            }
            if (Input.anyKeyDown && isanykey)
            {
                InvokeEventRunOnTrigger();
            }
            return;
        }
    }

}