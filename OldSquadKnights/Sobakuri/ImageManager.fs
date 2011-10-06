module ImageManager

open System.IO
open System.Drawing
open Paraiba.Drawing
open Paraiba.Drawing.Surfaces

// modules
open Utility

let transparentColor = Color.FromArgb(1, 1, 1)

/// 詳細パネルの背景画像
let sideWidth = 160
let sideImages =
    BitmapUtil.Load(Path.Combine("image", "window", "sideback.png"), transparentColor)
        .SplitToBitmaps(sideWidth, 480)
    |> Array.map (fun b -> b.ToSurface())
/// 数字画像
let numberImages =
    BitmapUtil.Load(Path.Combine("image", "window", "number.png"), transparentColor)
        .SplitToBitmaps(4, 7)
    |> Array.map (fun b -> b.ToSurface())
/// 記号画像
let signImages =
    BitmapUtil.Load(Path.Combine("image", "window", "sign.png"), transparentColor)
        .SplitToBitmaps(3, 7)
    |> Array.map (fun b -> b.ToSurface())
/// ミニユニットウィンドウ
let unitMiniImages =
    BitmapUtil.Load(Path.Combine("image", "window", "unitmini.png"), transparentColor)
        .SplitToBitmaps(80, 32)
    |> Array.map (fun b -> b.ToSurface())
/// 詳細ユニットウィンドウ
let unitDetailImages =
    BitmapUtil.Load(Path.Combine("image", "window", "unitdetail.png"), transparentColor)
        .SplitToBitmaps(160, 128)
    |> Array.map (fun b -> b.ToSurface())
/// 詳細ウィンドウボタン
let buttonImages =
    BitmapUtil.Load(Path.Combine("image", "window", "button.png"), transparentColor)
        .SplitToBitmaps(160, 32)
    |> Array.map (fun b -> b.ToSurface())
/// マップチップ
let mapchipSurfaces =
    BitmapUtil.Load(Path.Combine("image", "mapchip.png"), transparentColor)
        .SplitToBitmaps(24, 24)
    |> Array.map (fun b -> b.ToSurface())
/// 勲章
let medalSurfaces =
    BitmapUtil.Load(Path.Combine("image", "unit", "medal.png"), transparentColor)
        .SplitToBitmaps(24, 24)
    |> Array.map (fun b -> b.ToSurface())
/// ゲームスタート
let titleImage = BitmapUtil.Load(Path.Combine("image", "screen", "Title.png"))
/// ゲームオーバー
let gameOverImage = BitmapUtil.Load(Path.Combine("image", "screen", "GameOver.png"))
/// ステージクリア
let stageClearImage = BitmapUtil.Load(Path.Combine("image", "screen", "StageClear.png"))
/// ゲームクリア
let gameClearImage = BitmapUtil.Load(Path.Combine("image", "screen", "GameClear.png"))
/// フォント
let font = new Font("ＭＳ ゴシック", float32 9, FontStyle.Bold)
