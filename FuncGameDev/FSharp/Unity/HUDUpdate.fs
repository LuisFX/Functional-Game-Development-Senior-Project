﻿namespace FSharp.Unity

open UnityEngine


// This script must be placed on a gameobject called HUD with a TextMesh
type HUDUpdate() = 
  inherit MonoBehaviour()
  
  member this.Update() = 
      let camera = GameObject.Find("Main Camera")
      let txt = this.GetComponent<TextMesh>()
      let time : int = int GameState.instance.gamedata.time
      let time_string : string = string time
      txt.text <- "Time: " + time_string
      this.transform.position <- new Vector3(0.0f, 4.5f, 5.0f) + camera.transform.position
      ()