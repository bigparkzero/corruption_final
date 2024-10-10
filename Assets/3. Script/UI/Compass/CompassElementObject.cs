using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassElementObject : MonoBehaviour
{
    public Sprite ElementImage;

    private CompassElementAdder compassAdder;
    public Vector2 compassAdderSize = new Vector2(50,50);
    private IndicatorElementAdder indicatorAdder;
    public Vector2 indicatorAdderSize = new Vector2(150,150);

    private void Awake()
    {
        // 추가를 위해 초기화
        compassAdder = CompassUiScript.Instance.CompassBar.ElementContainer.GetComponent<CompassElementAdder>();
        indicatorAdder = CompassUiScript.Instance.GetComponent<IndicatorElementAdder>();
    }

    private void OnEnable()
    {
        if (compassAdder != null)
        {
            compassAdder.AddElement(this);  // 자신을 리스트에 추가
            indicatorAdder.AddElement(this);
        }
        else
        {
            Debug.LogWarning("ElementContainer에서 CompassElementAdder 스크립트를 찾을 수 없음");
            return;
        }
    }

    private void OnDisable()
    {
        if (compassAdder != null)
        {
            compassAdder.RemoveElement(this);  // 자신을 리스트에서 제거
            indicatorAdder.RemoveElement(this);
        }
    }

    public float GetElementAngle()
    {
        // 요소의 각도를 반환 (나중에 실제 각도를 설정)
        return transform.eulerAngles.y; // y축을 기준으로 각도를 반환
    }

    public Vector3 GetElementPosition()
    {
        return transform.position; // 요소의 월드 공간 위치 반환
    }
}
