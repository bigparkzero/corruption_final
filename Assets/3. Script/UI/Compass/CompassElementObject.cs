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
        // �߰��� ���� �ʱ�ȭ
        compassAdder = CompassUiScript.Instance.CompassBar.ElementContainer.GetComponent<CompassElementAdder>();
        indicatorAdder = CompassUiScript.Instance.GetComponent<IndicatorElementAdder>();
    }

    private void OnEnable()
    {
        if (compassAdder != null)
        {
            compassAdder.AddElement(this);  // �ڽ��� ����Ʈ�� �߰�
            indicatorAdder.AddElement(this);
        }
        else
        {
            Debug.LogWarning("ElementContainer���� CompassElementAdder ��ũ��Ʈ�� ã�� �� ����");
            return;
        }
    }

    private void OnDisable()
    {
        if (compassAdder != null)
        {
            compassAdder.RemoveElement(this);  // �ڽ��� ����Ʈ���� ����
            indicatorAdder.RemoveElement(this);
        }
    }

    public float GetElementAngle()
    {
        // ����� ������ ��ȯ (���߿� ���� ������ ����)
        return transform.eulerAngles.y; // y���� �������� ������ ��ȯ
    }

    public Vector3 GetElementPosition()
    {
        return transform.position; // ����� ���� ���� ��ġ ��ȯ
    }
}
