using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassScript : MonoBehaviour
{
    public RectTransform CompassBarPanel;
    public RawImage Compass;
    public RectTransform CompassBarElementContainer;
    public Transform player;

    private float CompassBarCurrentDegrees;

    private void Update()
    {
        // �÷��̾��� ȸ������ �̿��Ͽ� ���۽� ���� ������Ʈ
        UpdateCompassBar(player);
    }

    public void InitCompassBar()
    {
        // check references
        if (CompassBarPanel == null || Compass == null || CompassBarElementContainer == null)
        {
            return;
        }

        // show compass bar
        ShowCompassBar(true);
    }

    public void ShowCompassBar(bool value)
    {
        if (CompassBarPanel != null)
            CompassBarPanel.gameObject.SetActive(value);
    }

    public void UpdateCompassBar(Transform rotationReference)
    {
        // set compass bar texture coordinates (�÷��̾��� Y�� ȸ���� �������� ���۽� �̵�)
        Compass.uvRect = new Rect((rotationReference.eulerAngles.y / 360f) - .5f, 0f, 1f, 1f);

        // calculate 0-360 degrees value
        Vector3 perpDirection = Vector3.Cross(Vector3.forward, rotationReference.forward);
        float angle = Vector3.Angle(new Vector3(rotationReference.forward.x, 0f, rotationReference.forward.z), Vector3.forward);
        CompassBarCurrentDegrees = (perpDirection.y >= 0f) ? angle : 360f - angle;

        // ���۽� ���� ����� ���
        Debug.Log("Compass Bar Current Degrees: " + CompassBarCurrentDegrees);
    }
}
