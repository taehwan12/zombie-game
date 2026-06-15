using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RankingUI : MonoBehaviour {
    public GameObject recordPrefab; // 랭킹 항목 프리팹
    public Transform contentParent; // 항목들이 생성될 부모 (ScrollView의 Content)
    public string mapSaveKey = "Ranking_Park"; // 표시할 맵의 랭킹 키

    private void Start() {
        LoadRanking();
    }

    public void LoadRanking() {
        // 기존 리스트 삭제
        foreach (Transform child in contentParent) {
            Destroy(child.gameObject);
        }

        // 데이터 로드
        RankingData data = RankingManager.LoadData(mapSaveKey);

        if (data == null || data.entries == null) {
            Debug.LogWarning("No ranking data found.");
            return;
        }

        // 랭킹 항목 생성
        for (int i = 0; i < data.entries.Count; i++) {
            var entry = data.entries[i];
            GameObject obj = Instantiate(recordPrefab, contentParent);
            obj.SetActive(true); // 프리팹이 비활성화 상태일 수 있으므로 활성화
            
            // 프리팹 내부의 텍스트들 설정 (이름, 점수, 시간 등)
            // 인덱스 0: 순위, 1: 이름, 2: 점수, 3: 시간
            TextMeshProUGUI[] texts = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (texts.Length >= 4) {
                texts[0].text = (i + 1).ToString();
                texts[1].text = entry.name;
                texts[2].text = entry.score.ToString();
                texts[3].text = string.Format("{0:0.0}s", entry.time);
                
                // 글씨가 잘 보이도록 색상 보정
                foreach (var t in texts) t.color = Color.white;
            }
        }
    }

    public void BackToIntro() {
        SceneManager.LoadScene("Intro");
    }
}