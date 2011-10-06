namespace SquadKnights.Entity

/// マップを構成する1チップ
[<Measure>]
type chip

/// マップの座標系
type ChipPoint =
    {
        X : int<chip>
        Y : int<chip>
    }
    override this.ToString() = "{ X : " + this.X.ToString() + ", Y : " + this.Y.ToString() + " }";

/// マップの座標系のサイズ
type ChipSize =
    {
        W : int<chip>
        H : int<chip>
    }
    override this.ToString() = "{ W : " + this.W.ToString() + ", H : " + this.H.ToString() + " }";
