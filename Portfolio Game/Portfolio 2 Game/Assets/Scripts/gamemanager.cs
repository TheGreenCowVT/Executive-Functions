using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuHUD;
    [SerializeField] TMP_Text waveNumberText;

    public Image WaveTimer;
    public Image playerHPBar;
    public Image enemyHPBar;
    public GameObject playerDamageScreen;
    public GameObject player;
    public PlayerController playerScript;

    public bool isPaused;

    int numEnemies;
    int goalCount;
    int waveNum = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        StartNextWave();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);

            }
            else if(menuActive == menuPause)
            {
                StateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive = menuPause;
        menuHUD.SetActive(false);
        menuActive.SetActive(true);
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState  = CursorLockMode.Locked;
        menuHUD.SetActive(true);
        menuActive.SetActive(false);
        menuActive = null;

    }

    public void updateGameGoal(int amount)
    {
        goalCount += amount;

        if(goalCount <= 0)
        {
            if (waveNum > 4)
            {
                // You Win
                statePause();
                menuActive = menuWin;
                menuActive.SetActive(true);
            }
            else
            {
                // Start Next Wave
                StartNextWave();
            }
        }
    }

    public void youLose()
    {
        statePause();
        playerDamageScreen.SetActive(false);
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void StartNextWave()
    {
        waveNum++;
        waveNumberText.text = "Wave " + waveNum.ToString();
    }
}
