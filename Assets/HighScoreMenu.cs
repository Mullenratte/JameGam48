using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class HighScoreMenu : MonoBehaviour
{

    [SerializeField] Transform contentParent;
    [SerializeField] GameObject entryPrefab;
    [SerializeField] GameObject noEntryPrefab;

    void Start() {
        var entries = HighScoreManager.Instance.LoadHighscoreList();

        if (entries.Length == 0) {
            GameObject obj = Instantiate(noEntryPrefab, contentParent);
            return;
        }

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var entry in entries) {
            GameObject obj = Instantiate(entryPrefab, contentParent);
            var texts = obj.GetComponentsInChildren<TMPro.TMP_Text>();
            texts[0].text = entry.playerName;
            texts[1].text = entry.score.ToString();
        }
    }
}
