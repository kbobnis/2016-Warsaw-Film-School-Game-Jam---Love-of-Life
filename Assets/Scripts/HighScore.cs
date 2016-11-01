using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HighScore {

	internal static List<UserScore> Save(string gameId, int gameScore, int returnCount) {
		List<UserScore> allScores = Load(gameId);
		allScores.Add(new UserScore(gameScore));
		allScores = allScores.OrderByDescending(x => x.Score).ToList();
		ListWrapper lw = new ListWrapper();
		lw.AddRange(allScores);
		string serialized = JsonUtility.ToJson(lw);
		Debug.Log("scores serialized: " + serialized);
		PlayerPrefs.SetString("scores." + gameId, serialized);
		return allScores.Take(returnCount).ToList();
	}

	private static List<UserScore> Load(string gameId) {
		ListWrapper scores = JsonUtility.FromJson<ListWrapper>(PlayerPrefs.GetString("scores." + gameId));
		if (scores == null) {
			scores = new ListWrapper();
		}
		return (List<UserScore>)scores;
	}
}

[Serializable]
public class ListWrapper : List<UserScore> { }

[Serializable]
public class UserScore {
	public readonly int Score;

	public UserScore(int gameScore) {
		Score = gameScore;
	}
}