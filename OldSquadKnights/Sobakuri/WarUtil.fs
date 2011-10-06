module WarUtil

// namespace
open Paraiba.Geometry
open Paraiba.Utility
open Sobakuri.Entity

// module
open Settings
open Global
open Utility

/// get all units
let getUnits war =
    war.Map
    |> Map.toSeq
    |> Seq.map snd
    |> Seq.choose (fun chip -> chip.Unit)

/// get all units and points
let getPointsAndUnits war =
    war.Map
    |> Map.toSeq
    |> Seq.choose (fun (p, c) ->
        c.Unit |> Option.map (fun u -> p, u))

/// get all leaders and squads
let getLeadersAndSquads =
    getUnits
    >> Seq.filter (fun u -> u.IsLeader)
    >> Seq.map (fun u -> u, u.Squad)

/// get all sorted leaders and squads
let getSortedLeadersAndSquads =
    getLeadersAndSquads
    >> Seq.sortBy (fun (u, t) -> t.Wt)

/// get next leader and squad
let getNextLeaderAndSquad =
    getLeadersAndSquads
    >> Seq.minBy (fun (u, t) -> t.Wt)

/// update all units
let updateUnits war =
    getUnits war
    |> Seq.iter (fun u -> u.UpdateStatus war)


/// Wait turn
let getSquadUnits squad war =
    getUnits war
    |> Seq.filter (fun u -> refeql u.Squad squad)

let getSquadPointsAndUnits squad war =
    getPointsAndUnits war
    |> Seq.filter (fun (_, u) -> refeql u.Squad squad)

let getSquadAgi squad war =
    let average =
        getSquadUnits squad war
        |> Seq.averageBy (fun u -> float u.Agi)
    average + 20.0

let updateWt time war =
    let newSquads =
        getLeadersAndSquads war
        |> Seq.map (fun (l, squad) ->
            squad, { squad with Wt = squad.Wt - time }
            )
        |> Map.ofSeq
    getUnits war
    |> Seq.iter (fun u ->
        u.Squad <- newSquads |> Map.find u.Squad
        )

let resetWt (squad : Squad) war =
    squad.Wt <- MaxActionValue / (getSquadAgi squad war)

let advanceTurn (war : War) =
    let leader, squad = getNextLeaderAndSquad war
    LeaderPoint.Set (
        war.Map
        |> Map.pick (fun p c ->
            c.Unit
            |> Option.bind (fun u -> if u = leader then Some(p) else None)
            ))
    let advTime = squad.Wt
    getLeadersAndSquads war
    |> Seq.iter (fun (_, t) -> t.Wt <- t.Wt - advTime)
    getSquadUnits squad war
    |> Seq.iter (fun u -> u.IsActive <- true)
    updateUnits war

let startTurn (war : War) =
    // initialize all squad's wts
    getLeadersAndSquads war |> Seq.iter (fun (_, s) -> resetWt s war)
    // start first turn
    advanceTurn war
