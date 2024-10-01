using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    public RectTransform InformationWindow;
    public TextMeshProUGUI InformationWindowText;
    public float basicduration;
    public bool textruning;

    private void Start()
    {
        InformationWindow.localScale = new Vector3(0, 1, 1);  // X���� 0���� ����
    }
    private void OnValidate()
    {
        if (InformationWindow != null)
        {
            TextMeshProUGUI FindTMP = InformationWindow.GetComponentInChildren<TextMeshProUGUI>(true);
            if (FindTMP != null)
                InformationWindowText = FindTMP;
        }
    }

    public void SetBasicDuration(float duration)
    {
        basicduration = duration;
    }
    public void SetTextStringGimmick(string talk)
    {
        SetTextString(talk);
    }
    public void SetTextString(string talk, float duration = 0)
    {
        textruning = true;
        WindowIntro();

        InformationWindowText.text = talk;
        InformationWindowText.maxVisibleCharacters = 0;
        float typingDuration = duration == 0 ? basicduration : duration;

        // DOTween�� ����� Ÿ���� ȿ��
        DOTween.To(() => InformationWindowText.maxVisibleCharacters,
                    x => InformationWindowText.maxVisibleCharacters = (int)x,
                    InformationWindowText.text.Length, typingDuration).OnComplete(() =>
                    {
                        // Ÿ������ ���� �� 5�� �Ŀ� �ƿ�Ʈ�� ����
                        Invoke("WindowOutroV", 5f);
                        textruning = false;
                    });
    }
    private void Update()
    {
        if (!textruning && Input.GetKeyDown(KeyCode.G))
        {
            WindowOutroV();
        }
    }
    // ��Ʈ�� �ڷ�ƾ (X�� �������� õõ�� 0���� 1�� ����)
    void WindowIntro()
    {
        StopCoroutine(WindowOutro());
        InformationWindow.localScale = new Vector3(0, 1, 1);  // X���� 0���� ����
        InformationWindow.DOScaleX(1, 0.15f);  // 0.5�� ���� X�� �������� 1��
    }

    // �ƿ�Ʈ�� ����
    void WindowOutroV()
    {
        StopCoroutine(WindowOutro());
        StartCoroutine(WindowOutro());
    }

    // �ƿ�Ʈ�� �ڷ�ƾ (X�� �������� ���� �ٿ��� �������)
    IEnumerator WindowOutro()
    {
        float awefrakuhg = basicduration / (InformationWindowText.text.Length*10);
        while (InformationWindowText.text.Length > 0)
        {
            InformationWindowText.text = InformationWindowText.text.Substring(0, InformationWindowText.text.Length - 1); // ������ ���� ����
            yield return new WaitForSeconds(awefrakuhg);  // ���� �ð� ���

        }
        InformationWindow.DOScaleX(0, 0.15f);  // 0.5�� ���� X�� �������� 0���� ���̱�
    }
}
