using ForestDraw.Enemy.Data;
using ForestDraw.Enemy.Components;
using UnityEngine;

namespace ForestDraw.Enemy
{
    /// <summary>
    /// 敵のコンポーネントを管理し、初期化と死亡処理を行うクラス
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        IEnemyComponent[] components;
        EnemyHealth health;
        [SerializeField] Canvas canvas;
        void Awake()
        {
            // Enemyコンポーネント取得
            components = GetComponents<IEnemyComponent>();
            health = GetComponent<EnemyHealth>();
        }

        private void Start()
        {
            // 死亡イベント登録
            if (health != null)
                health.Died += HandleDeath;
        }

        /// <summary>
        /// Spawnerから呼ばれる初期化処理
        /// </summary>
        public void Initialize(Transform[] path, Transform target, EnemyData data, Camera camera)
        {
            var context = new EnemyInitContext
            {
                path = path,
                target = target,
                data = data
            };

            foreach (var c in components)
                c.Initialize(context);
            canvas.worldCamera = camera;
        }

        /// <summary>
        /// 敵死亡時に呼ばれ、オブジェクトを削除する
        /// </summary>
        private void HandleDeath()
        {
            Destroy(gameObject, 0.05f);
        }
    }
}