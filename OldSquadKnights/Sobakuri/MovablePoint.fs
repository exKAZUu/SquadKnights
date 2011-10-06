namespace Sobakuri.Entity

open Utility

type MovablePoint(p : ChipPoint, from : Option<MovablePoint>, remains : float) =
    member this.ChipPoint
        with get () = p

    member this.From
        with get () = from

    member this.Remains
        with get () = remains

    override this.ToString() = p.ToString()
