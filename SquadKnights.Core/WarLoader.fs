module WarLoader

open System.IO
open System.Drawing
open Paraiba.Drawing
open Paraiba.Drawing.Surfaces
open Paraiba.Geometry
open Paraiba.IO

// namespaces
open SquadKnights.Entity
// modules
open Utility
open ImageManager

let parseStatus next =
    {
        Hp = toInt(next())
        Atk = toInt(next()) 
        Def = toInt(next())
        Skl = toInt(next())
        Agi = toInt(next())
        Mov = toInt(next())
    }

let parseLandImpact (line : string[]) =
    let next = iterator 1 line
    {
        LandRev = parseStatus next
        MovCost = toFloat(next())
    }

let parseUnitData (line : string[]) =
    let next = iterator -1 line
    let name = next()
    let kind = next()
    let status = parseStatus next
    let chip =
        let path = Path.Combine("image", "unit", next())
        let surfaces =
            BitmapUtil.Load(path, transparentColor)
                .SplitToSurfaces(ChipPixelSize)
        surfaces.[0]
    let face =
        let fname = next()
        let image = 
            if System.String.IsNullOrWhiteSpace(fname) then
                new Bitmap(chip.GetImage(), 44, 44)
            else
                BitmapUtil.Load(Path.Combine("image", "minifaces", fname), transparentColor)
        image.ToSurface()
    let defaultAction = {
            ActKind = next();
            ActTarget = next();
            ActPower = toInt(next());
            ActRange = toInt(next());
            ActWidth = toInt(next())
        }
    let specialAction = {
            ActKind = next();
            ActTarget = next();
            ActPower = toInt(next());
            ActRange = toInt(next());
            ActWidth = toInt(next())
        }
    let squadWidth = toInt(next())
    let SquadRev = parseStatus next
    {
        BaseName = name
        BaseKind = kind
        BaseStatus = status
        BaseChip = chip
        BaseFace = face
        DefaultAction = defaultAction
        SpecialAction = specialAction
        SquadWidth = squadWidth
        SquadRev = SquadRev
    }

let readLines (path : string) =
    File.ReadLines(path, Paraiba.Text.XEncoding.SJIS)

let readCsv path =
    path
    |> readLines
    |> Seq.skip 1
    |> Seq.map (fun line ->
        line.Split <| [|','|])
    |> Seq.toArray   // actually read csv file

let readMap path =
    let csv = readCsv path
    (csv
    |> Seq.mapi (fun y ->
        Seq.mapi <| fun x v -> ({ X = 1<chip> * x; Y = 1<chip> * y }, v))
    |> Seq.collect (fun l -> l),
        { W = 1<chip> * csv.[0].Length; H = 1<chip> * csv.Length })

let readMapchip =
    readCsv
    >> Seq.mapi (fun y ->
        Seq.mapi(fun x s -> (y * 10 + x, s)))
    >> Seq.collect (fun l -> l)

let readLandImpacts =
    readCsv
    >> Seq.map (fun l -> ((l.[0], l.[1]), parseLandImpact l))

let readUnit =
    readCsv
    >> Array.mapi (fun i line -> parseUnitData line)

let (|FilterEmpty|) =
    Seq.filter (fun (p, s) -> not <| System.String.IsNullOrWhiteSpace(s))

let loadWar iStage =
    let getDataPath fileName = Path.Combine("data", fileName)
    let mapStr = "map" + iStage.ToString()
    let (MapOfSeq mapchips) = readMapchip (getDataPath "mapchip.csv")
    let (MapOfSeq landImpacts) = readLandImpacts (getDataPath "landimpact.csv")
    let units = readUnit (getDataPath "unit.csv")

    let (MapOfSeq tiles), mapSize = readMap (getDataPath (mapStr + "_tile1.csv"))
    let (FilterEmpty (MapOfSeq additionalTiles)), _ = readMap (getDataPath (mapStr + "_tile2.csv"))
    let (FilterEmpty (ListOfSeq unitData)), _ = readMap (getDataPath (mapStr + "_unit.csv"))

    let map =
        let mergeMap tiles =
            Map.map (fun p chipIds ->
                match Map.tryFind p tiles with
                | None -> chipIds
                | Some(Int id) ->
                    match chipIds with
                    | head :: tail when head = id -> chipIds
                    | _ -> id::chipIds)

        tiles
        |> Map.map (fun _ (Int chipId) -> [chipId])
        |> mergeMap additionalTiles
        |> Map.map (fun _ chipIds ->
            let toSurface = function
                | [chipId] -> mapchipSurfaces.[chipId]
                | _ as chipIds ->
                    chipIds
                    |> List.rev
                    |> Seq.distinct
                    |> Seq.map (fun id -> mapchipSurfaces.[id])
                    |> Seq.toArray
                    |> (fun ss -> if ss.Length = 1 then ss.[0] else new LayerSurface(ss) :> Surface)
            {
                TileImage = toSurface chipIds
                TileKind = Map.find (List.head chipIds) mapchips
            })

    let createUnit data aff squadId leader =
        let surface =
            match leader with
            | None -> 
                if aff = Affiliation.Friend then
                        new LayerSurface([| data.BaseChip; medalSurfaces.[0] |] ) :> Surface
                else
                        new LayerSurface([| data.BaseChip; medalSurfaces.[4] |] ) :> Surface
            | Some(_) -> data.BaseChip
        let commands = Seq.init 4 (fun i -> i, true) |> Map.ofSeq
        {
            Status = data.BaseStatus
            Affiliation = aff
            SquadId = squadId
            IsLeader = leader.IsNone
            Data = data
            CommandAvailabilities = commands
            UnitImage = surface
        }

    let parseAffiliation = function
        | 'E' -> Affiliation.Enemy
        | _ -> Affiliation.Friend

    let isLeader (str : string) = str.[1] = 'L'

    let (|ParseDeploy|)(s : string) =
        s.Split([|'-'|])
        |> (fun ss -> s.[0] |> parseAffiliation, s |> isLeader,
                      ss.[1] |> toInt |> toSquadId, ss.[2] |> toInt)

    let leaderData, otherData =
        unitData
        |> List.partition (fun (_, (ParseDeploy (_, isLeader, _, _))) -> isLeader)

    let (MapOfSeq squads) =
        leaderData
        |> Seq.map (fun (p, (ParseDeploy (aff, isLeader, iSquads, iUnits))) ->
            iSquads,
            (p, { Wt = 0.0<day>; IsLeaderLive = false },
                createUnit units.[iUnits] aff iSquads None))

    let (MapOfSeq units) =
        otherData
        |> Seq.map (fun (p, (ParseDeploy (aff, isLeader, iSquads, iUnits))) ->
            let _, _, leader = squads |> Map.find iSquads
            p, createUnit units.[iUnits] aff iSquads (Some leader))
        |> Seq.append (squads |> Map.toSeq |> Seq.map (fun (_, (p, _, u)) -> p, u))

    {
        Map = map
        MapSize = mapSize
        LandImpacts = landImpacts
        Units = units
        Squads = squads |> Map.map (fun _ (_, s, _) -> s)
        Time = 0.0<day>
    }