namespace SquadKnights.Entity

/// ゲーム状況
type War =
    {
        /// マップ
        Map : Map<ChipPoint, Tile>
        /// マップサイズ
        MapSize : ChipSize
        /// タイル種別とユニット種別の組み合わせと地形効果の対応表
        /// Tile.TileKind, UnitData.BaseKind -> LandImpact
        LandImpacts : Map<string * string, LandImpact>
        /// マップ上の位置とユニットの対応表
        Units : Map<ChipPoint, Unit>
        /// 分隊IDと分隊
        Squads : Map<int<squad_id>, Squad>
        /// ゲーム内時間
        Time : float<time>
    }
