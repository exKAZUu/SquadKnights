module Command

// namespace
open System
open System.Collections.Generic
open Paraiba.Collections
open Paraiba.Drawing.Surfaces
open Paraiba.Geometry
open Sobakuri.Entity

// module
open Utility
open Global
open Script
open ImageManager
open FormManager
open BattleManager
open WarUtil

let CalcMovableArea (war : War) p (chip : Chip) (doer : WarUnit) =
    let exists = new HashSet<MovablePoint>(
        {
            new IEqualityComparer<MovablePoint> with
                member this.Equals(l, r) = l.ChipPoint = r.ChipPoint
                member this.GetHashCode(o) = o.ChipPoint.GetHashCode()
        })
    let que = new PriorityQueue<MovablePoint>(
        {
            new IComparer<MovablePoint> with
                member this.Compare(l, r) = r.Remains.CompareTo l.Remains
        })
    que.Enqueue (new MovablePoint(p, None, float doer.Mov))
    let rec calcExists () = 
        if que.Count > 0 then
            let from = que.Dequeue()
            if exists.Add from then
                NextPoints from.ChipPoint
                |> Seq.choose (fun p -> war.Map |> Map.tryFind p |> Option.map (fun c -> p, c))
                |> Seq.filter (fun (p, chip) ->
                    chip.Unit
                    |> (not << Option.exists (fun u -> u.Affiliation <> doer.Affiliation)))
                |> Seq.iter (fun (p, chip) ->
                    let landform = war.Landforms.[chip.Kind, doer.Data.BaseKind]
                    let remains = from.Remains - landform.MovCost
                    if remains >= 0.0 then
                        que.Enqueue(new MovablePoint(p, Some(from), remains)))
            calcExists ()
    calcExists ()
    let area = exists |> Seq.map (fun mp -> mp.ChipPoint) |> Set.ofSeq
    let validArea = area |> Set.filter (fun p -> war.Map.[p].Unit.IsNone)
    let boot p =
        let newChip = war.Map.[p]
        let oldEnables = Array.copy doer.CommandEnables
        chip.Unit <- None
        newChip.Unit <- Some(doer)
        if doer.IsLeader then
            LeaderPoint.Set p
        if doer.Affiliation = Affiliation.Enemy && newChip.Kind = "建物" then
            newChip.Kind <- "廃墟"
            newChip.Surface <- new LayerSurface([|mapchipSurfaces.[0]; mapchipSurfaces.[100]|])
            let count =
                war.Map
                |> Map.toSeq
                |> Seq.filter (fun (p, c) -> c.Kind = "廃墟")
                |> Seq.length
            if count >= 3 then
                GameOver ()
        doer.CommandEnables.[0] <- false
        fun () ->
            chip.Unit <- Some(doer)
            newChip.Unit <- None
            Array.blit oldEnables 0 doer.CommandEnables 0 oldEnables.Length
    { Area = area; ValidArea = validArea; GetTargetArea = Set.singleton; Boot = boot }

let ReadyAttack war p (doer : WarUnit) act =
    let area =
        ScopeUtil.normal war act.ActRange p
        |> Set.ofSeq
    let isTarget (u : WarUnit) =
        (act.ActTarget = "Enemy" && u.Affiliation <> doer.Affiliation) ||
        (act.ActTarget = "Friend" && u.Affiliation = doer.Affiliation)
    let getTargetArea =
        ScopeUtil.normal war (act.ActWidth - 1)
    let validArea =
        area
        |> Set.filter (fun p ->
            getTargetArea p
            |> Seq.exists (fun p ->
                war.Map.[p].Unit
                |> Option.exists isTarget))
    let boot p =
        getTargetArea p
        |> Seq.iter (fun p ->
            let c = war.Map.[p]
            c.Unit
            |> Option.bind (fun u -> if isTarget u then Some(u) else None)
            |> Option.iter (fun u ->
                let old = u.Hp
                let oldEnables = Array.copy doer.CommandEnables
                let prob = calcProb.Invoke(doer, u, act)
                if prob >= random.Next(100) then
                    let effect = calcEffect.Invoke(doer, u, act)
                    if act.ActTarget = "Enemy" then
                        damage u c war effect
                    else
                        heal u c war effect
                doer.IsActive <- false
//                fun () ->
//                    u.Hp <- old
//                    Array.blit oldEnables 0 doer.CommandEnables 0 oldEnables.Length
            )
        )
        fun () -> ()
    { Area = area; ValidArea = validArea; GetTargetArea = getTargetArea; Boot = boot }

let TryAICommand (war : War) =
    let friends =
        getPointsAndUnits war
        |> Seq.filter (fun (p, u) -> u.Affiliation = Affiliation.Friend)
        |> Seq.toList
        |> List.sortBy (fun (p, u) -> if u.IsLeader then 0 else 1)

    let tryAttack p u =
        let scope = ReadyAttack war p u u.Data.DefaultAction
        let tp =
            scope.ValidArea
            |> Seq.filter (fun p ->
                war.Map.[p].Unit
                |> Option.exists (fun u -> u.Affiliation = Affiliation.Friend))
            // リーダー狙い
            |> Seq.sortBy (fun p -> if war.Map.[p].Unit.Value.IsLeader then 0 else 1)
            |> Seq.tryFind (fun _ -> true)
        tp
        |> Option.iter (fun p -> ignore <| scope.Boot p)
        tp.IsSome

    while (fst (getNextLeaderAndSquad war)).Affiliation = Affiliation.Enemy && GameContinued do
        let unit, squad = getNextLeaderAndSquad war
        let pointAndUnits = getSquadPointsAndUnits squad war |> Seq.toList
        if unit.Leader.Data.BaseName = "ヴォルフ" then
            pointAndUnits
            |> List.iter (fun (p, u) ->
                if not <| tryAttack p u && GameContinued then
                    // 移動
                    let fp, fu =
                        friends
                        |> List.minBy (fun (fp, fu) -> distance p fp)
                    let count =
                        getLeadersAndSquads war
                        |> Seq.filter (fun (l, s) ->
                            l.Affiliation = Affiliation.Enemy && (getSquadUnits s war |> Seq.length) > 1)
                        |> Seq.length
                    if (distance p fp) = 1 || count = 1 then
                        let scope = CalcMovableArea war p war.Map.[p] u
                        let housePoint = 
                            scope.ValidArea
                            |> Seq.tryFind (fun p -> war.Map.[p].Kind = "建物")
                        let tp =
                            match housePoint with
                            | None ->
                                scope.ValidArea
                                |> Seq.minBy (fun sp -> distance fp sp)
                            | Some(p) -> p
                        ignore <| scope.Boot tp
                        if GameContinued then
                            ignore <| tryAttack tp u
                u.IsActive <- false
            )

        else
            pointAndUnits
            |> List.iter (fun (p, u) ->
                if not <| tryAttack p u && GameContinued then
                    // 移動
                    let fp, fu =
                        friends
                        |> List.minBy (fun (fp, fu) -> distance p fp)
                    let scope = CalcMovableArea war p war.Map.[p] u
                    let housePoint = 
                        scope.ValidArea
                        |> Seq.tryFind (fun p -> war.Map.[p].Kind = "建物")
                    let tp =
                        match housePoint with
                        | None ->
                            scope.ValidArea
                            |> Seq.minBy (fun sp -> distance fp sp)
                        | Some(p) -> p
                    ignore <| scope.Boot tp
                    if GameContinued then
                        ignore <| tryAttack tp u
                u.IsActive <- false
                )
        WarUtil.resetWt squad war
        WarUtil.advanceTurn war
    form.Invalidate(true)
