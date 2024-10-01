using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit_Gimmick : GimmickTrigger
{
    public LayerMask defaultLayerMask;
    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� ����Ʈ ���̾ �ִ��� Ȯ��
        if ((defaultLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            // ����Ʈ ���̾ ���ϴ� ��ü�� ��쿡�� �̺�Ʈ ����
            InvokeEventRunOnTrigger();
        }
    }
}
