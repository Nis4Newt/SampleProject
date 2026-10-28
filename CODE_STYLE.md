# 코드 스타일 가이드

C# 코드 작성 규칙. 중괄호 규칙은 `.editorconfig`에 반영되어 있어 VS Code 저장 시 자동으로 맞춰진다.

## 1. 중괄호 `{}`

| 위치 | 스타일 | 여는 중괄호 |
| --- | --- | --- |
| 클래스, 구조체, 인터페이스, enum | 올맨(Allman) | 새 줄 |
| 함수, 프로퍼티(접근자 포함) | 올맨(Allman) | 새 줄 |
| 함수 내부 (`if`, `for`, `foreach`, `while`, `switch`, `try` 등) | K&R | 같은 줄 |

- `else`, `catch`, `finally`는 닫는 중괄호와 같은 줄에 쓴다. (`} else {`)
- 실행문이 한 줄이어도 중괄호를 **항상** 붙인다.
- 데이터 초기화(`new Dictionary<...> { ... }`)는 위 규칙의 대상이 아니며 기존 모양을 유지한다.

```csharp
public class Sample
{
    private int _count;

    public void Add(int value)
    {
        if (value <= 0) {
            return;
        }

        for (int i = 0; i < value; i++) {
            _count++;
        }

        if (_count > 10) {
            Debug.Log("many");
        } else {
            Debug.Log("few");
        }
    }
}
```

## 2. 네이밍

| 대상 | 규칙 | 예시 |
| --- | --- | --- |
| 클래스, 구조체, 인터페이스, enum | PascalCase (인터페이스는 `I` 접두사) | `MailboxPanel`, `IPanel` |
| 함수 | PascalCase | `OnMailProtocol`, `RefreshBanners` |
| 프로퍼티, public 필드 | PascalCase | `Level`, `Type`, `Content` |
| 클래스 내부 변수(필드) | `_camelCase` | `_levelText`, `_currentTab` |
| 함수 내 임시 변수, 매개변수 | camelCase | `tabIndex`, `slot` |

```csharp
public class MailboxPanel : BasePanel
{
    [SerializeField] private TMP_Text _titleText;
    private int _currentTab;

    public int CurrentTab => _currentTab;

    private void Refresh(int tabIndex)
    {
        int count = GetCount(tabIndex);
        _currentTab = tabIndex;
    }
}
```

## 3. 접근 제한자

- 클래스, 필드, 함수, 프로퍼티 등 모든 멤버에 접근 제한자(`public`, `private`, `protected`, `internal`)를 **명시**한다. 기본값(생략 시 `private`/`internal`)에 의존하지 않는다.
- `.editorconfig`의 `dotnet_style_require_accessibility_modifiers = always:warning`으로 검사한다.

```csharp
public class Sample
{
    private int _count;      // O
    int _total;              // X (접근 제한자 생략)

    private void Run() { }   // O
    void Stop() { }          // X
}
```

### `[SerializeField]` 필드

- `[SerializeField]`가 붙어도 private 필드이므로 `_camelCase`를 따른다. (`[SerializeField] private Button _closeButton;`)
- public 직렬화 필드는 쓰지 않는다. 밖에서 읽을 값은 프로퍼티로 노출한다. (`public Button CloseButton => _closeButton;`)
- 필드 이름을 바꾸면 씬/프리팹에 연결된 참조와 직렬화 값이 끊긴다. 코드에서는 `[FormerlySerializedAs]` 등으로 대응하지 않고, 에디터에서 수동으로 다시 연결한다.
