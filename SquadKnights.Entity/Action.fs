namespace SquadKnights.Entity

/// 行為（攻撃，回復...）
type Action =
    {
        /// 行為の種類
        ActKind : string
        /// 行為の対象（TODO: Affiliationでは？）
        ActTarget : string
        /// 行為の威力
        ActPower : int
        /// 行為の射程
        ActRange : int
        /// 行為の適用範囲
        ActWidth : int
    }
