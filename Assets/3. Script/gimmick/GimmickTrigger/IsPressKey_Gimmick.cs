using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPressKey_Gimmick : GimmickTrigger
{
    [Header("üũ�� Ű ����")]
    public KeyCode ChecKkeyCode;
    [Header("üũ�ϸ� �������� �Լ� ���� üũ �����ϸ� ������ ���� �Լ� ����")]
    public bool getkeydown;
    [Header("üũ�ϸ� �Լ� ������ �ڱ��ڽ� �ı�")]
    public bool InvokeEventEndDestroy;
    private void Update()
    {
        if (getkeydown)
        {
            if (Input.GetKeyDown(ChecKkeyCode))
            {
                InvokeEventRunOnTrigger();
            }
            else
            {
                return;
            }
        }
        else
        {
            if (Input.GetKeyUp(ChecKkeyCode))
            {

                InvokeEventRunOnTrigger();
                if (InvokeEventEndDestroy)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                return;
            }
        }
    }

}
