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
        /// 基礎移動力（不変）
        BaseMov : int
        /// 基礎HP（不変）
        BaseHp : int
        /// 基礎ATK（不変）
        BaseAtk : int
        /// 基礎DEF（不変）
        BaseDef : int
        /// 基礎SKL（不変）
        BaseSkl : int
        /// 基礎AGI（不変）
        BaseAgi : int
        /// 基礎チップグラフィック（不変）
        BaseChip : Surface
        /// 基礎顔グラフィック（不変）
        BaseFace : Surface
        /// 通常攻撃
        DefaultAction : Action
        /// 特殊攻撃
        SpecialAction : Action
        /// リーダー効果の範囲
        SquadWidth : int
        /// リーダーによる移動力補正
        SquadMov : int
        /// リーダーによるATK補正
        SquadAtk : int
        /// リーダーによるDEF補正
        SquadDef : int
        /// リーダーによるSKL補正
        SquadSkl : int
        /// リーダーによるAGI補正
        SquadAgi : int
        /// リーダーによるWT補正
        SquadWt : int
    }
