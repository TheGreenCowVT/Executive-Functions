using UnityEngine;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    public Image roundBar;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public bool isPaused;
    public Text waveText;
    int roundNum;
    [Range(0,23)][SerializeField] public int numEnemies;
    [Range(0,23)][SerializeField] public int numEnemiesOrig;

    void Awake()
    {
        instance = this;
        roundNum = 0;
        StartNextWave();
    }

    private void Update()
    {
        UpdateRoundBarUI();
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
            }
            else if (menuPause != null)
            {
                StateUnpause();
            }
        }
        TestEnemiesDying();

        if (numEnemies == 0)
        {
            StartNextWave();
        }
        
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive = menuPause;
        menuActive.SetActive(true);
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void UpdateRoundBarUI()
    {
        roundBar.fillAmount = (float)numEnemies / numEnemiesOrig;

    }

    public void UpdateWaveTitleUI()
    {
        waveText.text = "Round " + roundNum.ToString();
    }

    public void StartNextWave()
    {
        roundNum++;
        //spawn enemies
        numEnemies = numEnemiesOrig;
        UpdateWaveTitleUI();
        UpdateRoundBarUI();
    }

    public void TestEnemiesDying()
    {
        UpdateRoundBarUI();
    }

}
