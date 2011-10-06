module Script

// namespace
open System
open System.IO
open Sobakuri.Entity

let engine = IronPython.Hosting.Python.CreateEngine()
let scope = engine.ExecuteFile(Path.Combine("script", "battle.py"))
let calcProb = scope.GetVariable<Func<WarUnit, WarUnit, Action, int>>("calc_porb")
let calcEffect = scope.GetVariable<Func<WarUnit, WarUnit, Action, int>>("calc_effect")
System.Console.WriteLine (scope.Engine.ToString())
