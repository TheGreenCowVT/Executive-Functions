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
    [SerializeField] TMP_Text waveNumberText;
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


        updateNumEnemies(0);

        
        StartNewWave();

    }

    // Update is called once per frame
    void Update()
    {
        if (waveNum >= 3)
        {
            youWin();
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

    public void updateNumEnemies(int amount)
    {
        numEnemies += amount;

        UpdateWaveBar();

        if (numEnemies <=0)
        {
            if (waveNum < 4)
            {
                StartNewWave();
            }
            else if(waveNum == 4)
            {
                youWin();
            }
        }

       
    }

    public void advanceWave()
    {
        waveNum++;
        waveNumberText.text = waveNum.ToString("F0");
        
        UpdateWaveBar();
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
        gamemanager.instance.waveTimer.fillAmount = (float)numEnemies / numEnemiesOrig;
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

    public void StartNewWave()
    {
        UpdateWaveBar();
        waveNum++;
        waveNumberText.text = waveNum.ToString("F0");
    }

}
