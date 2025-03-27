using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class playerController : MonoBehaviour, IDamage, IPickup
{

    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [Header("----- Stats -----")]
    [Range(1, 10000)][SerializeField] public int HP; // Set back to 10 after testing is over
    [Range(2, 10)][SerializeField] public float speed;
    [Range(2, 4)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(15, 45)][SerializeField] int gravity;



    [Header("----- Weapon -----")]
    [SerializeField] public List<Weapon> weaponList = new List<Weapon>();
    [SerializeField] GameObject weaponModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] public float shootRate;

    [Header("----- Grapple -----")]
    [SerializeField] int grappleDist;
    [SerializeField] int grappleSpeed;
    [SerializeField] LineRenderer grappleLine;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;

    int HPOrig;
    int jumpCount;
    public int weaponListPos;
    public float shootTimer;

    public float rotationSpeed;
    bool isplayingSteps;
    bool isSprinting;

    Vector3 moveDir;
    Vector3 playerVelocity;

    public Transform handTransform;
    public Weapon currentWeapon;
    public Animator animator;

    public int expAmount;
    public int expMax;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        animator = GetComponent<Animator>();
        if (weaponList.Count > 0)
        {
            weaponListPos = 0;
            changeWeapon();
        }
        spawnPlayer();
        updatePlayerUI();
        //gamemanager.instance.UpdateWeaponUI();

        expAmount = 0;
        expMax = 100;

    }

    // Update is called once per frame
    void Update()
    {
        //if (expAmount >= expMax)
        //{
        //    levelUp();
        //}

        if (!gamemanager.instance.isPaused)
            movement();

        
        updatePlayerUI();

    }
    void movement()
    {
        shootTimer += Time.deltaTime;
        sprint();

        if (controller.isGrounded)
        {
            if (moveDir.magnitude > 0.3f /*&& !isplayingSteps*/)
                //   StartCoroutine(playSteps());
                jumpCount = 0;
            playerVelocity = Vector3.zero;

        }

        // Basic Movement & Camera Rotation

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(moveDir * speed * Time.deltaTime);

        jump();
        isGrappling();

        if (Input.GetButtonDown("Fire1") && weaponList.Count > 0 && weaponList[weaponListPos].ammoCur > 0 && shootTimer >= shootRate)
            shoot();


        selectWeapon();
        reloadWeapon();


        // Animation movement controls
        animator.SetFloat("Speed", Mathf.Round(moveDir.magnitude * 100f) / 100f);

        //IEnumerator playSteps()
        //{
        //    isplayingSteps = true;
        //    aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        //    if (!isSprinting)
        //        yield return new WaitForSeconds(0.5f);
        //    else
        //        yield return new WaitForSeconds(0.5f);

        //    isplayingSteps = false;
        //}

    }


    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
            //   aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
    }


    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void isGrappling()
    {
        if (Input.GetButton("Fire2") && grapple())
        {
            grappleLine.enabled = true;

        }
        else
        {
            grappleLine.enabled = false;
            controller.Move(playerVelocity * Time.deltaTime);
            playerVelocity.y -= gravity * Time.deltaTime;

        }

    }

    bool grapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grappleDist))
        {
            if (hit.collider.CompareTag("Grapple Point"))
            {
                controller.Move((hit.point - transform.position).normalized * grappleSpeed * Time.deltaTime);


                grappleLine.SetPosition(0, transform.position);
                grappleLine.SetPosition(1, hit.point);

                return true;
            }

        }

        return false;
    }


    void shoot()
    {
        shootTimer = 0;

        animator.SetTrigger("Shoot");

        weaponList[weaponListPos].ammoCur--;
        //        aud.PlayOneShot(weaponList[weaponListPos].shootSound[Random.Range(0, weaponList[weaponListPos].shootSound.Length)], weaponList[weaponListPos].shootVol);
        //gamemanager.instance.UpdateWeaponUI();
        Weapon currentWeapon = weaponList[weaponListPos];
        Transform projectilePos = handTransform;

        if (currentWeapon.projectilePrefab != null)
        {
            GameObject projectile = Instantiate(currentWeapon.projectilePrefab, projectilePos.position, projectilePos.rotation);

            damage projectileDamage = projectile.GetComponent<damage>();
            if (projectileDamage != null)
            {
                projectileDamage.SetDamageAmount(currentWeapon.shootDamage);
            }

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = projectilePos.forward * currentWeapon.projectileSpeed;
            }
        }
        animator.ResetTrigger("Shoot");
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());
        //  aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        if (HP <= 0)
        {
            gamemanager.instance.youLose();

        }
    }

    IEnumerator flashDamageScreen()
    {
        gamemanager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageScreen.SetActive(false);

    }


    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        gamemanager.instance.playerExpBar.fillAmount = (float)expAmount / expMax;
    }


    // Weapon Functions

    public void getWeaponStats(Weapon gun)
    {
        weaponList.Add(gun);
        weaponListPos = weaponList.Count - 1;
        changeWeapon();

    }

    void selectWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && weaponListPos < weaponList.Count - 1)
        {
            weaponListPos++;
            changeWeapon();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && weaponListPos > 0)
        {
            weaponListPos--;
            changeWeapon();
        }
    }

    void changeWeapon()
    {
        shootDamage = weaponList[weaponListPos].shootDamage;
        shootDist = weaponList[weaponListPos].shootDist;
        shootRate = weaponList[weaponListPos].shootRate;
        weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponList[weaponListPos].model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weaponList[weaponListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
        //gamemanager.instance.UpdateWeaponUI();
    }

    void reloadWeapon()
    {
        if (Input.GetButtonDown("Reload"))
        {
            weaponList[weaponListPos].ammoCur = weaponList[weaponListPos].ammoMax;
            //gamemanager.instance.UpdateWeaponUI();
        }
    }

    void levelUp()
    {
        HP += 3;
        speed += 0.5f;
        expAmount = 0;
        gamemanager.instance.updateLevelCount();
        StartCoroutine(levelUpNotification());

    }

    IEnumerator levelUpNotification()
    {
        gamemanager.instance.levelUp.SetActive(true);

        yield return new WaitForSeconds(3f);

        gamemanager.instance.levelUp.SetActive(false);
        updatePlayerUI();
    }

    public void spawnPlayer()
    {
        controller.transform.position = gamemanager.instance.playerSpawnPos.transform.position;
        HP = HPOrig;
        updatePlayerUI();

    }

}