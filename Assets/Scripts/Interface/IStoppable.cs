namespace ForestDraw.Player.Combat
{
    /// <summary>
    /// 動きを止めることができるオブジェクトに実装するインターフェース
    /// </summary>
    public interface IStoppable
    {
        /// <summary>
        /// 指定した時間、動きを止める
        /// </summary>
        /// <param name="duration">停止する時間（秒）</param>
        void StopMovement(float duration);
    }
}