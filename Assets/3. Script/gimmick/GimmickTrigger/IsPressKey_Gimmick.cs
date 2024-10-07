using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPressKey_Gimmick : GimmickTrigger
{
    [Header("체크할 키 설정")]
    public KeyCode ChecKkeyCode;
    [Header("체크하면 눌렀을때 함수 실행 체크 해제하면 눌렀다 땔때 함수 실행")]
    public bool getkeydown;
    [Header("체크하면 함수 실행후 자기자신 파괴")]
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
