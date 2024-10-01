using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class UiEditer : MonoBehaviour
{
    public RectTransform mainWindow;  // ���� �������� RectTransform
    public Vector2 targetPosition = new Vector2(-500f, 300f);  // ���� �����찡 �̵��� ��ġ
    public float duration = 1f;  // �ִϸ��̼� ��� �ð�

    private Coroutine openCoroutine;  // �ڷ�ƾ�� �����ϱ� ���� ����
    private Coroutine closeCoroutine;

    public bool isopen;

    public RectTransform anykeypresstext;


    public List<RectTransform> textlist;

    private void Update()
    {
        /*
        if (Input.anyKeyDown && !isopen)
        {
            mainwondowopen();
        }
       */
        if(Input.GetKeyDown(KeyCode.O))
            mainwondowopen();
        if (Input.GetKeyDown(KeyCode.C))
            mainwondowcloes();
    }

    public void mainwondowopen()
    {
        // �̹� ���� ���� �ڷ�ƾ�� ������ ����
        if (openCoroutine != null)
        {
            StopCoroutine(openCoroutine);
        }
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
        }

        // ���ο� �ڷ�ƾ ����
        openCoroutine = StartCoroutine(mainwondowopenCoroutine());
    }

    IEnumerator mainwondowopenCoroutine()
    {
        isopen = true;
        // �������� (0.78,0.78,0.78)���� (0.53,0.53,0.53)���� �ִϸ��̼�
        mainWindow.localScale = new Vector3(0.78f, 0.78f, 0.78f);  // �������� �ʱ�ȭ
        mainWindow.DOScale(new Vector3(0.53f, 0.53f, 0.53f), duration).SetEase(Ease.OutBack);  // ������ �ִϸ��̼�

        // ��ġ�� (0,0)���� targetPosition���� �̵�
        mainWindow.DOAnchorPos(targetPosition, duration).SetEase(Ease.InOutQuad);
        anykeypresstext.DOAnchorPos(Vector2.down * 1000, duration / 4).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(duration);  // �ִϸ��̼��� �Ϸ�� ������ ���
        for (int i = 0; textlist.Count > i; i++)
        {
            textlist[i].DOAnchorPos(new Vector2(-480, -10 - 80 *i), duration / 4).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(duration/16);
        }
    }

    public void mainwondowcloes()
    {
        // �̹� ���� ���� �ڷ�ƾ�� ������ ����
        if (openCoroutine != null)
        {
            StopCoroutine(openCoroutine);
        }
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
        }

        // ���ο� �ڷ�ƾ ����
        closeCoroutine = StartCoroutine(mainwondowcloesCoroutine());
    }

    IEnumerator mainwondowcloesCoroutine()
    {
        for (int i = 0; textlist.Count > i; i++)
        {
            textlist[i].DOAnchorPos(new Vector2(-1480,- 10 - 80 * i), duration / 4).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(duration / 16);
        }
        // �������� (0.53,0.53,0.53)���� (0.78,0.78,0.78)���� �ִϸ��̼�
        mainWindow.DOScale(new Vector3(0.78f, 0.78f, 0.78f), duration).SetEase(Ease.InBack);  // ������ �ִϸ��̼�

        // ��ġ�� targetPosition���� (0,0)���� �̵�
        mainWindow.DOAnchorPos(new Vector2(0,0), duration).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(duration);  // �ִϸ��̼��� �Ϸ�� ������ ���
        anykeypresstext.DOAnchorPos(Vector2.up * -300, duration / 4).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(duration / 4);  // �ִϸ��̼��� �Ϸ�� ������ ���
        isopen = false;
    }
}
