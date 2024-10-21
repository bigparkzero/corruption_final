using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class GimmickTrigger : MonoBehaviour
{
    [Header("체크하면 함수 실행후 자기자신 파괴")]
    public bool InvokeEventEndDestroy;
    [Header("체크하면 함수 실행후 자기자신 비활성화")]
    public bool InvokeEventEndDeactivate;

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
                        associatedOutput.isDone = false; // isDone을 false로 설정
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
        // OutputEvent 리스트를 순회하면서 하나씩 실행
        for (int i = 0; i < OutputEvent.Count; i++)
        {
            // 이벤트 호출
            OutputEvent[i].Invoke();

            // GimmickOutput의 isDone을 false로 설정
            for (int j = 0; j < OutputEvent[i].GetPersistentEventCount(); j++)
            {
                Object targetObj = OutputEvent[i].GetPersistentTarget(j);
                GimmickOutput associatedOutput = targetObj.GetComponent<GimmickOutput>();
                if (associatedOutput != null)
                {
                    associatedOutput.isDone = false; // 이벤트 호출 후 isDone을 false로 설정
                }
            }

            isOutputGimmick = false; // Gimmick_Output이 존재하는지 체크
            isDoneAll = false; // 완료 여부 체크

            // Gimmick_Output 체크
            for (int j = 0; j < OutputEvent[i].GetPersistentEventCount(); j++)
            {
                Object targetObj = OutputEvent[i].GetPersistentTarget(j);
                GimmickOutput associatedOutput = targetObj.GetComponent<GimmickOutput>();
                if (associatedOutput != null)
                {
                    isOutputGimmick = true; // Gimmick_Output이 존재함
                }
            }

            if (isOutputGimmick)
            {
                // 해당 오브젝트의 isDone이 true가 될 때까지 대기
                while (!isDoneAll)
                {
                    yield return null; // 매 프레임 대기
                    isDoneAll = true; // 기본적으로 완료된 것으로 설정
                    for (int j = 0; j < OutputEvent[i].GetPersistentEventCount(); j++)
                    {
                        Object targetObj = OutputEvent[i].GetPersistentTarget(j);
                        GimmickOutput associatedOutput = targetObj.GetComponent<GimmickOutput>();
                        if (associatedOutput != null && !associatedOutput.isDone)
                        {
                            isDoneAll = false; // 완료되지 않은 경우 false로 설정
                            break; // 더 이상 확인할 필요 없음
                        }
                    }

                    Debug.Log("<color=yellow>" + gameObject.name + "</color>" + "의 " + "<color=blue>" + i + "</color>" + " 번째 이벤트 기다리는 중");
                }
            }

            // i + 1이 OutputEvent.Count를 초과하지 않도록 확인
            if (i + 1 < OutputEvent.Count)
            {
                OutputEvent[i + 1].Invoke();
                Debug.Log("<color=yellow>" + gameObject.name + "</color>" + " 의 " + "<color=blue>" + i + "</color>" + " 번째 이벤트 실행");
            }
        }

        Debug.Log("<color=yellow>" + gameObject.name + "</color>" + "의 모든 이벤트 실행 완료");
        if (InvokeEventEndDeactivate)
        {
            gameObject.SetActive(false);
            Debug.Log("<color=yellow>" + gameObject.name + "</color>" + " 비활성화됨");
        }
        if (InvokeEventEndDestroy)
        {
            Destroy(gameObject);
            Debug.Log("<color=yellow>" + gameObject.name + "</color>" + " 삭제됨");
        }
    }

    public bool testbool;
    [HideInInspector] public bool isTriggered;

    [Header("GimmickInput을 설정할 수 있습니다.\n 설정값이 없을 경우 부모 오브젝트의 GimmickInput를 실행합니다")]
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
            Debug.LogWarning("Gimmick Input script not found in parent object! Please add the Gimmick Input script to the parent object or place it as a child object of the object with Gimmick Input.");
        }
    }

    private void OnValidate()
    {
        GimmickInput GmmiInput = GetComponentInParent<GimmickInput>();
        if (GmmiInput != null)
        {
            GimmickInput = GmmiInput;
        }
        testbool = false;
    }
}
