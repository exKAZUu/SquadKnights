namespace SquadKnights.Entity

type War =
    {
        Map : Map<ChipPoint, Tile>;
        MapSize : ChipSize;
        /// TileKind, Data.BaseKind -> Landform
        Landforms : Map<string * string, Landform>;
        Units : Map<ChipPoint, Unit>
        Squads : Map<int<squad_id>, Squad>
        Time : float<time>;
    }
