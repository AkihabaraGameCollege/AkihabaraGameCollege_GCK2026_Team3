namespace ForestDraw.Combat
{
    /// <summary>
    /// 回復を受け取れるオブジェクトの共通インターフェース
    /// </summary>
    public interface IHealable
    {
        void Heal(int amount);
    }
}