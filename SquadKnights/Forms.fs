module Forms

open System.Drawing
open Paraiba.Drawing
open System.Windows.Forms
open Paraiba.Windows.Forms
open Paraiba.Utility

open WarLoader
open Utility
open ImageManager

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
/// マップパネル用のスクロールバー付きパネル
let scrollablePanel =
    let panel = new ScrollablePanel()
    panel.Dock <- DockStyle.Fill
    panel.Panel <- mapPanel
    panel

/// メインパネル（マップパネル＋ステータスパネル）
let splitContainer =
    let container = new SplitContainer(Dock = DockStyle.Fill,
                                        Panel1MinSize = 0,
                                        Panel2MinSize = 0,
                                        FixedPanel = FixedPanel.Panel2,
                                        IsSplitterFixed = true,
                                        SplitterWidth = 1,
                                        SplitterDistance = 0)
    container.Panel1.SuspendLayout()
    container.Panel2.SuspendLayout()
    container.SuspendLayout()
    container.Panel1.Controls.Add scrollablePanel
    container.Panel2.Controls.Add sidePanel
    container

/// メインフォーム
let mainForm =
    let form = new Form(Size = new Size(800, 600))
    form.SuspendLayout()
    form.Controls.Add splitContainer
    splitContainer.Panel1.ResumeLayout(false)
    splitContainer.Panel2.ResumeLayout(false)
    splitContainer.ResumeLayout(false)
    form.ResumeLayout(false)
    form.Visible <- true
    form

/// コマンドウィンドウ
let cmdForm = 
    let form = new Form()
    form.SuspendLayout()
    form.Controls.Add(cmdPanel)
    form.FormBorderStyle <- FormBorderStyle.FixedToolWindow
    form.ControlBox <- false
    form.ClientSize <- new Size(112, 32 * 4)
    form.ResumeLayout(false)
    form.Show(mainForm)
    form.Visible <- false
    form

splitContainer.SplitterDistance <- mainForm.Width - sideWidth - 16

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
    mainForm.Controls.Clear()
    mainForm.Controls.Add startPanel
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
        mainForm.Controls.Add splitContainer
        splitContainer.Visible <- true
        GameContinued <- true
        )
        
let invalidateChipPoints cps =
    cps |> Seq.iter (fun cp -> mapPanel.Invalidate(cp2rect cp))

let invalidateChipPointSet cps ocps =
    Set.difference cps ocps |> invalidateChipPoints
    Set.difference ocps cps |> invalidateChipPoints
