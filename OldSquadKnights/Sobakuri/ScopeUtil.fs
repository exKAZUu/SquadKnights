module ScopeUtil

open Sobakuri.Entity

open Utility

let arrange (war : War) cps =
    cps
    |> Seq.filter (fun cp -> war.Map |> Map.containsKey cp)
    |> Set.ofSeq

let normalWithoutSelf war range p =
    seq {
        for y = 1 to range do
            for x = 0 to range - y do
                yield { X = p.X - x * 1<chip>; Y = p.Y - y * 1<chip> }
                yield { X = p.X + x * 1<chip>; Y = p.Y + y * 1<chip> }
                yield { X = p.X - y * 1<chip>; Y = p.Y + x * 1<chip> }
                yield { X = p.X + y * 1<chip>; Y = p.Y - x * 1<chip> }
    }
    |> arrange war

let normal war range p =
    normalWithoutSelf war range p
    |> Set.add p
            