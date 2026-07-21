using UnityEngine;

public class DebugGameOverTrigger : MonoBehaviour
{
    public GameOverManager gameOverManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (gameOverManager != null)
            {
                gameOverManager.TriggerGameOver();
            }
        }
    }
}