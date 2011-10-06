module Utility

open System
open System.Drawing
open System.Windows.Forms

open Paraiba.Geometry

open SquadKnights.Entity


/// グローバルな乱数生成器
let random = new Random()
/// seq to Map
let (|MapOfSeq|) = Map.ofSeq
/// seq to list
let (|ListOfSeq|) = List.ofSeq
/// string to int
let (|Int|) = System.Int32.Parse
/// string to float
let (|Float|) = System.Double.Parse

/// string to int
let toInt = System.Int32.Parse
/// string to float
let toFloat = System.Double.Parse
/// ReferenceEquals
let refeql = (fun a b -> System.Object.ReferenceEquals(a, b))

/// 1チップの横幅のピクセルサイズ
let ChipWidth = 24</chip>
/// 1チップの縦幅のピクセルサイズ
let ChipHeight = 24</chip>
/// 1チップのピクセルサイズ
let ChipPixelSize = new Size(ChipWidth * 1<chip>, ChipHeight * 1<chip>)

let pt2cp (p : Point) = { X = p.X / ChipWidth; Y = p.Y / ChipHeight }
let cp2pt p = new Point(p.X * ChipWidth, p.Y * ChipHeight)
let cp2pt2 p = new Point2(p.X * ChipWidth, p.Y * ChipHeight)
let cp2sz s = new Size(s.W * ChipWidth, s.H * ChipHeight)
let cp2rect p = new Rectangle(cp2pt p, ChipPixelSize)

/// チップ間のマンハッタン距離
let manhattan p1 p2 = (abs (p1.X - p2.X) + abs (p1.Y - p2.Y)) * 1</chip>

let (|Pt2Cp|) = pt2cp
let (|Cp2Pt|) = cp2pt
let (|Cp2Pt2|) = cp2pt2
let (|Cp2Sz|) = cp2sz

let (|MouseEvent2Cp|) (e : MouseEventArgs) = pt2cp e.Location
let (|MouseEvent2Pt|) (e : MouseEventArgs) = e.Location
let (|MouseEvent2Pt2|) (e : MouseEventArgs) = new Point2(e.X, e.Y)
let (|PaintEvent2Graphics|) (e : PaintEventArgs) = e.Graphics

let NextPoints from = 
    seq {
        yield { X = from.X + 1<chip>; Y = from.Y + 0<chip> }
        yield { X = from.X - 1<chip>; Y = from.Y - 0<chip> }
        yield { X = from.X + 0<chip>; Y = from.Y + 1<chip> }
        yield { X = from.X - 0<chip>; Y = from.Y - 1<chip> }
    }

let statusNames =
    [| "HP"; "ATK"; "DEF"; "SKL"; "AGI"; "MOV"; "KIND" |]

let mutable SquadCount = -1
let NextSquadId () = SquadCount <- SquadCount + 1; SquadCount

let mutable UnitCount = -1
let NextUnitId () = UnitCount <- UnitCount + 1; UnitCount

let iterator start (array : 'a[]) =
    let value = ref start
    (fun () -> value := !value + 1; array.[!value])

let mutable maxId = 0
let recordUsedId id = maxId <- max maxId id; id
