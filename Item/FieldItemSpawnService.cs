using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    /// <summary>
    /// アイテムAグループ (PowerUp, DefenceUp) の出現サイクルとロジック。
    /// - 初回出現: ゲーム開始から27秒後
    /// - 次回出現: 取得または消滅（30秒）から27秒後
    /// - 交互スポーン: 前回出現しなかった方が必ず出現する
    /// </summary>
    public class FieldItemService
    {
        private EFieldItemType lastSpawnedItemA = EFieldItemType.GrenadePack; // 初期値ダミー

        public FieldItemService()
        {
            // アイテムAグループの候補
            var itemListA = new List<EFieldItemType>
            {
                EFieldItemType.PowerUpItem,
                EFieldItemType.DefenceUpItem
            };
            
            // 初回のみランダムに決定
            Random _random = new Random();
            int randomIndex = _random.Next(itemListA.Count);
            lastSpawnedItemA = itemListA[randomIndex];
        }

        /// <summary>
        /// 次に出現すべきアイテムAを選択する（交互スポーンロジック）
        /// 出現タイミング：取得または消滅から27秒後
        /// </summary>
        public EFieldItemType GenerateNextItemA()
        {
            // 前回出現したのがパワーアップなら次はディフェンスアップ、逆も然り
            lastSpawnedItemA = (lastSpawnedItemA == EFieldItemType.PowerUpItem) 
                ? EFieldItemType.DefenceUpItem 
                : EFieldItemType.PowerUpItem;

            return lastSpawnedItemA;
        }

        public EFieldItemType GenerateItemListA()
        {
            return GenerateNextItemA();
        }

        public EFieldItemType GenerateItemListB()
        {
            return EFieldItemType.GrenadePack;
        }

    }
}
