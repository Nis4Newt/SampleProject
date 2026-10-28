using TMPro;
using UnityEngine;

public class SampleWorld : MonoBehaviour
{
    [Header("User")]
    [SerializeField] private TMP_Text _levelText;


    [Header("Contents")]
    [SerializeField] private ExpansionButton _expansionButton;
    [SerializeField] private ContentsPanel _expansionPanel;
    [SerializeField] private MailboxPanel _mailboxPanel;
    [SerializeField] private CodexPanel _codexPanel;
    [SerializeField] private DungeonPanel _dungeonPanel;
    [SerializeField] private EtcContentPanel _etcContentPanel;

    private UserData _UserData = new UserData();

    private void Awake()
    {
        _expansionButton.gameObject.SetActive(true);
        _expansionButton.SetOn(false);
        _expansionPanel.Close();
        _mailboxPanel.Close();
        _codexPanel.Close();
        _dungeonPanel.Close();
        _etcContentPanel.Close();

        RefreshLevelText();
    }

    private void OnEnable()
    {
        _expansionButton.AddClickListener(OnExpansionClicked);
        _expansionPanel.AddClickListener(OnContentsClicked);
    }

    private void OnDisable()
    {
        _expansionButton.RemoveClickListener(OnExpansionClicked);
        _expansionPanel.RemoveClickListener(OnContentsClicked);
    }

    public void SetUserLevel(int level)
    {
        if (level <= _UserData.Level) {
            return;
        }

        _UserData.SetLevel(level);
        RefreshLevelText();
        _dungeonPanel.SetUserLevel(_UserData.Level);
    }

    private void RefreshLevelText()
    {
        if (_levelText == null) {
            return;
        }

        _levelText.text = $"레벨 : {_UserData.Level}";
    }

    private void OnExpansionClicked(bool isOn)
    {
        if (isOn) {
            _expansionPanel.Open();
        } else {
            _expansionPanel.Close();
        }
    }

    private void OnContentsClicked(ContentsType type)
    {
        _expansionButton.SetOn(false);

        switch (type) {
            case ContentsType.Mail:
                _mailboxPanel.Open();
                break;
            case ContentsType.Codex:
                _codexPanel.Open();
                break;
            case ContentsType.Dungeon:
                _dungeonPanel.SetUserLevel(_UserData.Level);
                _dungeonPanel.Open();
                break;
            default:
                _etcContentPanel.Open(type);
                break;
        }
    }
}
