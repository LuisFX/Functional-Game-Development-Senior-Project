﻿module GameState

type T = {
    entities : Collections.Map<int, CommonEntityData.T>; // map from id to entity
    level: LevelData.T;
    gamedata: GameData.T;
    nextid: int // the next created entity will have this id
    spawnIds: int list
    killIds: int list
    updateLevel: bool
    random: System.Random
}

let createInitialGameState () =
    {
    entities = Map.empty;
    level = {
        LevelData.grid = [];
        LevelData.validTiles = [];
        LevelData.size = (0,0);
        LevelData.startpos = (0,0);
        LevelData.stairpos = (-14,-7);
        LevelData.complete = false;
        LevelData.generator = 1
    };
    gamedata = {
        GameData.time = 60.0;
        GameData.floor = 0
        GameData.camera = { 
            position = (-0.5, -0.5);
            rotation = (0.0, 0.0);
            scale = (1.0, 1.0);
            width = 36.0;
            height = 18.0;
            shakeTime = 0.0;
        }
    };
    nextid = 1
    spawnIds = []
    killIds = []
    updateLevel = false
    random = new System.Random()
}

let mutable instance: T = createInitialGameState ()

