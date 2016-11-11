using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;

public class HighScore {

	internal static GameHighScores Save(string gameId, string gameHash, int gameScore, int returnCount) {
		GameHighScores allScores = Load(gameId, gameHash);
		allScores.Scores.Add(new UserScore(gameScore));
		allScores.Scores = allScores.Scores.OrderBy(x => x.Score).ToList();
		string jsoned = JsonUtility.ToJson(new HashedScores() { { gameHash, allScores } });
		PlayerPrefs.SetString("scores." + gameId, jsoned);
		return allScores;
	}

	public static GameHighScores Load(string gameId, string gameHash) {
		string jsoned = PlayerPrefs.GetString("scores." + gameId, "{}");
		HashedScores hs = JsonUtility.FromJson<HashedScores>(jsoned);
		if (!hs.ContainsKey(gameHash)) {
			hs.Add(gameHash, new GameHighScores());
		}
		return hs[gameHash];
	}
}

[Serializable]
public class HashedScores : SerializableDictionary<string, GameHighScores> {
}

[Serializable]
public class GameHighScores {
	public List<UserScore> Scores = new List<UserScore>();
}

[Serializable]
public class UserScore {
	public int Score;

	public UserScore(int gameScore) {
		Score = gameScore;
	}
}