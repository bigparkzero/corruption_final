using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GimmickInput : MonoBehaviour
{
    public bool testbool;
    [Header("�ڽĿ�����Ʈ���� gimmick trigger�� ���� ������Ʈ�� Ž��\n������ Ʈ���Ÿ� �־ ���� Ʈ���Ÿ� ������ �� �ֽ��ϴ�")]
    public List<GimmickTrigger> triggers;
    
    public enum typetotrigger
    {
        And,
        Or,
        Sequentially
    }

    [Header("And = ���� Ʈ���� �Ǿ���\nOr = Ʈ������ �ϳ��� Ʈ���ŵǸ� ���� \nSequentially = ����Ʈ�� ������� Ʈ���� �Ǿ���")]
    public typetotrigger TypeToTrigger;
    private int sequentialIndex = 0;
    bool CheckAllTriggers()
    {
        foreach (var trigger in triggers)
        {
            if (!trigger.isTriggered) // �ϳ��� Ʈ���ŵ��� �ʾҴٸ� false ��ȯ
            {
                return false;
            }
        }
        return true; // ��� Ʈ���ŵǾ��ٸ� true ��ȯ
    }

    // Ʈ���� �� �ϳ��� �ߵ��Ǿ����� Ȯ���ϴ� �Լ� (Or ���)
    bool CheckAnyTrigger()
    {
        foreach (var trigger in triggers)
        {
            if (trigger.isTriggered) // �ϳ��� Ʈ���ŵǾ��ٸ� true ��ȯ
            {
                return true;
            }
        }
        return false; // �ƹ� Ʈ���ŵ� �ߵ����� �ʾ����� false ��ȯ
    }

    // ���������� Ʈ���Ű� �ߵ��Ǿ����� Ȯ���ϴ� �Լ� (Sequentially ���)
    bool CheckSequentialTrigger()
    {
        // ���� ������ Ʈ���Ű� �ߵ��Ǿ����� Ȯ��
        if (sequentialIndex < triggers.Count && triggers[sequentialIndex].isTriggered)
        {
            sequentialIndex++; // ���� ������ �̵�

            // ��� Ʈ���Ű� ���������� �ߵ��Ǿ����� true ��ȯ
            if (sequentialIndex >= triggers.Count)
            {
                sequentialIndex = 0; // ���� �ʱ�ȭ (���� üũ�� ����)
                return true;
            }
        }

        return false;
    }
    public void TriggerCheck()
    {
        if (triggers.Count != 0)
        {
            if (triggers.Count <= 2)
            {
                switch (TypeToTrigger)
                {
                    case typetotrigger.And:
                        if (CheckAllTriggers())
                        {
                            InvokeEvent();
                        }
                        break;

                    case typetotrigger.Or:
                        if (CheckAnyTrigger())
                        {
                            InvokeEvent();
                        }
                        break;

                    case typetotrigger.Sequentially:
                        if (CheckSequentialTrigger())
                        {
                            InvokeEvent();
                        }
                        break;
                }
            }
            else
            {
                InvokeEvent();
            }
        }
    }



[System.Serializable]
    public class SimpleEvent : UnityEvent
    {
    }

    public List<SimpleEvent> OutputEvent;

    public bool triggerMultipleTimes;

    public void InvokeEvent()
    {
        if (triggerMultipleTimes)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                triggers[i].isTriggered = false;
            }
            for (int i = 0; i < OutputEvent.Count; i++)
            {
                for (int j = 0; j < OutputEvent[i].GetPersistentEventCount(); j++)
                {
                    Object targetObj = OutputEvent[i].GetPersistentTarget(j);
                    GimmickOutput associatedOutput = targetObj.GetComponent<GimmickOutput>();
                    if (associatedOutput != null)
                    {
                        associatedOutput.isDone = false;
                    }
                }
            }
        }
        if (OutputEvent != null && OutputEvent.Count > 0)
        {
            StartCoroutine(InvokeEventsCoroutine());
        }
    }

    private bool isOutputGimmick;
    private bool isDoneAll;


    private IEnumerator InvokeEventsCoroutine()
    {
        OutputEvent[0].Invoke();

        // OutputEvent ����Ʈ�� ��ȸ�ϸ鼭 �ϳ��� ����
        for (int i = 0; i < OutputEvent.Count; i++)
        {
            isOutputGimmick = false;
            isDoneAll = false; // �� �̺�Ʈ���� �ʱ�ȭ

            for (int j = 0; j < OutputEvent[i].GetPersistentEventCount(); j++)
            {
                Object targetObj = OutputEvent[i].GetPersistentTarget(j);
                GimmickOutput associatedOutput = targetObj.GetComponent<GimmickOutput>();
                if (associatedOutput != null)
                {
                    isOutputGimmick = true;
                }
            }

            if (isOutputGimmick)
            {
                // �ش� ������Ʈ�� isDone�� true�� �� ������ ���
                while (!isDoneAll)
                {
                    yield return null;
                    isDoneAll = true; // �⺻������ �Ϸ�� ������ ����
                    for (int j = 0; j < OutputEvent[i].GetPersistentEventCount(); j++)
                    {
                        Object targetObj = OutputEvent[i].GetPersistentTarget(j);
                        GimmickOutput associatedOutput = targetObj.GetComponent<GimmickOutput>();
                        if (associatedOutput != null && !associatedOutput.isDone)
                        {
                            isDoneAll = false; // �Ϸ���� ���� ��� false�� ����
                            break; // �� �̻� Ȯ���� �ʿ� ����
                        }
                    }

                    print("��ٸ��� ��");
                }
            }

            // i + 1�� OutputEvent.Count�� �ʰ����� �ʵ��� Ȯ��
            if (i + 1 < OutputEvent.Count)
            {
                OutputEvent[i + 1].Invoke();
                print(i + "��° �̺�Ʈ ����");
            }
        }
    }

}
