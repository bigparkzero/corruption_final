using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassElementAdder : MonoBehaviour
{
    public List<CompassElementObject> compassElements = new List<CompassElementObject>();
    private List<GameObject> uiElements = new List<GameObject>(); // UI ��� ���� ����Ʈ

    public void AddElement(CompassElementObject element)
    {
        compassElements.Add(element);

        // �̹��� ���� �� ElementContainer ������ �߰�
        GameObject newElement = new GameObject("CompassElement");
        Image image = newElement.AddComponent<Image>();
        RectTransform rectTransform = newElement.GetComponent<RectTransform>();

        image.sprite = element.ElementImage;

        // RectTransform ����
        rectTransform.SetParent(transform, false);
        rectTransform.localScale = Vector3.one; // ������ �ʱ�ȭ
        rectTransform.pivot = new Vector2(0.5f, 0.5f); // �ǹ� ����
        rectTransform.sizeDelta = element.compassAdderSize; // ũ�� ���� (�ʿ信 ���� ����)

        uiElements.Add(newElement); // UI ��� ����Ʈ�� �߰�
    }

    public void RemoveElement(CompassElementObject element)
    {
        int index = compassElements.IndexOf(element);
        if (index >= 0)
        {
            compassElements.RemoveAt(index);
            Destroy(uiElements[index]); // UI �̹��� ����
            uiElements.RemoveAt(index); // ����Ʈ���� ����
        }
    }

    void Update()
    {
        UpdateElements();
    }

    public void UpdateElements()
    {
        for (int i = 0; i < compassElements.Count; i++)
        {
            var element = compassElements[i];
            if (element != null && i < uiElements.Count)
            {
                // ������Ʈ�� ���� ��ġ�� ������
                Vector3 elementPosition = element.GetElementPosition();

                // ������Ʈ�� ���� ��ġ�� ȭ�� ��ǥ�� ��ȯ
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(elementPosition);

                // ȭ�� ��ǥ�� UI ��ǥ�� ��ȯ
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)transform,
                    screenPoint,
                    null,  // null�� Canvas�� Camera�� ������� �ʵ��� ����
                    out Vector2 uiPosition
                );

                // UI ����� ��ġ�� ������Ʈ
                uiElements[i].GetComponent<RectTransform>().anchoredPosition = uiPosition * new Vector3(1,0,1);
            }
        }
    }
}
