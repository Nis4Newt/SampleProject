public class UserData
{
    private int _level = 1;

    public int Level => _level;

    // 레벨은 올라가기만 한다. 올랐을 때만 true
    public bool TrySetLevel(int value)
    {
        if (value <= _level) {
            return false;
        }

        _level = value;
        return true;
    }
}
