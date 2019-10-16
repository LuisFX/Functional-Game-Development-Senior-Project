namespace Interpretation.Utility

open System.IO
open System
open UnityEngine

type ControlParser() =
    member this.Read() =
        use stream = new StreamReader @"C:\Users\daileyab\Desktop\Game_Controls.txt"

        let mutable valid = true
        while (valid) do
            let line = stream.ReadLine()
            if (line = null) then
                valid <- false
            else
                Debug.Log(line)