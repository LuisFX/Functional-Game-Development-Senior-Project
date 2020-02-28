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
        GameState.instance.gamedata.highestFloor = (highestFloor |> int)
        GameState.instance.gamedata.highestRemaining = (highestRemaining |> float)
        GameState.instance.gamedata.lowestTotal = (lowestTotal |> float)
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
                    let newGameData = { GameState.instance.gamedata with lowestTotal = GameState.instance.gamedata.totaltime }
                    GameState.instance <- { GameState.instance with gamedata = newGameData }
                    ()
                | _ -> ()
                let newGameData = { GameState.instance.gamedata with highestRemaining = GameState.instance.gamedata.time }
                GameState.instance <- { GameState.instance with gamedata = newGameData }
                ()

            | _ -> 
                match lowestTotal with
                | lowestTotal when lowestTotal |> float > GameState.instance.gamedata.totaltime ->
                    let newGameData = { GameState.instance.gamedata with lowestTotal = GameState.instance.gamedata.totaltime }
                    GameState.instance <- { GameState.instance with gamedata = newGameData }
                    ()
                | _ -> ()
            let newGameData = { GameState.instance.gamedata with highestFloor = GameState.instance.gamedata.floor }
            GameState.instance <- { GameState.instance with gamedata = newGameData }
            ScoreSavingService.storeScores ([|GameState.instance.gamedata.highestFloor |> string; 
                                                GameState.instance.gamedata.highestRemaining |> string; 
                                                GameState.instance.gamedata.lowestTotal |> string|])
            "GameOver" |> SceneManager.LoadScene
        | _ -> 
            match GameState.instance.gamedata.time with
            | 0.0 -> 
                match highestFloor with 
                | highestFloor when highestFloor |> int < GameState.instance.gamedata.floor ->
                    let newGameData = { GameState.instance.gamedata with highestFloor = GameState.instance.gamedata.floor }
                    GameState.instance <- { GameState.instance with gamedata = newGameData }
                    ()
                | _ -> ()
                ScoreSavingService.storeScores ([|GameState.instance.gamedata.highestFloor |> string; 
                                                    GameState.instance.gamedata.highestRemaining |> string; 
                                                    GameState.instance.gamedata.lowestTotal |> string|])
                "GameOver" |> SceneManager.LoadScene
            | _ -> ()
        ()