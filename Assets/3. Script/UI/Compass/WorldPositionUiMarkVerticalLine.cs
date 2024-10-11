using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WorldPositionUiMarkVerticalLine : MonoBehaviour
{
    LineRenderer line;

    // 레이를 쏘는 시작점과 방향을 정하기 위한 변수
    public Transform rayOrigin;
    public float rayLength = 100f;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        // 라인렌더러 초기 설정
        line.positionCount = 2;
    }
    private void Update()
    {
        // 레이캐스트를 발사
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            // 레이캐스트 시각화 (라인 렌더러를 사용)
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hit.point);

            // 레이가 맞은 위치로 메쉬 이동 (버텍스 데이터 변경)
            MoveMeshToPosition(hit.point);
        }
    }

    // 메쉬의 버텍스 데이터를 레이 충돌 위치로 이동시키는 함수
    private void MoveMeshToPosition(Vector3 targetPosition)
    {
        // 메쉬 데이터를 가져옴
        Mesh mesh = meshF.mesh;
        Vector3[] vertices = mesh.vertices;

        // 버텍스 이동을 위한 오프셋 계산
        Vector3 offset = targetPosition - transform.position;

        // 각 버텍스 위치에 오프셋을 적용하여 이동
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += offset;
        }

        // 변경된 버텍스 데이터를 메쉬에 다시 적용
        mesh.vertices = vertices;

        // 메쉬를 업데이트하여 변형된 것이 반영되도록 함
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
