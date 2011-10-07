namespace SquadKnights.Entity

open Paraiba.Drawing
open Paraiba.Drawing.Surfaces

// ユニット
type Unit =
    {
        /// HP
        Hp : int
        /// ATK
        Atk : int
        /// DEF
        Def : int
        /// SKL
        Skl : int
        /// AGI
        Agi : int
        /// MOV
        Mov : int
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
        with get () = this.Hp > 0
