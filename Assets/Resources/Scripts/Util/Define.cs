public class Define
{
    public enum PlayerState
    {
        Appear,
        Attack,
        Dead,
        Idle,
        Walk,
        Jump,
        Die,
        Hit,
    }

    public enum KeyboardEvent
    {
        None,
        Right,
        Left,
        Space,
        Shift,
    }

    public enum KeyInputType
    {
        Hold,
        Down,
    }

    public enum MouseEvent
    {
        None,
        LClick,
        RClick
    }

    public enum SoundType
    {
        Bgm = 0,
        Effect = 1,
        Count = 2
    }

    public enum EffectSoundType
    {
        UI_Show,
        UI_Click,
        UI_Shop,
        P1_Appear,
        P2_Appear,
        P3_Appear,
        PlayerHit,
        PlayerDead,
        CheckPointEnter,
        SpearmanAttack
    }

    public enum SceneType
    {
        Unknown,
        Title,
        Lobby,
        Stage,
    }

    public enum UIEvent
    {
        Click,
        PointerEnter,
        PointerExit
    }

    public enum UIAnimType
    {
        None,
        Scale,
        Fade
    }

    public enum Stage
    {
        None,
        Stage11 = 11, Stage12, Stage13, Stage14, Stage15, Stage16,
        Stage21 = 21, Stage22, Stage23, Stage24, Stage25, Stage26,
    }

    public enum ItemType
    {
        None,
        Consumable,
        Equipment
    }

    public enum EnemyType
    {
        None,
        Spearman
    }

    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Hit,
        Die
    }
}
