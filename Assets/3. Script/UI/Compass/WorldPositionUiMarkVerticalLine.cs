using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WorldPositionUiMarkVerticalLine : MonoBehaviour
{
    LineRenderer line;

    // ���̸� ��� �������� ������ ���ϱ� ���� ����
    public Transform rayOrigin;
    public float rayLength = 100f;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        // ���η����� �ʱ� ����
        line.positionCount = 2;
    }
    private void Update()
    {
        // ����ĳ��Ʈ�� �߻�
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            // ����ĳ��Ʈ �ð�ȭ (���� �������� ���)
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hit.point);

            // ���̰� ���� ��ġ�� �޽� �̵� (���ؽ� ������ ����)
        }
    }

   
}
