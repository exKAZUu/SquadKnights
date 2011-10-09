namespace SquadKnights.Entity

open System.IO
open System.Drawing
open Paraiba.Drawing
open Paraiba.Drawing.Surfaces

open Settings
open Utility

/// ユニットデータ（不変）
type UnitData =
    {
        /// ユニット名
        BaseName : string
        /// ユニットの種類
        BaseKind : string
        /// 基本的な基礎ステータス
        BaseStatus : Status
        /// 基礎チップグラフィック
        BaseChip : Surface
        /// 基礎顔グラフィック
        BaseFace : Surface
        /// 通常攻撃
        DefaultAction : Action
        /// 特殊攻撃
        SpecialAction : Action
        /// リーダー効果の範囲
        SquadWidth : int
        /// リーダーによるステータス補正
        SquadStatus : Status
    }
