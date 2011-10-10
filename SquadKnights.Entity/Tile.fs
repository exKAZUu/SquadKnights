namespace SquadKnights.Entity

open Paraiba.Drawing
open Paraiba.Drawing.Surfaces

/// タイル（マップを構成する1マス）
type Tile =
    {
        /// タイル画像
        TileImage : Surface
        /// タイル種別
        TileKind : string
    }
