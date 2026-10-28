using UnityEditor;
using UnityEngine;

// 서버 역할을 대신하는 치트 창 (플레이 모드에서 프로토콜을 클라이언트로 전송)
public class CheatWindow : EditorWindow
{
    private int _level = 1;

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
            world.SetUserLevel(level);
        }
    }

    private static void SendMail(MailSlotType type, string content, ParcelItem itemType = ParcelItem.None)
    {
        if (TryFind(out MailboxPanel panel)) {
            panel.OnMailProtocol(new MailProtocol(type, content, itemType));
        }
    }

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
