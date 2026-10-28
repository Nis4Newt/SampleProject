using System;
using UnityEngine;

[Serializable]
public class UserData
{
    [Header("Level")]
    [SerializeField] private int _level = 1;

    public int Level => _level;

    public void SetLevel(int value)
    {
        _level = Mathf.Max(1, value);
    }
}
