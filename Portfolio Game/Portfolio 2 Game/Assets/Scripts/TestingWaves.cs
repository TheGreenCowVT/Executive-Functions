using UnityEngine;

public class TestingWaves : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        gamemanager.instance.StartNewWave();
    }
    public void killAllEnemies()
    {
        for (int en = 0; en > 0; en++)
        {
            Destroy(gamemanager.instance.enemies[en]);
            gamemanager.instance.updateNumEnemies(-(en - 1));
        }
    }
}
