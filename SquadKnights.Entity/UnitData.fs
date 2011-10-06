namespace SquadKnights.Entity

open System.IO
open System.Drawing
open Paraiba.Drawing
open Paraiba.Drawing.Surfaces

open Settings
open Utility

type UnitData =
    {
        BaseName : string
        BaseKind : string;
        BaseMov : int;
        BaseHp : int;
        BaseAtk : int;
        BaseDef : int;
        BaseSkl : int;
        BaseAgi : int;
        BaseChip : Surface;
        BaseFace : Surface;
        DefaultAction : Action;
        SpecialAction : Action;
        SquadWidth : int;
        SquadMov : int;
        SquadAtk : int;
        SquadDef : int;
        SquadSkl : int;
        SquadAgi : int;
        SquadWt : int;
    }
