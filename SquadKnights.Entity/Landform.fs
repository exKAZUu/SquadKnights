namespace SquadKnights.Entity

/// 地形効果
type LandImpact =
    {
        /// HP補正
        LandHp : int
        /// 地形のATK補正
        LandAtk : int
        /// 地形のDEF補正
        LandDef : int
        /// 地形のSKL補正
        LandSkl : int
        /// 地形のAGI補正
        LandAgi : int
        /// 移動コスト
        MovCost : float
    }