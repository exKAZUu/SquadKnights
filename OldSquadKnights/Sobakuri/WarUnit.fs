namespace Sobakuri.Entity

open System.IO
open Paraiba.Drawing
open Paraiba.Drawing.Surfaces

open Settings
open Utility
open ImageManager

type Squad =
    { mutable Wt : float; IsFirst : bool }

and Chip(s : Surface, kind : string, u : Option<WarUnit>) =
    let mutable _surface = s
    let mutable _kind = kind
    let mutable _unit = u

    member this.Surface
        with get () = _surface
        and set value = _surface <- value

    member this.Kind
        with get () = _kind
        and set value = _kind <- value

    member this.Unit
        with get () = _unit
        and set value = _unit <- value

and WarUnit(data : UnitData, aff : Affiliation, squad : Squad, leader : Option<WarUnit>) as this =
    let mutable _hp = data.BaseHp
    let mutable _atk = data.BaseAtk
    let mutable _def = data.BaseDef
    let mutable _skl = data.BaseSkl
    let mutable _agi = data.BaseAgi
    let mutable _mov = data.BaseMov
    let mutable _aff = aff
    let mutable _squad = squad
    let _cmdEnables = Array.create 4 false
    let mutable _leader =
        match leader with
        | None -> this
        | Some(u) -> u
    let mutable _surface =
        match leader with
        | None -> 
            if aff = Affiliation.Friend then
                 new LayerSurface([| data.BaseChip; medalSurfaces.[0] |] ) :> Surface
            else
                 new LayerSurface([| data.BaseChip; medalSurfaces.[4] |] ) :> Surface
        | Some(u) -> data.BaseChip

    member this.Surface
        with get () = _surface
        and set value = _surface <- value

    member this.Data
        with get () = data

    member this.Hp
        with get () = _hp
        and set value =
            _hp <- value
            if _hp < 0 then _hp <- 0
            if _hp > this.Data.BaseHp then _hp <- this.Data.BaseHp

    member this.Atk
        with get () = _atk
        and set value = _atk <- value

    member this.Def
        with get () = _def
        and set value = _def <- value

    member this.Skl
        with get () = _skl
        and set value = _skl <- value

    member this.Agi
        with get () = _agi
        and set value = _agi <- value

    member this.Mov
        with get () = _mov
        and set value = _mov <- value

    member this.IsLive
        with get () = _hp > 0

    member this.IsActive
        with get () = _cmdEnables |> Array.exists (fun b -> b)
        and set value =
            Array.fill _cmdEnables 0 _cmdEnables.Length value
            if System.String.IsNullOrWhiteSpace(this.Data.SpecialAction.ActKind) then
                _cmdEnables.[3] <- false

    member this.CommandEnables
        with get () = _cmdEnables

    member this.Affiliation
        with get () = _aff

    member this.Squad
        with get () = _squad
        and set value = _squad <- value

    member this.Leader
        with get () = _leader
        and set value = _leader <- value

    member this.IsLeader
        with get () = _leader = this

    member this.UpdateStatus (war : War) =
        let mp = war.Map |> Map.findKey (fun p (c : Chip) -> c.Unit |> Option.exists (fun u -> u = this))
        let lp = war.Map |> Map.findKey (fun p (c : Chip) -> c.Unit |> Option.exists (fun u -> u = _leader))
        let (landform : Landform) = war.Landforms |> Map.find (war.Map.[mp].Kind, data.BaseKind)
        _atk <- data.BaseAtk + landform.AtkRev
        _def <- data.BaseDef + landform.DefRev
        _skl <- data.BaseSkl + landform.SklRev
        _agi <- data.BaseAgi + landform.AgiRev
        let length = distance mp lp
        if length <> 0 && length <= _leader.Data.RevWidth then
            _atk <- _atk + _leader.Data.RevAtk
            _def <- _def + _leader.Data.RevDef
            _skl <- _skl + _leader.Data.RevSkl
            _agi <- _agi + _leader.Data.RevAgi
        ()

    override this.ToString() =
        "{" + data.BaseName + "," + data.BaseKind + "," + _hp.ToString() + "," + _atk.ToString() + "," + _def.ToString() + "," + _skl.ToString() + "," + _agi.ToString() + "," + _mov.ToString() + "}"

and War =
    {
        Map : Map<ChipPoint, Chip>;
        MapSize : ChipSize;
        /// ChipKind, UnitKind -> Landform
        Landforms : Map<string * string, Landform>;
        Time : float;
    }
