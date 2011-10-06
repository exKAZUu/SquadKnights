module Program

open System.IO
open System.Windows.Forms
open Paraiba.Windows.Forms
open System.Collections.Generic
open System.Drawing
open Paraiba.Drawing
open Paraiba.Drawing.Surfaces
open Paraiba.Geometry
open Paraiba.IO
open Paraiba.Utility

// namespaces
open Sobakuri.Entity
// modules
open Utility
open Global
open WarLoader
open Command
open ImageManager
open FormManager
open WarUtil

//let mainLoop = new MainLoop(15)

let endTurn squad =
    WarUtil.resetWt squad war
    WarUtil.advanceTurn war
    getSquadPointsAndUnits (snd (getNextLeaderAndSquad war)) war
    |> Seq.iter (fun (p, _) -> mapPanel.Invalidate(cp2rect p))
    TryAICommand war
    // WTリスト更新の反映
    sidePanel.Invalidate(false)

/// 行動終了処理
let booted () =
    if CurretActive.IsSome then
        let p, _, doer = CurretActive.Value
        updateUnits war
        // 行動完了の反映
        mapPanel.Invalidate(cp2rect p)
        Scope.Set None
        if not doer.IsActive then
            let _, squad = getNextLeaderAndSquad war
            let isDone =
                getSquadUnits squad war
                |> Seq.forall (fun u -> not u.IsActive)
            if isDone then
                endTurn squad
        // WTリスト更新の反映
        sidePanel.Invalidate(false)

cmdPanel.Paint
|> Observable.add(fun (PaintEvent2Graphics g) -> 
    let cmdImages =
        BitmapUtil.Load(Path.Combine("image", "window", "command.png"), transparentColor)
            .SplitToBitmaps(112, 32)
        |> Array.map (fun b -> b.ToSurface())
    let draw i =
        let _, _, unit = CurretActive.Value
        let iCmdImages =
            if not unit.CommandEnables.[i] then i * 4 + 3
            elif i <> CurrentCommandIndex.Value then i * 4
            else i * 4 + 1
        g.DrawSurface(cmdImages.[iCmdImages], 0, i * 32)
    draw 0
    draw 1
    draw 2
    draw 3
    )
cmdPanel.MouseClick
|> Observable.add (fun e ->
    let p, chip, doer = CurretActive.Value
    let i = CurrentCommandIndex.Value
    match i with
    | 0 when doer.CommandEnables.[i] ->
        Scope.Set (Some(CalcMovableArea war p chip doer))
        cmdForm.Visible <- false
    | 1 when doer.CommandEnables.[i] ->
        Scope.Set (Some(ReadyAttack war p doer doer.Data.DefaultAction))
        cmdForm.Visible <- false
    | 2 when doer.CommandEnables.[i] ->
        doer.IsActive <- false
        booted()
        cmdForm.Visible <- false
    | 3 when doer.CommandEnables.[i] ->
        Scope.Set (Some(ReadyAttack war p doer doer.Data.SpecialAction))
        cmdForm.Visible <- false
    | _ -> ()
    )
cmdPanel.MouseMove
|> Observable.add (fun e ->
    CurrentCommandIndex.Set (e.Y / 32)
    )
cmdPanel.MouseLeave
|> Observable.add (fun e ->
    CurrentCommandIndex.Set -1
    )
        
let activeBrush = new SolidBrush(Color.FromArgb(64, 0, 0, 255))
let nonActiveBrush = new SolidBrush(Color.FromArgb(128, 127, 127, 127))
let leaderBrush = new SolidBrush(Color.FromArgb(64, 255, 255, 0))
let areaBrush = new SolidBrush(Color.FromArgb(0, 0, 0, 255))
let validAreaBrush = new SolidBrush(Color.FromArgb(64, 0, 0, 255))
let targetAreaBrush = new SolidBrush(Color.FromArgb(64, 255, 0, 0))
mapPanel.Paint
|> Observable.add(fun e ->
    let g = e.Graphics
    let rect = e.ClipRectangle
    let sy = rect.Top / ChipHeight
    let ey = (rect.Bottom - 1) / ChipHeight
    let sx = rect.Left / ChipWidth
    let ex = (rect.Right - 1) / ChipHeight
    let area, validArea =
        match Scope.Value with
        | None -> Set.empty, Set.empty
        | Some(s) -> s.Area, s.ValidArea
    let leader, squad = getNextLeaderAndSquad war
    for y = sy to ey do
        for x = sx to ex do
            let cp = { X = x * 1<chip>; Y = y * 1<chip> }
            let c = war.Map.[cp]
            let p = cp2pt cp
            g.DrawSurface(c.Surface, p)
            c.Unit
            |> Option.iter (fun u ->
                g.DrawSurface(u.Surface, p)
                if u.Squad = squad then
                    if u.IsActive then
                        g.FillRectangle(activeBrush, p.X, p.Y, ChipWidth, ChipHeight)
                    else
                        g.FillRectangle(nonActiveBrush, p.X, p.Y, ChipWidth, ChipHeight)
                )
            if Set.contains cp LeaderArea.Value then
                g.FillRectangle(leaderBrush, p.X, p.Y, ChipWidth, ChipHeight)
            if Set.contains cp area then
                g.FillRectangle(areaBrush, p.X, p.Y, ChipWidth, ChipHeight)
            if Set.contains cp validArea then
                g.FillRectangle(validAreaBrush, p.X, p.Y, ChipWidth, ChipHeight)
            if Set.contains cp TargetArea.Value then
                g.FillRectangle(targetAreaBrush, p.X, p.Y, ChipWidth, ChipHeight)
    )
mapPanel.MouseMove
|> Observable.add (fun (MouseEvent2Cp p) ->
    CurrentMapPoint.Set p
    )

let mapPanelLeftClick, mapPanelRightClick =
    mapPanel.MouseClick
    |> Observable.partition (fun e -> e.Button <> MouseButtons.Right)

mapPanelLeftClick
|> Observable.add (fun e ->
    let p = pt2cp e.Location
    let chip = Map.find p war.Map
    if Scope.Value.IsNone then
        // コマンドウィンドウの表示
        match chip.Unit with
        | None ->
            cmdForm.Visible <- false
        | Some(u) -> 
            if u.IsActive then
                CurretActive <- Some(p, chip, u)
                cmdForm.Location <- Cursor.Position
                cmdForm.Visible <- true
    else
        // 行動の発動
        let s = Scope.Value.Value
        if s.ValidArea |> Seq.exists (fun p -> p = CurrentMapPoint.Value) then
            let cancel = s.Boot CurrentMapPoint.Value
            booted()
    )
mapPanelRightClick
|> Observable.add (fun e ->
    cmdForm.Visible <- false
    Scope.Set None
    )

let revAtkPos = [|new Point2(17, 31); new Point2(21, 31); new Point2(26, 31)|]
let revDefPos = [|new Point2(17, 40); new Point2(21, 40); new Point2(26, 40)|]
let revSklPos = [|new Point2(45, 31); new Point2(49, 31); new Point2(54, 31)|]
let revLucPos = [|new Point2(45, 40); new Point2(49, 40); new Point2(54, 40)|]
let revAgiPos = [|new Point2(73, 31); new Point2(77, 31); new Point2(82, 31)|]
let revMovPos = [|new Point2(73, 40); new Point2(77, 40); new Point2(82, 40)|]
let revWtPos = [|new Point2(97, 31); new Point2(101, 31); new Point2(106, 31)|]
let wtPos = [|new Point2(96, 40); new Point2(101, 40); new Point2(106, 40)|]
let hpPos = [|new Point2(77, 17); new Point2(82, 17); new Point2(87, 17)|]
let maxHpPos = [|new Point2(96, 17); new Point2(101, 17); new Point2(106, 17)|]

let miniHpPos = [|new Point2(43, 17); new Point2(48, 17); new Point2(53, 17)|]
let miniMaxHpPos = [|new Point2(62, 17); new Point2(67, 17); new Point2(72, 17)|]
let miniWtPos = [|new Point2(60, 6); new Point2(65, 6); new Point2(70, 6)|]

let detailHpPos = [|new Point2(121,44); new Point2(126,44); new Point2(131,44)|]
let detailMaxHpPos = [|new Point2(140,44); new Point2(145,44); new Point2(150,44)|]
let detailAtkPos = [|new Point2(28,60); new Point2(33,60); new Point2(38,60)|]
let detailDefPos = [|new Point2(28,77); new Point2(33,77); new Point2(38,77)|]
let detailSklPos = [|new Point2(28,94); new Point2(33,94); new Point2(38,94)|]
let detailAgiPos = [|new Point2(28,111); new Point2(33,111); new Point2(38,111)|]
let detailRevAtkPos = [|new Point2(44,60); new Point2(48,60); new Point2(53,60)|]
let detailRevDefPos = [|new Point2 (44,77); new Point2 (48,77); new Point2(53,77)|]
let detailRevSklPos = [|new Point2 (44,94); new Point2 (48,94); new Point2(53,94)|]
let detailRevAgiPos = [|new Point2 (44,111); new Point2 (48,111); new Point2(53,111)|]

let buttonRect = new Rectangle(0, 352, 160, 32)

sidePanel.MouseMove
|> Observable.add (fun e ->
    let index = if buttonRect.Contains(e.Location) then 0 else -1
    CurrentButtonIndex.Set index
    )

sidePanel.MouseLeave
|> Observable.add (fun e -> CurrentButtonIndex.Set -1)

sidePanel.MouseClick
|> Observable.add (fun e ->
    let leader, squad = getNextLeaderAndSquad war
    if leader.Affiliation = Affiliation.Friend then
        match CurrentButtonIndex.Value with
        | 0 ->
            Scope.Set None
            getSquadPointsAndUnits squad war
            |> Seq.iter (fun (p, u) ->
                u.IsActive <- false
                mapPanel.Invalidate(cp2rect p))
            endTurn squad
        | _ -> ()
    )

sidePanel.Paint
|> Observable.add(fun e ->
    let g = e.Graphics
    let leader, squad = getNextLeaderAndSquad war
    let getAffIndex (u : WarUnit) = if u.Affiliation = Affiliation.Friend then 0 else 1
    let iAff = getAffIndex leader
    g.DrawSurface(sideImages.[iAff], 0, 0)
    g.DrawString(leader.Data.BaseName, font, Brushes.Black, float32 2, float32 5)
    let drawNumbers2 value (pos : Point2[]) =
        let iSign =
            if value = 0 then 2
            elif value > 0 then 0
            else 1
        let v = abs value
        g.DrawSurface(signImages.[iSign], pos.[0])
        g.DrawSurface(numberImages.[v /  10 % 10], pos.[1])
        g.DrawSurface(numberImages.[v /   1 % 10], pos.[2])
    let drawNumbers3 value (pos : Point2[]) =
        g.DrawSurface(numberImages.[value / 100 % 10], pos.[0])
        g.DrawSurface(numberImages.[value /  10 % 10], pos.[1])
        g.DrawSurface(numberImages.[value /   1 % 10], pos.[2])
//    drawNumbers3 leader.Hp hpPos
//    drawNumbers3 leader.Data.Hp maxHpPos
    drawNumbers2 leader.Data.RevAtk revAtkPos
    drawNumbers2 leader.Data.RevDef revDefPos
    drawNumbers2 leader.Data.RevSkl revSklPos
    drawNumbers2 leader.Data.RevAgi revAgiPos
    drawNumbers2 leader.Data.RevMov revMovPos
    drawNumbers2 leader.Data.RevWt revWtPos
    drawNumbers2 0 revLucPos
    g.DrawSurface(leader.Data.BaseFace, 112, 4)

    getSquadUnits squad war
    |> Seq.iteri (fun i u ->
        let img = unitMiniImages.[iAff + 2]
        let bp = new Vector2(0, 52 + img.Height * i)
        g.DrawSurface(img, bp.X, bp.Y)
        g.DrawSurface(u.Surface, bp.X + 4, bp.Y + 4)
        drawNumbers3 u.Hp (miniHpPos |> Array.map (fun p -> p + bp))
        drawNumbers3 u.Data.BaseHp (miniMaxHpPos |> Array.map (fun p -> p + bp))
        )

    getSortedLeadersAndSquads war
    |> Seq.skip 1
    |> Seq.zip (seq {0 .. 8})
    |> Seq.iter (fun (i, (u, s)) ->
        let img = unitMiniImages.[getAffIndex u]
        let bp = new Vector2(80, 52 + img.Height * i)
        g.DrawSurface(img, bp.X, bp.Y)
        g.DrawSurface(u.Surface, bp.X + 4, bp.Y + 4)
        drawNumbers3 u.Hp (miniHpPos |> Array.map (fun p -> p + bp))
        drawNumbers3 u.Data.BaseHp (miniMaxHpPos |> Array.map (fun p -> p + bp))
        drawNumbers3 (int(s.Wt * (getSquadAgi s war))) (miniWtPos |> Array.map (fun p -> p + bp))
        )

    let unit =
        war.Map |> Map.tryFind CurrentMapPoint.Value
        |> Option.bind (fun c -> c.Unit)
    match unit with
    | None ->
        let iButton =
            if leader.Affiliation <> Affiliation.Friend then 3
            elif CurrentButtonIndex.Value <> 0 then 0
            else 1
        g.DrawSurface(buttonImages.[iButton], 0, 352)
    | Some(u) ->
        let iAff = getAffIndex u
        let bp = new Vector2(0, 352)
        g.DrawSurface(unitDetailImages.[iAff], bp.X, bp.Y)
        g.DrawSurface(u.Surface, bp.X + 4, bp.Y + 4)
        g.DrawString(u.Data.BaseName, font, Brushes.Black, float32 (bp.X + 30), float32 (bp.Y + 5))
        drawNumbers3 u.Hp (detailHpPos |> Array.map (fun p -> p + bp))
        drawNumbers3 u.Data.BaseHp (detailMaxHpPos |> Array.map (fun p -> p + bp))
        drawNumbers3 u.Data.BaseAtk (detailAtkPos |> Array.map (fun p -> p + bp))
        drawNumbers2 (u.Atk - u.Data.BaseAtk) (detailRevAtkPos |> Array.map (fun p -> p + bp))
        drawNumbers3 u.Data.BaseDef (detailDefPos |> Array.map (fun p -> p + bp))
        drawNumbers2 (u.Def - u.Data.BaseDef) (detailRevDefPos |> Array.map (fun p -> p + bp))
        drawNumbers3 u.Data.BaseSkl (detailSklPos |> Array.map (fun p -> p + bp))
        drawNumbers2 (u.Skl - u.Data.BaseSkl) (detailRevSklPos |> Array.map (fun p -> p + bp))
        drawNumbers3 u.Data.BaseAgi (detailAgiPos |> Array.map (fun p -> p + bp))
        drawNumbers2 (u.Agi - u.Data.BaseAgi) (detailRevAgiPos |> Array.map (fun p -> p + bp))
    //numberImages
)


Application.Run(form)