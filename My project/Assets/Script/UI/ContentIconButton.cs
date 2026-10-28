using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ContentIconButton : MonoBehaviour
{
    [Header("Content")]
    [SerializeField] private ContentsType _type;
    [SerializeField] private GameObject _noti;

    private Button _button;

    public ContentsType Type => _type;
    public Button Button
    {
        get
        {
            if (_button != null) {
                return _button;
            }

            _button = GetComponent<Button>();
            return _button;
        }
    }

    private void Awake()
    {
        _noti.SetActive(false);
    }
}
