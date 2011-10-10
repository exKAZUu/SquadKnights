module UnitAction

open SquadKnights.Entity

let moveUnit (war : War) (src : ChipPoint) (dst : ChipPoint) =
    let unit = war.Units |> Map.find src
    let movedUnits = war.Units |> Map.remove src |> Map.add dst unit  
    { war with Units = movedUnits }

    