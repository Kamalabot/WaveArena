using PrimeTween;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameManager>();
            }
            return instance;
        }
    }

    [SerializeField] private TextMeshProUGUI scoreText;
    private int score;
    private Vector3 initialScoreScale;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (scoreText != null)
        {
            initialScoreScale = scoreText.transform.localScale;
        }
    }

    public void AddScore(int points)
    {
        score += points;
        if (scoreText == null) return;

        scoreText.text = $"SCORE: {score}";

        Tween.Scale(scoreText.transform, endValue: initialScoreScale * 1.3f, duration: 0.08f, ease: Ease.OutQuad)
            .OnComplete(() => Tween.Scale(scoreText.transform, endValue: initialScoreScale, duration: 0.12f, ease: Ease.OutBack));
    }
}