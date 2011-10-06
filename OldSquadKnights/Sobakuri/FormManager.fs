module FormManager

open System.Drawing
open Paraiba.Drawing
open System.Windows.Forms
open Paraiba.Windows.Forms
open Paraiba.Utility
open Sobakuri.Entity

open Global
open Utility
open ImageManager
open WarLoader

/// メインフォーム
let form = new Form(Size = new Size(800, 600))
form.SuspendLayout()

/// コマンドウィンドウ
let cmdForm = new Form()
cmdForm.SuspendLayout()

/// コマンドパネル
let cmdPanel =
    let panel = new DrawableControl()
    panel.Dock <- DockStyle.Fill
    panel
/// マップパネル
let mapPanel =
    let panel = new DrawableControl()
    panel
/// 詳細パネル
let sidePanel = new DrawableControl(Dock = DockStyle.Fill)

cmdForm.Controls.Add(cmdPanel)
cmdForm.FormBorderStyle <- FormBorderStyle.FixedToolWindow
cmdForm.ControlBox <- false
cmdForm.ClientSize <- new Size(112, 32 * 4)
cmdForm.ResumeLayout(false)
cmdForm.Show(form)
cmdForm.Visible <- false


let splitContainer = new SplitContainer(Dock = DockStyle.Fill,
                                        Panel1MinSize = 0,
                                        Panel2MinSize = 0,
                                        FixedPanel = FixedPanel.Panel2,
                                        IsSplitterFixed = true,
                                        SplitterWidth = 1,
                                        SplitterDistance = 0)
splitContainer.Panel1.SuspendLayout()
splitContainer.Panel2.SuspendLayout()
splitContainer.SuspendLayout()

splitContainer.Panel2.Controls.Add sidePanel

let scrollablePanel = new ScrollablePanel()
scrollablePanel.Dock <- DockStyle.Fill
scrollablePanel.Panel <- mapPanel
splitContainer.Panel1.Controls.Add scrollablePanel

form.Controls.Add splitContainer

splitContainer.Panel1.ResumeLayout(false)
splitContainer.Panel2.ResumeLayout(false)
splitContainer.ResumeLayout(false)
form.ResumeLayout(false)

form.Visible <- true
splitContainer.SplitterDistance <- form.Width - sideWidth - 16

let mutable StageIndex =
    let args = System.Environment.GetCommandLineArgs()
    if args.Length > 1 then toInt(args.[1]) else 1

let mutable war = loadWar(StageIndex)
mapPanel.Size <- cp2sz war.MapSize

let mutable GameContinued = true

let ShowScreen (screenImage : Bitmap) =
    let startPanel = new DrawableControl()
    splitContainer.Visible <- false
    startPanel.Dock <- DockStyle.Fill
    form.Controls.Clear()
    form.Controls.Add startPanel
    startPanel.Paint
    |> Observable.add (fun e ->
        e.Graphics.Clear(Color.Black)
        let x = (startPanel.Width - screenImage.Width) / 2
        let y = (startPanel.Height - screenImage.Height) / 2
        e.Graphics.DrawImage(screenImage, x, y)
        )
    startPanel.SizeChanged
    |> Observable.add (fun e ->
        startPanel.Invalidate(false))
    startPanel.MouseClick
    |> Observable.add (fun e ->
        startPanel.Visible <- false
        form.Controls.Add splitContainer
        splitContainer.Visible <- true
        GameContinued <- true
        )
        
let setTargetArea scope cp =
    let targetArea =
        match Scope.Value with
        | None -> Set.empty
        | Some(s) -> if s.ValidArea |> Set.contains cp then s.GetTargetArea cp else Set.empty
    TargetArea.Set(targetArea)

let invalidateChipPoints cps =
    cps |> Seq.iter (fun cp -> mapPanel.Invalidate(cp2rect cp))

let invalidateChipPointSet cps ocps =
    Set.difference cps ocps |> invalidateChipPoints
    Set.difference ocps cps |> invalidateChipPoints

let Initialize () =
    CurrentMapPoint <- MonitoredWrap.Create({ X = 0<chip>; Y = 0<chip> })
    CurrentMapUnit <- MonitoredWrap.Create(Option<WarUnit>.None)
    CurrentCommandIndex <- MonitoredWrap.Create(-1)
    CurrentButtonIndex <- MonitoredWrap.Create(-1)
    CurretActive <- Option<ChipPoint * Chip * WarUnit>.None
    Scope <- MonitoredWrap.Create(Option<PrintableScope>.None)
    TargetArea <- MonitoredWrap.Create<Set<ChipPoint>>(Set.empty)

    /// マップ上のマウス位置の変化イベント
    CurrentMapPoint.Changed <- (fun cp ocp ->
        let chip = war.Map.[cp]
        CurrentMapUnit.Set(chip.Unit)
        setTargetArea Scope.Value cp
        )
    /// マップ上のマウス位置にいるユニットの変化イベント
    CurrentMapUnit.Changed <- (fun u ou ->  
        sidePanel.Invalidate(false)
        )
    /// 詳細ウィンドウにあるボタンにおけるマウス位置変化イベント
    CurrentButtonIndex.Changed <- (fun i oi ->
        sidePanel.Invalidate(false)
        )
    /// 行為の効果対象変化イベント
    TargetArea.Changed <- (fun cps ocps ->
        invalidateChipPointSet cps ocps
        )
    /// 行為のスコープ変化イベント
    Scope.Changed <- (fun scope oldScope ->
        let getArea s =
            match s with
            | None -> Set.empty
            | Some(s) -> s.Area
        invalidateChipPointSet (getArea scope) (getArea oldScope)
        setTargetArea scope CurrentMapPoint.Value
        )
    /// コマンドパネルのマウス位置変化イベント
    CurrentCommandIndex.Changed <- (fun i oi ->
        cmdPanel.Invalidate(false)
        )
    /// リーダーの位置変化イベント
    LeaderPoint.Changed <- (fun p op ->
        let leader = war.Map.[p].Unit.Value
        LeaderArea.Set (ScopeUtil.normalWithoutSelf war leader.Data.RevWidth p)
        )
    /// リーダーの効果範囲変化イベント
    LeaderArea.Changed <- (fun cps ocps ->
        invalidateChipPointSet cps ocps
        )
    mapPanel.Size <- cp2sz war.MapSize
    form.Invalidate(true)
    WarUtil.startTurn war

let Start () =
    ShowScreen titleImage
    Initialize ()

let GameOver () =
    GameContinued <- false
    ShowScreen gameOverImage
    war <- loadWar StageIndex
    Initialize ()

let Clear () =
    if StageIndex < 3 then
        ShowScreen stageClearImage
        StageIndex <- StageIndex + 1
    else
        ShowScreen gameClearImage
        StageIndex <- 1
    war <- loadWar StageIndex
    Initialize ()

Start ()