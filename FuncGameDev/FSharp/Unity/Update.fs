namespace FSharp.Unity

open UnityEngine
open UnityEngine.SceneManagement

type Updater() = 
    inherit MonoBehaviour()

    let highScores = ScoreSavingService.getScores
    let highestFloor = highScores.[0]
    let highestRemaining = highScores.[1]
    let lowestTotal = highScores.[2]

    member this.Start() =
        GameState.instance <- GameState.createInitialGameState ()
        GameObjectWrapper.wrappers <- Map.empty
        LevelGameObject.stairs <- null
        Spawner.spawnPlayer (GameState.instance.level.stairpos |> fst |> float, GameState.instance.level.stairpos |> snd |> float)
        Generator.generateLevel GameState.instance
        ()

    member this.Update() =
        CameraManager.updateCamera()
        GameState.instance <- GameObjectWrapper.wrappers |> CommonEntityUpdater.updateGameStateEntities GameState.instance
        Time.deltaTime |> float |> GameDataUtils.decreaseTime |> Commands.addCommand
        UpdaterDispatcher.issueUpdateCommands GameState.instance GameObjectWrapper.wrappers
        GameState.instance.entities |> Map.iter UserController.tryQueryInput
        GameState.instance.entities |> Map.iter EnemyAIScript.callEnemyAI
        GameState.instance <- Commands.executeAllCommands GameState.instance
        UpdaterDispatcher.updateAllGameObjects GameState.instance GameObjectWrapper.wrappers
        Spawner.spawnGameObjects GameState.instance |> Map.iter (fun key value -> GameObjectWrapper.addWrapper value)
        Generator.tryGenerateLevel GameState.instance
        GameState.instance <- GameStateUtils.removeMarkedEntities GameState.instance
        GameState.instance.killIds |> Destroyer.update
        CameraUpdater.update()
        this.checkGameOver()
        ()

    member this.checkGameOver() =
        match GameState.instance.gamedata.floor with 
        | 11 ->
            match highestRemaining with
            | highestRemaining when highestRemaining |> float < GameState.instance.gamedata.time ->
                match lowestTotal with
                | lowestTotal when lowestTotal |> float > GameState.instance.gamedata.totaltime ->
                    ScoreSavingService.storeScores ([|"11"; GameState.instance.gamedata.time |> string; GameState.instance.gamedata.totaltime |> string|])
                | _ ->
                    ScoreSavingService.storeScores ([|"11"; GameState.instance.gamedata.time |> string; lowestTotal|])
            | _ -> 
                match lowestTotal with
                | lowestTotal when lowestTotal |> float > GameState.instance.gamedata.totaltime ->
                    ScoreSavingService.storeScores ([|"11"; highestRemaining; GameState.instance.gamedata.totaltime |> string|])
                | _ ->
                    ScoreSavingService.storeScores ([|"11"; highestRemaining; lowestTotal|])
            "GameOver" |> SceneManager.LoadScene
        | _ -> 
            match GameState.instance.gamedata.time with
            | 0.0 -> 
                match highestFloor with 
                | highestFloor when highestFloor |> int < GameState.instance.gamedata.floor ->
                    ScoreSavingService.storeScores ([|GameState.instance.gamedata.floor |> string; highestRemaining; lowestTotal|])
                | _ -> ()
                "GameOver" |> SceneManager.LoadScene
            | _ -> ()
        ()