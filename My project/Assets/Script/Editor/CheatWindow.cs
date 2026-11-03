using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// 서버 역할을 대신하는 치트 창 (플레이 모드에서 프로토콜을 클라이언트로 전송)
public class CheatWindow : EditorWindow
{
    private int _level = 1;
    private readonly Dictionary<CodexTabType, HashSet<string>> _sentCodexTexts =
        new Dictionary<CodexTabType, HashSet<string>>();

    [MenuItem("Tool/Cheat")]
    private static void Open()
    {
        GetWindow<CheatWindow>("Cheat");
    }

    private void OnGUI()
    {
        GUILayout.Label("Mail Protocol", EditorStyles.boldLabel);

        if (GUILayout.Button("메일 수신")) {
            SendMail(MailSlotType.Mail, $"안녕하세요. 신규 메일입니다! ({Now()})");
        }

        if (GUILayout.Button("택배 수신")) {
            SendMail(MailSlotType.Parcel, $"똑똑똑. 신규 택배왔습니다! ({Now()})", GetRandomParcelItem());
        }

        GUILayout.Space(10);
        GUILayout.Label("Codex Protocol", EditorStyles.boldLabel);

        if (GUILayout.Button("숫자 도감 수신")) {
            SendCodex(CodexTabType.Number);
        }

        if (GUILayout.Button("글자 도감 수신")) {
            SendCodex(CodexTabType.Korean);
        }

        if (GUILayout.Button("영어 도감 수신")) {
            SendCodex(CodexTabType.English);
        }

        GUILayout.Space(10);
        GUILayout.Label("User Level", EditorStyles.boldLabel);

        _level = EditorGUILayout.IntField("레벨", _level);
        if (GUILayout.Button("레벨 변경")) {
            SendLevel(_level);
        }

        if (!Application.isPlaying) {
            EditorGUILayout.HelpBox("플레이 모드에서만 실행할 수 있습니다.", MessageType.Info);
        }
    }

    private static void SendLevel(int level)
    {
        if (TryFind(out SampleWorld world)) {
            world.SendCheatUserLevel(level);
        }
    }

    private static void SendMail(MailSlotType type, string content, ParcelItem itemType = ParcelItem.None)
    {
        if (TryFind(out SampleWorld world)) {
            world.SendCheatMail(type, content, itemType);
        }
    }

    // 아직 보내지 않은 도감 데이터 중 하나를 랜덤으로 보낸다. 모두 보냈으면 경고만 남긴다.
    private void SendCodex(CodexTabType tab)
    {
        if (!TryFind(out SampleWorld world)) {
            return;
        }

        if (!_sentCodexTexts.TryGetValue(tab, out var sentTexts)) {
            sentTexts = new HashSet<string>();
            _sentCodexTexts[tab] = sentTexts;
        }

        string[] allTexts = CodexModel.GetTexts(tab);
        var unsentTexts = allTexts.Where(text => !sentTexts.Contains(text)).ToList();
        if (unsentTexts.Count == 0) {
            Debug.LogWarning($"Cheat: {tab} 도감 데이터를 모두 보냈습니다.");
            return;
        }

        string picked = unsentTexts[Random.Range(0, unsentTexts.Count)];
        sentTexts.Add(picked);
        world.SendCheatCodex(tab, picked);
        Debug.Log($"Cheat: {tab} 도감 전송 '{picked}' ({sentTexts.Count}/{allTexts.Length})");
    }

    // 플레이 모드 진입 시 전송 기록을 비운다 (도메인 리로드를 꺼도 매 플레이마다 초기화)
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode) {
            _sentCodexTexts.Clear();
        }
    }

    private void OnEnable() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    private void OnDisable() => EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

    private static bool TryFind<T>(out T target) where T : Object
    {
        target = null;

        if (!Application.isPlaying) {
            Debug.LogWarning("Cheat: 플레이 모드에서만 실행할 수 있습니다.");
            return false;
        }

        target = FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (target == null) {
            Debug.LogWarning($"Cheat: 씬에서 {typeof(T).Name}을(를) 찾을 수 없습니다.");
            return false;
        }

        return true;
    }

    private static ParcelItem GetRandomParcelItem()
    {
        return (ParcelItem)Random.Range((int)ParcelItem.First, (int)ParcelItem.Both + 1);
    }

    private static string Now() => System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
