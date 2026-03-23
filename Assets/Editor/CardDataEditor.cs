using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ForestDraw;

[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CardData card = (CardData)target;

        // ── 共通フィールド ──
        card.cardId = EditorGUILayout.TextField("Card Id", card.cardId);
        card.cardName = EditorGUILayout.TextField("Card Name", card.cardName);
        card.cardType = (CardType)EditorGUILayout.EnumPopup("Card Type", card.cardType);

        // CardType に応じて EffectType を制限
        CardEffectType[] availableEffects;

        switch (card.cardType)
        {
            case CardType.Attack:
                availableEffects = new CardEffectType[]
                {
                    CardEffectType.DamageSingle,
                    CardEffectType.DamageLine,
                    CardEffectType.DamageArea,
                    CardEffectType.DamageAllOnScreen
                };
                break;

            case CardType.Support:
                availableEffects = new CardEffectType[]
                {
                    CardEffectType.BuffNext,
                    CardEffectType.Shield,
                    CardEffectType.DamageReduction
                };
                break;

            case CardType.Recovery:
                availableEffects = new CardEffectType[]
                {
                    CardEffectType.CostRecover,
                    CardEffectType.Heal,
                    CardEffectType.CostRegen
                };
                break;
            case CardType.Utility:
                availableEffects = new CardEffectType[]
                {
                    CardEffectType.Draw
                };
                break;

            default:
                availableEffects = Enum.GetValues(typeof(CardEffectType))
                                       .Cast<CardEffectType>()
                                       .ToArray();
                break;
        }

        // EnumPopup で制限付き表示
        int currentIndex = Array.IndexOf(availableEffects, card.effectType);
        if (currentIndex < 0) currentIndex = 0; // 現在の effectType が配列外なら先頭を選択

        currentIndex = EditorGUILayout.Popup("Card Effect Type",
                                             currentIndex,
                                             availableEffects.Select(e => e.ToString()).ToArray());

        card.effectType = availableEffects[currentIndex];


        EditorGUILayout.LabelField("カード説明");
        card.description = EditorGUILayout.TextArea(card.description, GUILayout.Height(60));

        card.cost = EditorGUILayout.IntField("必要コスト", card.cost);
        card.useDuration = EditorGUILayout.FloatField("カードの使用時間", card.useDuration);
        card.cardImage = (Sprite)EditorGUILayout.ObjectField("画像", card.cardImage, typeof(Sprite), false);

        EditorGUILayout.Space();


        // カードタイプに応じて表示
        switch (card.cardType)
        {
            case CardType.Attack:
                EditorGUILayout.LabelField("Attack Parameters", EditorStyles.boldLabel);
                var attack = card.attackParams;
                attack.damage = EditorGUILayout.IntField("ダメージ", attack.damage);

                // 効果タイプ別の追加パラメータ
                switch (card.effectType)
                {
                    case CardEffectType.DamageLine:
                        attack.lineLength = EditorGUILayout.FloatField("長さ", attack.lineLength);
                        attack.lineWidth = EditorGUILayout.FloatField("横幅", attack.lineWidth);
                        break;

                    case CardEffectType.DamageArea:
                        attack.areaRadius = EditorGUILayout.FloatField("半径", attack.areaRadius);
                        break;

                    case CardEffectType.DamageSingle:
                    case CardEffectType.DamageAllOnScreen:
                        break;
                }
                break;

            case CardType.Recovery:
                EditorGUILayout.LabelField("recovery Parameters", EditorStyles.boldLabel);
                var recover = card.recoverParams;

                // 効果タイプ別の追加パラメータ
                switch (card.effectType)
                {
                    case CardEffectType.CostRecover:
                        recover.costRecoverAmount = EditorGUILayout.IntField("コストの回復量", recover.costRecoverAmount);
                        break;

                    case CardEffectType.CostRegen:
                        recover.intervalReduction = EditorGUILayout.IntField("コストの回復速度", recover.intervalReduction);
                        card.buffDuration = EditorGUILayout.IntField("効果時間", card.buffDuration);
                        break;

                    case CardEffectType.Heal:
                        recover.healAmount = EditorGUILayout.IntField("HPの回復量", recover.healAmount);
                        break;
                }
                break;

            case CardType.Support:
                EditorGUILayout.LabelField("Support Parameters", EditorStyles.boldLabel);
                var support = card.supportParams;

                // 効果タイプ別の追加パラメータ
                switch (card.effectType)
                {
                    case CardEffectType.DamageReduction:
                        support.damageReduction = EditorGUILayout.FloatField("ダメージ軽減率", support.damageReduction);
                        card.buffDuration = EditorGUILayout.IntField("効果時間", card.buffDuration);
                        break;
                    case CardEffectType.BuffNext:
                        support.attackMultiplier = EditorGUILayout.FloatField("ダメージ倍率", support.attackMultiplier);
                        break;
                }
                break;
            case CardType.Utility:
                EditorGUILayout.LabelField("Support Parameters", EditorStyles.boldLabel);
                var utility = card.utilityParams;

                // 効果タイプ別の追加パラメータ
                switch (card.effectType)
                {
                    case CardEffectType.Draw:
                        utility.drawCount = EditorGUILayout.IntField("ドロー枚数", utility.drawCount);
                        break;
                }
                break;

        }

        // 保存
        if (GUI.changed)
        {
            EditorUtility.SetDirty(card);
        }
    }
}