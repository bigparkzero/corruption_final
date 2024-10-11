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
        }
    }

   
}
