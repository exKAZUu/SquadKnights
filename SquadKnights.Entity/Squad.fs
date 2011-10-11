namespace SquadKnights.Entity

/// 分隊
type Squad =
    {
        /// Waiting time（残り待機時間）
        Wt : float<day>
        /// リーダーが生きているかどうか
        IsLeaderLive : bool
    }
