using System;
using System.Collections.Generic;
using UnityEngine;

public class HighScore {

	private static SerializableDictionary<string, List<UserScore>> HighScores;
	private const int Limit = 100;

	static HighScore() {
		return;
		string json = PlayerPrefs.GetString("highScores");
		SerializableDictionary<string, List<UserScore>> tmp = JsonUtility.FromJson<SerializableDictionary<string, List<UserScore>>>(json);
		HighScores = tmp;
		if (HighScores == null) {
			HighScores = new SerializableDictionary<string, List<UserScore>>();
		}
	}

	internal static void SaveGameScore(string gameId, int gameTime) {
		return;
		if (!HighScores.ContainsKey(gameId)) {
			HighScores.Add(gameId, new List<UserScore>());
		}
		foreach(UserScore usTmp in HighScores[gameId]) {
			usTmp.Latest = false;
		}
		HighScores[gameId].Add(new UserScore(gameTime, true));
		HighScores[gameId].Sort((t, y) => t.Hours.CompareTo(y.Hours));
		string json = JsonUtility.ToJson(HighScores);
		PlayerPrefs.SetString("highScores", json);
		PlayerPrefs.Save();
	}

	internal static List<UserScore> GetHighScore(string gameId) {
		return new List<UserScore>();
		return HighScores[gameId];
	}

}

[Serializable]
public class UserScore {
	public int Hours;
	public bool Latest;

	private UserScore() { }
	public UserScore(int gameTime, bool latest=false) {
		Hours = gameTime;
		Latest = latest;
	}

	public override string ToString() {
		return Hours + " godzin. " + (Latest?"<< obecny wynik":"");
	}
}

