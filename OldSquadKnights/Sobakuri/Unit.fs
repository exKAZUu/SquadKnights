namespace Sobakuri.Entity

type G(h : E) =
    let _h = h
and E(g : G) =
    let _g = g
    new () as this = new E (new G(this))

type Unit =
    {
        Hp : int;
        Atk : int;
        Def : int;
        Skl : int;
        Agi : int;
        Mov : int;
        Affiliation : Affiliation;
        Squad : Squad;
        OptionLeader : Option<Unit>;
        UnitData : UnitData;
    }

//    member this.IsLive
//        with get () = this.Hp > 0
//
//    member this.IsLeader
//        with get () = this.OptionLeader.IsNone
//
//    member this.Leader
//        with get () = 
//            match this.OptionLeader with
//            | None -> this
//            | Some(leader) -> leader
