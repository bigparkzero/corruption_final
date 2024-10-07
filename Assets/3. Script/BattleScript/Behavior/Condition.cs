using System;
using System.Collections.Generic;
using UnityEngine;

public enum EVariableType
{
    None = 0,

    Distance,
    Target,
    Attacking,
    AngleToTarget,
}

public enum EComparison
{
    None = 0,

    EqualTo,
    GreaterThan,
    LessThan,
}

[Serializable]
public struct CheckValue<T>
{
    [Header("[Check Value]")]
    public EVariableType variableType;
    public EComparison comparison;
    public T value;
}

[Serializable]
public struct Condition
{
    [Header("[Condition]")]
    public List<CheckValue<bool>> checkBool;
    public List<CheckValue<int>> checkInt;
    public List<CheckValue<float>> checkFloat;
}