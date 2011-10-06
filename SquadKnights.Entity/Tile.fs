namespace SquadKnights.Entity

open Paraiba.Drawing
open Paraiba.Drawing.Surfaces

type Tile =
    {
        TileImage : Surface;
        TileKind : string;
    }
