module Global

// namespace
open Paraiba.Geometry
open Paraiba.Utility
open Sobakuri.Entity

// module
open Utility

// コマンドによって計算されたスコープとコマンド実行
type PrintableScope =
    {
        Area : Set<ChipPoint>;
        ValidArea : Set<ChipPoint>;
        GetTargetArea : ChipPoint -> Set<ChipPoint>;
        Boot : ChipPoint -> unit -> unit
    }

/// 現在のマウス位置
let mutable CurrentMapPoint = MonitoredWrap.Create({ X = 0<chip>; Y = 0<chip> })

/// 現在のマウス位置にいるユニット
let mutable CurrentMapUnit = MonitoredWrap.Create(Option<WarUnit>.None)

/// 現在のコマンド位置
let mutable CurrentCommandIndex = MonitoredWrap.Create(-1)

/// 現在のコマンド位置
let mutable CurrentButtonIndex = MonitoredWrap.Create(-1)

/// 最後にクリックした位置
let mutable CurretActive = Option<ChipPoint * Chip * WarUnit>.None

/// 移動可能範囲
let mutable Scope = MonitoredWrap.Create(Option<PrintableScope>.None)

/// 行為の適用範囲
let mutable TargetArea = MonitoredWrap.Create<Set<ChipPoint>>(Set.empty)

/// リーダーの位置
let mutable LeaderPoint = MonitoredWrap.Create<ChipPoint>({ X = -1<chip>; Y = -1<chip> })

/// リーダーの効果範囲
let mutable LeaderArea = MonitoredWrap.Create<Set<ChipPoint>>(Set.empty)