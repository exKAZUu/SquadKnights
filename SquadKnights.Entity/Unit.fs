namespace SquadKnights.Entity

open Paraiba.Drawing
open Paraiba.Drawing.Surfaces

type Unit =
    {
        Hp : int;
        Atk : int;
        Def : int;
        Skl : int;
        Agi : int;
        Mov : int;
        Affiliation : Affiliation;
        SquadId : int;
        IsLeader : bool;
        Data : UnitData;
        Commands : Map<int, bool>
        UnitImage : Surface;
    }

    member this.IsLive
        with get () = this.Hp > 0
