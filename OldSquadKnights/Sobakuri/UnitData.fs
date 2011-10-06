namespace Sobakuri.Entity

open System.IO
open System.Drawing
open Paraiba.Drawing
open Paraiba.Drawing.Surfaces

open Settings
open Utility
open ImageManager

type UnitData =
    {
        BaseName : string;
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
        RevWidth : int;
        RevMov : int;
        RevAtk : int;
        RevDef : int;
        RevSkl : int;
        RevAgi : int;
        RevWt : int;
    }
