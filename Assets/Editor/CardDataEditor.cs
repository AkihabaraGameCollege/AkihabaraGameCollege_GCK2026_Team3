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

        EditorGUILayout.LabelField("カード説明");
        card.description = EditorGUILayout.TextArea(card.description, GUILayout.Height(60));

        card.cost = EditorGUILayout.IntField("必要コスト", card.cost);
        card.useDuration = EditorGUILayout.FloatField("カードの使用時間", card.useDuration);
        card.cardImage = (Sprite)EditorGUILayout.ObjectField("画像", card.cardImage, typeof(Sprite), false);
        card.cardDetail_Image   = (Sprite)EditorGUILayout.ObjectField("詳細画像", card.cardDetail_Image, typeof(Sprite), false);// データオブジェクトに情報追加（中山が編集）
        card.usedSE = (AudioClip)EditorGUILayout.ObjectField("SE", card.usedSE, typeof(AudioClip), false);

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


        EditorGUILayout.Space();


        // カードタイプに応じて表示
        switch (card.cardType)
        {
            case CardType.Attack:
                EditorGUILayout.LabelField("Attack Parameters", EditorStyles.boldLabel);
                var attack = card.attackParams;
                attack.damage = EditorGUILayout.IntField("ダメージ", attack.damage);

                attack.attackEffect = (GameObject)EditorGUILayout.ObjectField("エフェクト", attack.attackEffect, typeof(GameObject), false);

                // 効果タイプ別の追加パラメータ
                switch (card.effectType)
                {
                    case CardEffectType.DamageLine:
                        attack.lineLength = EditorGUILayout.FloatField("長さ", attack.lineLength);
                        attack.lineWidth = EditorGUILayout.FloatField("横幅", attack.lineWidth);
                        break;

                    case CardEffectType.DamageArea:
                        attack.areaRadius = EditorGUILayout.FloatField("半径", attack.areaRadius);
                        attack.areaRange = EditorGUILayout.FloatField("前に出す距離", attack.areaRange);
                        break;

                    case CardEffectType.DamageSingle:
                    case CardEffectType.DamageAllOnScreen:
                        break;
                }
                break;

            case CardType.Recovery:
                EditorGUILayout.LabelField("recovery Parameters", EditorStyles.boldLabel);
                var recover = card.recoverParams;

                // エフェクトの設定（中山が追加）
                recover.SupportEffect = (GameObject)EditorGUILayout.ObjectField("エフェクト", recover.SupportEffect, typeof(GameObject), false);

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

                // エフェクトの設定（中山が追加）
                support.SupportEffect = (GameObject)EditorGUILayout.ObjectField("エフェクト", support.SupportEffect, typeof(GameObject), false);

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

                // エフェクトの設定（中山が追加）
                utility.SupportEffect = (GameObject)EditorGUILayout.ObjectField("エフェクト", utility.SupportEffect, typeof(GameObject), false);

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