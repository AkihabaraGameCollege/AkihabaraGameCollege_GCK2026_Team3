namespace ForestDraw.Combat
{
    /// <summary>
    /// ダメージを受け取れるオブジェクトの共通インターフェース
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int amount);
    }
}