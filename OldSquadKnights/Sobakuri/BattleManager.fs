module BattleManager

open Paraiba.Drawing.Surfaces
open Sobakuri.Entity

open Utility
open FormManager
open ImageManager
open WarUtil

let damage (unit : WarUnit) (chip : Chip) (war : War) value =
    unit.Hp <- unit.Hp - value
    if not unit.IsLive then
        chip.Unit <- None
        if unit.Leader = unit then
            getSquadPointsAndUnits unit.Squad war
            |> Seq.iter (fun (p, u) ->
                u.Leader <- u
                u.Squad <- { Wt = 0.0; IsFirst = false }
                if u.Affiliation = Affiliation.Friend then
                    u.Surface <- new LayerSurface([| u.Data.BaseChip; medalSurfaces.[1] |]) :> Surface
                else
                    u.Surface <- new LayerSurface([| u.Data.BaseChip; medalSurfaces.[5] |]) :> Surface
                WarUtil.resetWt u.Squad war
                FormManager.mapPanel.Invalidate(cp2rect p)
            )
        let isLive =
            getLeadersAndSquads war
            |> Seq.filter (fun (u, s) -> u.Affiliation = unit.Affiliation)
            |> Seq.exists (fun (u, s) -> s.IsFirst)
        if not isLive then
            if unit.Affiliation = Affiliation.Friend then
                GameOver ()
            else
                Clear ()

let heal (unit : WarUnit) (chip : Chip) (war : War) value =
    unit.Hp <- unit.Hp + value