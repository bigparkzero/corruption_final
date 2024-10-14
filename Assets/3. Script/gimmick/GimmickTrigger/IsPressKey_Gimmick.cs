using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPressKey_Gimmick : GimmickTrigger
{
    [Header("üũ�� Ű ����Ʈ")]
    public List<KeyCode> keyList; // ���� Ű ����Ʈ
    [Header("��� Ű�� ������ �Լ� ����")]
    public bool requireAllKeys; // true�̸� ��� Ű�� ������ ����
    [Header("������ �� ���� (üũ ������ ���� �� ����)")]
    public bool getKeyDown; // true�̸� ������ ��, false�̸� ���� �� ����

    private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>(); // ���� ���� Ű ����

    private void Update()
    {
        // ����Ʈ�� ���� Ű�� �������� üũ
        if (CheckForUnlistedKeyPress())
        {
            pressedKeys.Clear(); // ����Ʈ�� ���� Ű�� �������� �ʱ�ȭ
            return;
        }

        if (requireAllKeys)
        {
            if (getKeyDown)
            {
                // Ű�� �������� pressedKeys�� �߰�
                UpdatePressedKeysDown();
                if (AllKeysPressed()) // ��� Ű�� ���ȴ��� üũ
                {
                    InvokeEventRunOnTrigger();
                    pressedKeys.Clear(); // ���� �� �ʱ�ȭ
                }
            }
            else
            {
                // Ű�� �������� pressedKeys���� ����
                UpdatePressedKeysUp();
                if (AllKeysReleased()) // ��� Ű�� �������� üũ
                {
                    InvokeEventRunOnTrigger();
                    pressedKeys.Clear(); // ���� �� �ʱ�ȭ
                }
            }
        }
        else
        {
            if (getKeyDown)
            {
                if (AnyKeyPressedDown()) // ����Ʈ �� �ϳ��� ���ȴ��� üũ
                {
                    InvokeEventRunOnTrigger();
                }
            }
            else
            {
                if (AnyKeyPressedUp()) // ����Ʈ �� �ϳ��� �������� üũ
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
                pressedKeys.Add(key); // ���� Ű ����
            }
        }
    }

    private void UpdatePressedKeysUp()
    {
        foreach (KeyCode key in keyList)
        {
            if (Input.GetKeyUp(key))
            {
                pressedKeys.Remove(key); // �� Ű ����
            }
        }
    }

    private bool AllKeysPressed()
    {
        return pressedKeys.Count == keyList.Count; // ����� Ű ������ ����Ʈ�� Ű ���� ��
    }

    private bool AllKeysReleased()
    {
        return pressedKeys.Count == 0; // ��� Ű�� �������� ����� Ű�� ����� ��
    }

    private bool AnyKeyPressedDown()
    {
        foreach (KeyCode key in keyList)
        {
            if (Input.GetKeyDown(key))
            {
                return true; // �ϳ��� �������� true ��ȯ
            }
        }
        return false; // �ƹ� Ű�� ������ �ʾ����� false ��ȯ
    }

    private bool AnyKeyPressedUp()
    {
        foreach (KeyCode key in keyList)
        {
            if (Input.GetKeyUp(key))
            {
                return true; // �ϳ��� �������� true ��ȯ
            }
        }
        return false; // �ƹ� Ű�� ������ �ʾ����� false ��ȯ
    }

    private bool CheckForUnlistedKeyPress()
    {
        // ����Ʈ�� ���� Ű�� ���ȴ��� Ȯ��
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key) && !keyList.Contains(key))
            {
                return true; // ����Ʈ�� ���� Ű�� �������� true ��ȯ
            }
        }
        return false; // ��� ���� Ű�� ����Ʈ�� ������ false ��ȯ
    }
}
