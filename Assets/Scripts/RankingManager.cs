using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class RankingEntry {
    public string name;
    public int score;
    public float time;
    public string date;
}

[Serializable]
public class RankingData {
    public List<RankingEntry> entries = new List<RankingEntry>();
}

public static class RankingManager {
    private const int MAX_ENTRIES = 10;

    public static void SaveRecord(string saveKey, string name, int score, float time) {
        RankingData data = LoadData(saveKey);
        
        RankingEntry newEntry = new RankingEntry {
            name = name,
            score = score,
            time = time,
            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        };

        data.entries.Add(newEntry);
        
        // Sort by score (descending) and take top MAX_ENTRIES
        data.entries = data.entries.OrderByDescending(e => e.score).Take(MAX_ENTRIES).ToList();

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
    }

    public static RankingData LoadData(string saveKey) {
        if (PlayerPrefs.HasKey(saveKey)) {
            string json = PlayerPrefs.GetString(saveKey);
            
            // Fix potential format issues
            if (json.StartsWith("[") && json.EndsWith("]")) {
                json = json.Substring(1, json.Length - 2);
            }

            try {
                RankingData data = JsonUtility.FromJson<RankingData>(json);
                if (data != null && data.entries != null) return data;
            } catch (Exception e) {
                Debug.LogError("Failed to load ranking data for key " + saveKey + ": " + e.Message);
            }
        }
        return new RankingData();
    }
}