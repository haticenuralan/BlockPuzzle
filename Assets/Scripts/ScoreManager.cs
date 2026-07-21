using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int currentScore = 0;
    private int pointsPerLine = 10;

    public TextMeshProUGUI scoreText;
    public GridManager gridManager;

    void Awake()
    {
        // Basit bir Singleton: sahnede tek bir ScoreManager olsun,
        // diğer scriptler ona kolayca erişebilsin
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        // GridManager'ın "satırlar temizlendi" haberine abone oluyoruz.
        // GridManager bizim varlığımızı bilmiyor, sadece haber yayınlıyor.
        if (gridManager != null)
        {
            gridManager.OnLinesCleared += HandleLinesCleared;
        }
    }

    void OnDisable()
    {
        // Sahneden kaldırıldığımızda aboneliği iptal ediyoruz,
        // yoksa yok olmuş bir objeye event göndermeye çalışır ve hata verir.
        if (gridManager != null)
        {
            gridManager.OnLinesCleared -= HandleLinesCleared;
        }
    }

    void Start()
    {
        UpdateScoreText();
    }

    private void HandleLinesCleared(int linesCleared)
    {
        AddScore(linesCleared);
    }

    public void AddScore(int linesCleared)
    {
        // Birden fazla satır aynı anda temizlenirse bonus ver (basit bir combo mantığı)
        int points = linesCleared * pointsPerLine;
        if (linesCleared > 1)
        {
            points += linesCleared * 5; // küçük bir combo bonusu
        }

        currentScore += points;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }
}