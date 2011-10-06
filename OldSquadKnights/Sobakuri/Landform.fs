namespace Sobakuri.Entity

open Utility

type Landform(line : string[]) =
    let mutable _hp = toInt(line.[2])
    let mutable _atk = toInt(line.[3])
    let mutable _def = toInt(line.[4])
    let mutable _skl = toInt(line.[5])
    let mutable _agi = toInt(line.[6])
    let mutable _cost = toFloat(line.[7])

    member this.HpVar
        with get () = _hp
        and set value = _hp <- value

    member this.AtkRev
        with get () = _atk
        and set value = _atk <- value

    member this.DefRev
        with get () = _def
        and set value = _def <- value

    member this.SklRev
        with get () = _skl
        and set value = _skl <- value

    member this.AgiRev
        with get () = _agi
        and set value = _agi <- value

    member this.MovCost
        with get () = _cost
        and set value = _cost <- value
