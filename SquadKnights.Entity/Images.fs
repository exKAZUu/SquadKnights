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
        .SplitToSurfaces(sideWidth, 480)
/// 数字画像
let numberImages =
    BitmapUtil.Load(Path.Combine("image", "window", "number.png"), transparentColor)
        .SplitToSurfaces(4, 7)
/// 記号画像
let signImages =
    BitmapUtil.Load(Path.Combine("image", "window", "sign.png"), transparentColor)
        .SplitToSurfaces(3, 7)
/// ミニユニットウィンドウ
let unitMiniImages =
    BitmapUtil.Load(Path.Combine("image", "window", "unitmini.png"), transparentColor)
        .SplitToSurfaces(80, 32)
/// 詳細ユニットウィンドウ
let unitDetailImages =
    BitmapUtil.Load(Path.Combine("image", "window", "unitdetail.png"), transparentColor)
        .SplitToSurfaces(160, 128)
/// 詳細ウィンドウボタン
let buttonImages =
    BitmapUtil.Load(Path.Combine("image", "window", "button.png"), transparentColor)
        .SplitToSurfaces(160, 32)
/// マップチップ
let mapchipSurfaces =
    BitmapUtil.Load(Path.Combine("image", "mapchip.png"), transparentColor)
        .SplitToSurfaces(ChipPixelSize)
/// 勲章
let medalSurfaces =
    BitmapUtil.Load(Path.Combine("image", "unit", "medal.png"), transparentColor)
        .SplitToSurfaces(ChipPixelSize)
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
