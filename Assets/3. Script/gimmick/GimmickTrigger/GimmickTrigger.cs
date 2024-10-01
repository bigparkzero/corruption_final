using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class GimmickTrigger : MonoBehaviour
{
    public void test(string text)
    {
        Debug.Log(text);
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
    public void DestroyObject(GameObject target)
    {
        Destroy(target);
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








    public bool testbool;
    [HideInInspector] public bool isTriggered;
    [Header("GimmickInput�� ������ �� �ֽ��ϴ�.\n �������� ���� ��� �θ� ������Ʈ�� GimmickInput�� �����մϴ�")]
    public GimmickInput GimmickInput;
    private void Start()
    {
        isTriggered = false;
    }
    public void InvokeEventRunOnTrigger()
    {
        if (GimmickInput != null)
        {
            isTriggered = true;
            InvokeEvent();
            GimmickInput.TriggerCheck();
        }
        else
        {
            if (OutputEvent != null)
            {
                InvokeEvent();
            }
            Debug.LogWarning("gimmick input script not found in parent object!! Please add the gimmick input script to the parent object or place it as a child object of the object with gimmick input.");
        }
    }
    private void OnValidate()
    {

        GimmickInput GmmiInut = GetComponentInParent<GimmickInput>();
        if (GmmiInut != null)
        {
            GimmickInput = GmmiInut;
        }
        testbool = false;
    }
}
