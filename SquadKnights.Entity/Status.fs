namespace SquadKnights.Entity

/// 基本的なステータス
type Status =
    {
        /// HP（ヒットポイント）
        Hp : int
        /// Attack（攻撃力）
        Atk : int
        /// Defence（防御力）
        Def : int
        /// Skill（技量）
        Skl : int
        /// Agility（素早さ）
        Agi : int
        /// Movement（移動力）
        Mov : int
    }

    override this.ToString() =
        "{ Hp : " + this.Hp.ToString() +
        ", Atk : " + this.Atk.ToString() +
        ", Def : " + this.Def.ToString() +
        ", Skl : " + this.Skl.ToString() +
        ", Agi : " + this.Agi.ToString() +
        ", Mov : " + this.Mov.ToString() +
        " }";

