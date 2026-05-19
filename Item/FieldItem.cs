using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    /// <summary>
    /// フィールドアイテム: マップ上の特定の地点に直接出現し、プレイヤーが触れることで取得・使用できるアイテム。
    /// 効果持続時間は原則として 30秒。
    /// </summary>
    public enum EFieldItemType
    {
        GranadeLauncher,    // 特殊武器: グレネードランチャー
        FlameThrower,       // 特殊武器: 火炎放射器
        PowerUpItem,        // 攻撃力2倍 (30秒)
        DefenceUpItem,      // 防御力2倍 (30秒)
        SpeedUpItem,        // 移動速度2倍 (30秒)
        StealthItem,        // キャラ半透明化 (30秒)
        GrenadePack,        // ノーマルグレネード満タン補充
        HealItem            // HP回復
    }






}
