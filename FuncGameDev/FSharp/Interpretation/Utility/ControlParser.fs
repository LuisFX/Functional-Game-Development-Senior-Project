module ControlParser

open System.IO
open System

type ControlParser() =
    member x.Read() =
        use stream = new StreamReader @"C:\Users\daileyab\Desktop\"

        let mutable valid = true
        while (valid) do
            let line = stream.ReadLine()
            if (line = null) then
                valid <- false
            else
                printfn "%A" line