namespace FSharp.Interpretation

open UnityEngine
open Interpretation.Utility




type UserControllerScript() =
    inherit MonoBehaviour()
    member this.Update() =
        if Input.GetKeyDown(this.right)
            then Debug.Log("Player moved right. Call move right function here.")
        if Input.GetKeyDown(this.left)
            then Debug.Log("Player moved left. Call move left function here.")
        if Input.GetKeyDown(this.up)
            then Debug.Log("Player moved up. Call move up function here.")
        if Input.GetKeyDown(this.down)
            then Debug.Log("Player moved down. Call move down function here.")

    member this.left : string = controlSettings.read("Left")
    member this.right : string = controlSettings.read("Right")
    member this.up : string = controlSettings.read("Up")
    member this.down : string = controlSettings.read("Down")


        


