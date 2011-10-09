namespace SquadKnights.Entity

open Paraiba.Drawing
open Paraiba.Drawing.Surfaces

// ユニット
type Unit =
    {
        /// 基本的なステータス補正
        Status : Status
        /// 所属
        Affiliation : Affiliation
        /// 分隊ID
        SquadId : int
        /// リーダーか否か
        IsLeader : bool
        /// ユニットデータ（不変）
        Data : UnitData
        /// コマンド番号と使用可能か否かの対応表
        CommandAvailabilities : Map<int, bool>
        /// チップグラフィック
        UnitImage : Surface
    }

    member this.IsLive
        with get () = this.Status.Hp > 0
