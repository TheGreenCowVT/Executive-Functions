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
    public Image playerExpBar;
    public GameObject playerDamageScreen;
    public GameObject player;
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

    [Header("----Waypoint----")]
    public GameObject waypoint;
    public Transform[] waypoints;
    public GameObject[] enemywaypoints;

    [Header("----Wave----")]
    [SerializeField] TMP_Text waveNumberText;
    public int numEnemies;
    public int numEnemiesOrig;
    int goalCount;
    int levelCount;
    int waveNum = 0;
    public Image waveTimer;
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");

        playerScript = player.GetComponent<playerController>();

        enemyHP = GameObject.FindWithTag("EnemyHPBar");
        enemiesHP = GameObject.FindGameObjectsWithTag("EnemyHPBar");
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

        numEnemiesOrig = numEnemies;

        updateNumEnemies(0);


        UpdateWaveBar();

        levelCount = 1;
        currentLevelCount.text = levelCount.ToString("F0");

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

    public void updateNumEnemies(int amount)
    {
        numEnemies += amount;


        UpdateWaveBar();

       
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
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

    public void UpdateWeaponUI()
    {
        activeWeaponIcon = playerScript.weaponList[playerScript.weaponListPos].UIImage;
        weaponIcon.sprite = activeWeaponIcon.sprite;

        if (playerScript.weaponList[playerScript.weaponListPos].ammoMax > 0)
        {
            ammoText.enabled = true;
            ammoCount.enabled = true;
            ammoCount.text = playerScript.weaponList[playerScript.weaponListPos].ammoCur.ToString() + " / " + playerScript.weaponList[playerScript.weaponListPos].ammoMax.ToString();
        }
        else if (playerScript.weaponList[playerScript.weaponListPos].ammoMax == 0 || playerScript.weaponList.Count <= 0)
        {
            ammoText.enabled = false;
            ammoCount.enabled = false;
        }
    }

    public void updateLevelCount()
    {
        levelCount++;
        currentLevelCount.text = levelCount.ToString("F0");
    }
}
