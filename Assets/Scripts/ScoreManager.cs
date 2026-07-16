using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int currentScore = 0;
    private int pointsPerLine = 10;

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

    public void AddScore(int linesCleared)
    {
        // Birden fazla satır aynı anda temizlenirse bonus ver (basit bir combo mantığı)
        int points = linesCleared * pointsPerLine;
        if (linesCleared > 1)
        {
            points += linesCleared * 5; // küçük bir combo bonusu
        }

        currentScore += points;
        Debug.Log("Score: " + currentScore);
    }
}