using TMPro;
using UnityEngine;

public class SampleWorld : MonoBehaviour
{
    [Header("User")]
    [SerializeField] private TMP_Text _levelText;

    [Header("Contents")]
    [SerializeField] private ExpansionButton _expansionButton;
    [SerializeField] private ContentsPanel _contentsPanel;
    [SerializeField] private MailboxPanel _mailboxPanel;
    [SerializeField] private CodexPanel _codexPanel;
    [SerializeField] private DungeonPanel _dungeonPanel;
    [SerializeField] private EtcContentPanel _etcContentPanel;

    private readonly UserData _userData = new UserData();
    private readonly MailboxModel _mailboxModel = new MailboxModel();
    private readonly CodexModel _codexModel = new CodexModel();
    private readonly DungeonModel _dungeonModel = new DungeonModel();

    private void Start()
    {
        _contentsPanel.Bind();
        _mailboxPanel.Bind(_mailboxModel);
        _codexPanel.Bind(_codexModel);
        _dungeonPanel.Bind(_dungeonModel);

        _expansionButton.gameObject.SetActive(true);
        _expansionButton.SetOn(false);
        _contentsPanel.Close();
        _mailboxPanel.Close();
        _codexPanel.Close();
        _dungeonPanel.Close();
        _etcContentPanel.Close();

        RefreshLevelText();
    }

    private void OnEnable()
    {
        _expansionButton.AddClickListener(OnExpansionClicked);
        _contentsPanel.AddClickListener(OnContentsClicked);
        NotifyManager.OnMasterNotify += OnMasterNotify;
    }

    private void OnDisable()
    {
        _expansionButton.RemoveClickListener(OnExpansionClicked);
        _contentsPanel.RemoveClickListener(OnContentsClicked);
        NotifyManager.OnMasterNotify -= OnMasterNotify;
    }

    private void RefreshLevelText()
    {
        if (_levelText == null) {
            return;
        }

        _levelText.text = $"레벨 : {_userData.Level}";
    }

    private void OnMasterNotify(bool isOn)
    {
        _expansionButton.SetNotify(isOn);
    }

    private void OnExpansionClicked(bool isOn)
    {
        if (isOn) {
            _contentsPanel.Open();
        } else {
            _contentsPanel.Close();
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
                _dungeonPanel.Open();
                break;
            default:
                _etcContentPanel.Open(type);
                break;
        }
    }

    #region [ Cheat Protocol ]

    public void SendCheatMail(MailSlotType type, string content, ParcelItem itemType = ParcelItem.None)
    {
        _mailboxModel.OnMailProtocol(new MailProtocol(type, content, itemType));
    }

    public void SendCheatCodex(CodexTabType tab, string text)
    {
        _codexModel.OnCodexProtocol(tab, text);
    }

    public void SendCheatUserLevel(int level)
    {
        if (!_userData.TrySetLevel(level)) {
            return;
        }

        _dungeonModel.ChangeUserLevel(level);
        RefreshLevelText();
    }

    #endregion
}
