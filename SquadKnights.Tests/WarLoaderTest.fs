namespace SquadKnights.Tests

open System.IO
open SquadKnights.Entity
open NUnit.Framework
open FsUnit

[<TestFixture>] 
type ``loadWar in WarLoader`` ()=
    do System.Environment.CurrentDirectory <- Path.Combine("..", "..", "fixture")
    let war = WarLoader.loadWar 0
    let genStatus v =
        {
            Hp = v + 0; Atk = v + 1; Def = v + 2;
            Skl = v + 3; Agi = v + 4; Mov = v + 5;
        }

    [<Test>]
    member test.``read map0_tile1/2 and mapchip`` () =
        war.Map.Count |> should equal 6
        war.Map.[{ X = 0<chip>; Y = 0<chip> }].TileKind |> should equal "道"
        war.Map.[{ X = 1<chip>; Y = 0<chip> }].TileKind |> should equal "木"
        war.Map.[{ X = 0<chip>; Y = 1<chip> }].TileKind |> should equal "草原"
        war.Map.[{ X = 1<chip>; Y = 1<chip> }].TileKind |> should equal "道"
        war.Map.[{ X = 0<chip>; Y = 2<chip> }].TileKind |> should equal "草原"
        war.Map.[{ X = 1<chip>; Y = 2<chip> }].TileKind |> should equal "草原"

    [<Test>]
    member test.``read unit and map0_unit`` () =
        war.Units.Count |> should equal 2
        let assertUnitData u v name aff =
            let expStatus = genStatus v
            let expAction = 
                {
                    ActKind = "剣"; ActTarget = "Enemy";
                    ActPower = v + 6; ActRange = v + 7; ActWidth = v + 8;
                }
            let expSpecialAction = 
                {
                    ActKind = "回復"; ActTarget = "Friend";
                    ActPower = v + 9; ActRange = v + 10; ActWidth = v + 11;
                }
            let expSquadRev = genStatus (v + 13)
            u.Status |> should equal expStatus
            u.Data.BaseStatus |> should equal expStatus
            u.Data.BaseKind |> should equal name
            u.Data.BaseName |> should equal (u.Data.BaseKind + "1")
            u.Affiliation |> should equal aff
            u.Data.DefaultAction |> should equal expAction
            u.Data.SpecialAction |> should equal expSpecialAction
            u.Data.SquadWidth |> should equal (v + 12)
            u.Data.SquadRev |> should equal expSquadRev
        let u1 = war.Units.[{ X = 1<chip>; Y = 1<chip> }]
        ignore <| assertUnitData u1 1 "歩兵" Affiliation.Friend
        let u2 = war.Units.[{ X = 1<chip>; Y = 2<chip> }]
        ignore <| assertUnitData u2 21 "騎兵" Affiliation.Enemy

    [<Test>]
    member test.``read landimpact`` () =
        let u1 = war.Units.[{ X = 1<chip>; Y = 1<chip> }]
        let u2 = war.Units.[{ X = 1<chip>; Y = 2<chip> }]
        let kinds = ["草原"; "道"; "木"]
        let grw u =
            kinds
            |> List.map (fun kind -> war.LandImpacts.[kind, u.Data.BaseKind])
        let [g1; r1; w1] = grw u1
        let [g2; r2; w2] = grw u2
        g1 |> should equal { LandRev = (genStatus 1); MovCost = 1.1 }
        g2 |> should equal { LandRev = (genStatus 7); MovCost = 2.1 }
        r1 |> should equal { LandRev = (genStatus 13); MovCost = 3.1 }
        r2 |> should equal { LandRev = (genStatus 19); MovCost = 4.1 }
        w1 |> should equal { LandRev = (genStatus 25); MovCost = 5.1 }
        w2 |> should equal { LandRev = (genStatus 31); MovCost = 6.1 }
