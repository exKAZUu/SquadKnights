module WarLoader

open System.IO
open System.Drawing
open Paraiba.Drawing
open Paraiba.Drawing.Surfaces
open Paraiba.Geometry
open Paraiba.IO

// namespaces
open Sobakuri.Entity
// modules
open Utility
open ImageManager

let loadUnitData (line : string[]) =
    let index = ref -1
    let next () = index := !index + 1; line.[!index]
    let name = next()
    let kind = next()
    let mov = toInt(next())
    let hp = toInt(next())
    let atk = toInt(next())
    let def = toInt(next())
    let skl = toInt(next())
    let agi = toInt(next())
    let chip =
        let path = Path.Combine("image", "unit", next())
        let surfaces =
            BitmapUtil.Load(path, transparentColor)
                .SplitToBitmaps(24, 24)
            |> Array.map (fun b -> b.ToSurface())
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
    let revWidth = toInt(next())
    let revMov = toInt(next())
    let revAtk = toInt(next())
    let revDef = toInt(next())
    let revSkl = toInt(next())
    let revAgi = toInt(next())
    let revWt = toInt(next())
    {
        BaseName = name;
        BaseKind = kind;
        BaseMov = mov;
        BaseHp = hp;
        BaseAtk = atk;
        BaseDef = def;
        BaseSkl = skl;
        BaseAgi = agi;
        BaseChip = chip;
        BaseFace = face;
        DefaultAction = defaultAction;
        SpecialAction = specialAction;
        RevWidth = revWidth;
        RevMov = revMov;
        RevAtk = revAtk;
        RevDef = revDef;
        RevSkl = revSkl;
        RevAgi = revAgi;
        RevWt = revWt;
    }

let readLines (path : string) =
    seq {
        use fr = new StreamReader(path, Paraiba.Text.XEncoding.SJIS)
        while not fr.EndOfStream do
            yield fr.ReadLine()
    }

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

let readLandform =
    readCsv
    >> Seq.map (fun l -> ((l.[0], l.[1]), new Landform(l)))

let readUnit =
    readCsv
    >> Array.mapi (fun i line -> loadUnitData(line))

let (|FilterEmpty|) =
    Seq.filter (fun (p, s) -> not <| System.String.IsNullOrWhiteSpace(s))

let loadWar iStage =
    let mapStr = "map" + iStage.ToString()
    let (MapOfSeq mapchips) = readMapchip (Path.Combine("data", "mapchip.csv"))
    let (MapOfSeq landforms) = readLandform (Path.Combine("data", "landform.csv"))
    let units = readUnit (Path.Combine("data", "unit.csv"))

    let mapRowTable1, mapSize = readMap (Path.Combine("data", mapStr + "_chip1.csv"))
    let (FilterEmpty (MapOfSeq mapTable2)), _ = readMap (Path.Combine("data", mapStr + "_chip2.csv"))
    let (FilterEmpty (ListOfSeq unitRowData)), _ = readMap (Path.Combine("data", mapStr + "_unit1.csv"))

    let map = 
        let tableToIndexAndSurfaces =
            Seq.map (fun (p, (Int i)) -> p, (i, [i]))
            >> Map.ofSeq
        let mergeMap mapTable =
            Map.map (fun p (i, is) ->
                match Map.tryFind p mapTable with
                | None -> i, is
                | Some(Int i) -> i, i::is)
        let parseAffiliation = function
            | 'E' -> Affiliation.Enemy
            | _ -> Affiliation.Friend
        let isLeader (str : string) = str.[1] = 'L'
        let (|ParseDeploy|)(s : string) =
            let ss = s.Split([|'-'|])
            (s.[0] |> parseAffiliation, s.[1] = 'L', ss.[1] |> toInt, ss.[2] |> toInt)
        let toSurface = function
        | [i] -> mapchipSurfaces.[i]
        | _ as l ->
            let ss = l |> List.rev |> Seq.distinct |> Seq.map (fun i -> mapchipSurfaces.[i]) |> Seq.toArray
            if ss.Length = 1 then ss.[0] else new LayerSurface(ss) :> Surface
        let mergeToChip unitData mapchips =
            let squads =
                unitData
                |> Seq.filter (fun (_, (ParseDeploy (_, isLeader, _, _))) -> isLeader)
                |> Seq.map (fun (p, (ParseDeploy (aff, isLeader, iSquads, iUnits))) ->
                    iSquads, (p, new WarUnit(units.[iUnits], aff, { Wt = 0.0; IsFirst = true }, None)))
                |> Map.ofSeq
            let (MapOfSeq unitTable) =
                unitData
                |> Seq.filter (fun (_, (ParseDeploy (_, isLeader, _, _))) -> not isLeader)
                |> Seq.map (fun (p, (ParseDeploy (aff, isLeader, iSquads, iUnits))) ->
                    let _, leader = squads |> Map.find iSquads
                    p, new WarUnit(units.[iUnits], aff, leader.Squad, Some(leader)))
                |> Seq.append (squads |> Map.toSeq |> Seq.map (fun (iSquads, (p, u)) -> p, u))
            Map.map <| fun p (i, is) ->
                new Chip(toSurface is, Map.find i mapchips, Map.tryFind p unitTable)

        tableToIndexAndSurfaces mapRowTable1
        |> mergeMap mapTable2
        |> mergeToChip unitRowData mapchips

    { Map = map; MapSize = mapSize; Landforms = landforms; Time = 0.0 }