using System.Collections.Generic;
using UnityEngine;

public enum EKeyName
{
    None,
    Move_Forward,
    Move_Back,
    Move_Left,
    Move_Right,

    Sprint,
    Jump,
    Roll,
    Air_Dash,

    Interact,

    Attack,
    Skill1,
    Skill2,
    Skill3,
    Skill4,
}

public class PlayerInputManager : Singleton<PlayerInputManager>
{
    Dictionary<EKeyName, KeyCode> binds = new Dictionary<EKeyName, KeyCode>();

    void Start()
    {
        foreach (EKeyName keyName in System.Enum.GetValues(typeof(EKeyName)))
        {
            KeyCode defaultKey = GetDefaultKeyForAction(keyName);
            binds.Add(keyName, defaultKey);
        }
    }

    KeyCode GetDefaultKeyForAction(EKeyName action)
    {
        switch (action)
        {
            case EKeyName.Move_Forward:
                return KeyCode.W;
            case EKeyName.Move_Back:
                return KeyCode.S;
            case EKeyName.Move_Left:
                return KeyCode.A;
            case EKeyName.Move_Right:
                return KeyCode.D;
            case EKeyName.Sprint:
                return KeyCode.LeftShift;
            case EKeyName.Jump:
                return KeyCode.Space;
            case EKeyName.Roll:
                return KeyCode.LeftAlt;
            case EKeyName.Air_Dash:
                return KeyCode.LeftControl;
            case EKeyName.Interact:
                return KeyCode.G;
            case EKeyName.Attack:
                return KeyCode.Mouse0;
            case EKeyName.Skill1:
                return KeyCode.Alpha1;
            case EKeyName.Skill2:
                return KeyCode.Alpha2;
            case EKeyName.Skill3:
                return KeyCode.Alpha3;
            case EKeyName.Skill4:
                return KeyCode.Alpha4;
            //case EKeyName.Toggle_Morph_Screen:
            //    return KeyCode.R;
            default:
                return KeyCode.None;
        }
    }

    public bool IsKeyDown(EKeyName keyName)
    {
        return Input.GetKeyDown(binds[keyName]);
    }

    public bool IsKeyStay(EKeyName keyName)
    {
        return Input.GetKey(binds[keyName]);
    }

    public bool IsKeyUp(EKeyName keyName)
    {
        return Input.GetKey(binds[keyName]);
    }
}
