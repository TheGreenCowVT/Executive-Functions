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
    public bool isPaused;

    [Header("----Player----")]
    public Image playerHeartBeat;
    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public GameObject player;
    public PlayerController playerScript;
    [SerializeField] public Image weaponIcon;
    [SerializeField] TMP_Text ammoCount;

    [Header("----Enemy----")]
    public GameObject enemy;
    public GameObject enemyHP;
    public GameObject[] enemies;
    public GameObject[] enemiesHP;
    [SerializeField] public Image enemyHPBar;

    [Header("----Waypoint----")]
    public GameObject waypoint;
    public Transform[] waypoints;
    public GameObject[] enemywaypoints;

    [Header("----Wave----")]
    [SerializeField] TMP_Text waveNumberText;
    public int numEnemies;
    public int numEnemiesOrig;
    int goalCount;
    int waveNum = 0;
    public Image waveTimer;
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");

        playerScript = player.GetComponent<PlayerController>();

        enemyHP = GameObject.FindWithTag("EnemyHPBar");
        enemiesHP = GameObject.FindGameObjectsWithTag("EnemyHPBar");
        StartNextWave();
        // Find enemies and waypoints
        //enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemy = GameObject.FindWithTag("Enemy");
        enemywaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        waypoint = GameObject.FindWithTag("Waypoint");

        if (waypoint != null)
        {
            waypoints = waypoint.GetComponentsInChildren<Transform>();
            Transform[] tempWaypoints = new Transform[waypoints.Length - 1];
            for (int i = 1; i < waypoints.Length; i++)
            {
                tempWaypoints[i - 1] = waypoints[i];
            }
            waypoints = tempWaypoints;
        }

        // Assign player target and waypoints to enemies
        if (player != null && waypoints != null && enemies != null && enemies.Length > 0)
        {
            foreach (GameObject enemyObj in enemies)
            {
                EnemyPatrol enemyPatrol = enemyObj.GetComponent<EnemyPatrol>();
                if (enemyPatrol != null)
                {
                    enemyPatrol.playerTarget = player.transform;
                    enemyPatrol.waypoints = waypoints;
                }
            }
        }
        else
        {
            Debug.LogError("Player, waypoints, or enemies not found.");
        }

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

        updateGameGoal(0);


        UpdateWaveBar();


    }

    // Update is called once per frame
    void Update()
    {
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

    public void updateGameGoal(int amount)
    {
        goalCount += amount;
        numEnemies += amount;


        UpdateWaveBar();

        if (goalCount <= 0)
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
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void StartNextWave()
    {
        waveNum++;
        waveNumberText.text = "Wave " + waveNum.ToString();

        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        goalCount = numEnemies = enemies.Length;
        numEnemiesOrig = numEnemies;

        UpdateWaveBar();

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

    public void UpdateWeaponUI()
    {
        weaponIcon = playerScript.weaponList[playerScript.weaponListPos].UIImage;
        if (playerScript.weaponList[playerScript.weaponListPos].ammoMax > 0)
        {
            ammoCount.text = playerScript.weaponList[playerScript.weaponListPos].ammoCur.ToString() + " / " + playerScript.weaponList[playerScript.weaponListPos].ammoMax.ToString();
        }
        else if (playerScript.weaponList[playerScript.weaponListPos].ammoMax == 0 || playerScript.weaponList.Count <= 0)
        {
            ammoCount.text = "";
        }
    }
}
