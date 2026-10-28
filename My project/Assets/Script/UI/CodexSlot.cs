using TMPro;
using UnityEngine;

public class CodexSlot : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text _text;

    public void SetText(string value)
    {
        _text.text = value;
    }
}
