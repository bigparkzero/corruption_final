using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassElementAdder : MonoBehaviour
{
    public List<CompassElementObject> compassElements = new List<CompassElementObject>();
    private List<GameObject> uiElements = new List<GameObject>(); // UI 요소 저장 리스트

    public void AddElement(CompassElementObject element)
    {
        compassElements.Add(element);

        // 이미지 생성 및 ElementContainer 하위에 추가
        GameObject newElement = new GameObject("CompassElement");
        Image image = newElement.AddComponent<Image>();
        RectTransform rectTransform = newElement.GetComponent<RectTransform>();

        image.sprite = element.ElementImage;

        // RectTransform 설정
        rectTransform.SetParent(transform, false);
        rectTransform.localScale = Vector3.one; // 스케일 초기화
        rectTransform.pivot = new Vector2(0.5f, 0.5f); // 피벗 설정
        rectTransform.sizeDelta = element.compassAdderSize; // 크기 설정 (필요에 따라 조정)

        uiElements.Add(newElement); // UI 요소 리스트에 추가
    }

    public void RemoveElement(CompassElementObject element)
    {
        int index = compassElements.IndexOf(element);
        if (index >= 0)
        {
            compassElements.RemoveAt(index);
            Destroy(uiElements[index]); // UI 이미지 제거
            uiElements.RemoveAt(index); // 리스트에서 제거
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
                // 오브젝트의 월드 위치를 가져옴
                Vector3 elementPosition = element.GetElementPosition();

                // 오브젝트의 월드 위치를 화면 좌표로 변환
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(elementPosition);

                // 화면 좌표를 UI 좌표로 변환
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)transform,
                    screenPoint,
                    null,  // null은 Canvas의 Camera를 사용하지 않도록 설정
                    out Vector2 uiPosition
                );

                // UI 요소의 위치를 업데이트
                uiElements[i].GetComponent<RectTransform>().anchoredPosition = uiPosition * new Vector3(1,0,1);
            }
        }
    }
}
