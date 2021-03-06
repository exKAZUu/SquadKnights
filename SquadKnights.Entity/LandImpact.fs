﻿namespace SquadKnights.Entity

/// 地形効果
type LandImpact =
    {
        /// 地形によるステータス補正（HPは回復量）
        LandRev : Status
        /// 移動コスト
        MovCost : float
    }