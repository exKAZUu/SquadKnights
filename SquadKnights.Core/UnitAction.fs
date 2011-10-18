module UnitAction

open SquadKnights.Entity

let moveUnit (src : ChipPoint) (dst : ChipPoint) (war : War) =
    let unit = war.Units |> Map.find src
    let movedUnits = war.Units |> Map.remove src |> Map.add dst unit  
    { war with Units = movedUnits }

    