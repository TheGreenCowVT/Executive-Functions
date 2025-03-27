using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [Header("----Menus----")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuHUD;
    [SerializeField] public GameObject levelUp;

    [SerializeField] public Transform playerTargetPos;
    public GameObject playerSpawnPos;
    public bool isPaused;

    [Header("----Player----")]
    public Image playerHeartBeat;
    public Image playerHPBar;
    public Image playerExpBar;
    public GameObject playerDamageScreen;
    public GameObject player;
    public GameObject playerDamageSpot;
    public playerController playerScript;
    [SerializeField] public Image weaponIcon;
    [SerializeField] public Image activeWeaponIcon;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text ammoCount;
    [SerializeField] TMP_Text currentLevelCount;

    [Header("----Enemy----")]
    public GameObject enemy;
    public GameObject enemyHP;
    public GameObject[] enemies;
    public GameObject[] enemiesHP;
    [SerializeField] public Image enemyHPBar;

    [Header("----Wave----")]
    public float waveTimerCountdownCurrent;
    private float waveTimerCountdownMax;
    [SerializeField] public TMP_Text enemyKillCount;
    public int enemyKillCountInt;
    public int numEnemies;
    public int numEnemiesOrig;
    int levelCount;
    public int waveNum = 0;
    public Image waveTimer;
    public EnemySpawner enemySpawner;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerDamageSpot = GameObject.FindWithTag("PlayerDamageSpot");

        //playerTargetPos = playerDamageSpot.transform;

        playerScript = player.GetComponent<playerController>();

        enemyHP = GameObject.FindWithTag("EnemyHPBar");
        enemiesHP = GameObject.FindGameObjectsWithTag("EnemyHPBar");
        // Find enemies and waypoints
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemy = GameObject.FindWithTag("Enemy");
        

        //Find Enemy HP bars
        enemyHP = GameObject.FindWithTag("EnemyHPBar");
        enemiesHP = GameObject.FindGameObjectsWithTag("EnemyHPBar");

        if (enemyHP != null)
        {
            foreach (GameObject enemyHPBar in enemiesHP)
            {
                Image enemyHPBarImage = enemyHPBar.GetComponent<Image>();
                if (enemyHPBarImage != null)
                {
                    enemyHPBarImage.fillAmount = 1;
                }
            }
            enemyHPBar = enemyHP.GetComponent<Image>();
        }
        else
        {
            Debug.Log("Enemy HP Bar not found");
        }

        waveTimerCountdownCurrent = 300;
        waveTimerCountdownMax = 300;
    }

    // Update is called once per frame
    void Update()
    {
        waveTimerCountdownCurrent -= Time.deltaTime;
        UpdateWaveBar();

        if (waveTimerCountdownCurrent <= 0.01f || waveNum >= 5)
        {
            youWin();
        }
        if (waveTimerCountdownCurrent <= 0.01f && waveNum < 5)
        {
            youLose();
        }

        if (waveTimerCountdownCurrent % 100 == 0)
        {
            enemySpawner.StartNextWave();
        }


        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);

            }
            else if (menuActive == menuPause)
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
        
        
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuHUD.SetActive(true);
        menuActive.SetActive(false);
        menuActive = null;

    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void updateEnemyHPBar(float enemyHP, float currentenemyHP)
    {
        if (enemyHPBar != null)
        {
            enemyHPBar.fillAmount = currentenemyHP / enemyHP;
        }
    }

    public void UpdateWaveBar()
    {
        waveTimer.fillAmount = waveTimerCountdownCurrent / waveTimerCountdownMax;
    }

    //public void UpdateWeaponUI()
    //{
    //    activeWeaponIcon = playerScript.weaponList[playerScript.weaponListPos].UIImage;
    //    weaponIcon.sprite = activeWeaponIcon.sprite;

    //    if (playerScript.weaponList[playerScript.weaponListPos].ammoMax > 0)
    //    {
    //        ammoText.enabled = true;
    //        ammoCount.enabled = true;
    //        ammoCount.text = playerScript.weaponList[playerScript.weaponListPos].ammoCur.ToString() + " / " + playerScript.weaponList[playerScript.weaponListPos].ammoMax.ToString();
    //    }
    //    else if (playerScript.weaponList[playerScript.weaponListPos].ammoMax == 0 || playerScript.weaponList.Count <= 0)
    //    {
    //        ammoText.enabled = false;
    //        ammoCount.enabled = false;
    //    }
    //}

    public void updateLevelCount()
    {
        levelCount++;
        currentLevelCount.text = levelCount.ToString("F0");
        

    }



}
