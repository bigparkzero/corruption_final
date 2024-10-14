using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPressKey_Gimmick : GimmickTrigger
{
    [Header("체크할 키 리스트")]
    public List<KeyCode> keyList; // 누를 키 리스트
    [Header("모든 키를 눌러야 함수 실행")]
    public bool requireAllKeys; // true이면 모든 키를 눌러야 실행
    [Header("눌렀을 때 실행 (체크 해제시 뗐을 때 실행)")]
    public bool getKeyDown; // true이면 눌렀을 때, false이면 뗐을 때 실행

    private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>(); // 현재 눌린 키 저장

    private void Update()
    {
        // 리스트에 없는 키를 눌렀는지 체크
        if (CheckForUnlistedKeyPress())
        {
            pressedKeys.Clear(); // 리스트에 없는 키가 눌렸으면 초기화
            return;
        }

        if (requireAllKeys)
        {
            if (getKeyDown)
            {
                // 키가 눌렸으면 pressedKeys에 추가
                UpdatePressedKeysDown();
                if (AllKeysPressed()) // 모든 키가 눌렸는지 체크
                {
                    InvokeEventRunOnTrigger();
                    pressedKeys.Clear(); // 실행 후 초기화
                }
            }
            else
            {
                // 키가 떼졌으면 pressedKeys에서 제거
                UpdatePressedKeysUp();
                if (AllKeysReleased()) // 모든 키가 떼졌는지 체크
                {
                    InvokeEventRunOnTrigger();
                    pressedKeys.Clear(); // 실행 후 초기화
                }
            }
        }
        else
        {
            if (getKeyDown)
            {
                if (AnyKeyPressedDown()) // 리스트 중 하나라도 눌렸는지 체크
                {
                    InvokeEventRunOnTrigger();
                }
            }
            else
            {
                if (AnyKeyPressedUp()) // 리스트 중 하나라도 떼졌는지 체크
                {
                    InvokeEventRunOnTrigger();
                }
            }
        }
    }

    private void UpdatePressedKeysDown()
    {
        foreach (KeyCode key in keyList)
        {
            if (Input.GetKeyDown(key))
            {
                pressedKeys.Add(key); // 누른 키 저장
            }
        }
    }

    private void UpdatePressedKeysUp()
    {
        foreach (KeyCode key in keyList)
        {
            if (Input.GetKeyUp(key))
            {
                pressedKeys.Remove(key); // 뗀 키 제거
            }
        }
    }

    private bool AllKeysPressed()
    {
        return pressedKeys.Count == keyList.Count; // 저장된 키 개수와 리스트의 키 개수 비교
    }

    private bool AllKeysReleased()
    {
        return pressedKeys.Count == 0; // 모든 키가 떼졌으면 저장된 키는 없어야 함
    }

    private bool AnyKeyPressedDown()
    {
        foreach (KeyCode key in keyList)
        {
            if (Input.GetKeyDown(key))
            {
                return true; // 하나라도 눌렸으면 true 반환
            }
        }
        return false; // 아무 키도 눌리지 않았으면 false 반환
    }

    private bool AnyKeyPressedUp()
    {
        foreach (KeyCode key in keyList)
        {
            if (Input.GetKeyUp(key))
            {
                return true; // 하나라도 떼졌으면 true 반환
            }
        }
        return false; // 아무 키도 떼지지 않았으면 false 반환
    }

    private bool CheckForUnlistedKeyPress()
    {
        // 리스트에 없는 키가 눌렸는지 확인
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key) && !keyList.Contains(key))
            {
                return true; // 리스트에 없는 키가 눌렸으면 true 반환
            }
        }
        return false; // 모든 눌린 키가 리스트에 있으면 false 반환
    }
}
