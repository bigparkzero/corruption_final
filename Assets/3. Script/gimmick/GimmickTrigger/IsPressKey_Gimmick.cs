using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPressKey_Gimmick : GimmickTrigger
{
    [Header("üũ�� Ű ����")]
    public KeyCode ChecKkeyCode;
    [Header("üũ�ϸ� �ƹ�Ű üũ")]
    public bool isanykey;
    public KeyCode ChecKeyCode;
    [Header("üũ�ϸ� �������� �Լ� ���� üũ �����ϸ� ������ ���� �Լ� ����")]
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