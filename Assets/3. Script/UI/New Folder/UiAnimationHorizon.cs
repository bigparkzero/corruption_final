using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiAnimationHorizon : MonoBehaviour
{
    public RectTransform uiElement; // �̵���ų UI ���
    public float moveDistance = 500f; // �̵� �Ÿ�
    public float duration = 1f;      // �̵� �ð�
    public float minDelay = 0.5f;    // �ּ� ���� �ð�
    public float maxDelay = 2f;      // �ִ� ���� �ð�

    void Start()
    {
        // ���ʿ��� ���������� �̵� �� �ٽ� ���� �����̷� �ǵ��ƿ��� �ִϸ��̼�
        MoveUI(uiElement);
    }

    void MoveUI(RectTransform uiElement)
    {
        // UI�� ���� ��ġ�� �������� ���������� �̵�
        uiElement.DOAnchorPosX(moveDistance, duration).SetEase(Ease.InOutQuad)
            // ���������� �̵��� �� �ٽ� �������� �ǵ��ư�
            .OnComplete(() =>
            {
                // �������� �ǵ��ƿ��� ���� ���� ������ ����
                float randomDelay = Random.Range(minDelay, maxDelay);
                uiElement.DOAnchorPosX(-moveDistance, duration).SetEase(Ease.InOutQuad)
                    .SetDelay(randomDelay) // ���� ���� �ð� �߰�
                    .OnComplete(() =>
                    {
                        // �ٽ� ���� �����̷� �̵� �ݺ�
                        randomDelay = Random.Range(minDelay, maxDelay);
                        MoveUI(uiElement);
                    });
            });
    }
}
