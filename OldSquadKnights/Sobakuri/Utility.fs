module Utility

open System
open System.Drawing
open System.Windows.Forms
open Paraiba.Geometry

let random = new Random()

let (|MapOfSeq|) = Map.ofSeq
let (|Int|) = System.Int32.Parse
let (|ListOfSeq|) = List.ofSeq

let intSeq = seq { 0 .. System.Int32.MaxValue }
let toInt = System.Int32.Parse
let toFloat = System.Double.Parse
let refeql = (fun a b -> System.Object.ReferenceEquals(a, b))

[<Measure>]
type chip

type ChipPoint =
    { X : int<chip>; Y : int<chip> }
    override this.ToString() = "{ X : " + this.X.ToString() + ", Y : " + this.Y.ToString() + " }";

type ChipSize =
    { W : int<chip>; H : int<chip> }

let ChipWidth = 24
let ChipHeight = 24
let ChipPixelSize = new Size(ChipWidth, ChipHeight)

let pt2cp (p : Point) = { X = p.X / 24</chip>; Y = p.Y / 24</chip> }
let cp2pt p = new Point(p.X * 24</chip>, p.Y * 24</chip>)
let cp2pt2 p = new Point2(p.X * 24</chip>, p.Y * 24</chip>)
let cp2sz s = new Size(s.W * 24</chip>, s.H * 24</chip>)
let cp2rect p = new Rectangle(cp2pt p, ChipPixelSize)

let distance p1 p2 = (abs (p1.X - p2.X) + abs (p1.Y - p2.Y)) * 1</chip>

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