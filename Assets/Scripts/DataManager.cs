using System.Collections.Generic;

[System.Serializable]
public class RaceData
{
    public int place;
    public string name;
    public float time;

    public RaceData(int place, string name, float time)
    {
        this.place = place;
        this.name = name;
        this.time = time;
    }
}

[System.Serializable]
public class GameData
{
    public string date;
    public string lobbyName;
    public int numberOfPlayers;
    public List<RaceData> raceDataList = new List<RaceData>();
}

[System.Serializable]
public class AllGameData
{
    public List<GameData> allGames = new List<GameData>();
}
[System.Serializable]
public class Settings
{
    public float volume = 0.5f;
    public float musicVolume = 0.5f;
    public int qualityIndex = 2;
    public bool isFullScreen = true;
    public int resolutionIndex = 0;
    public int targetFPS = 60;
}
