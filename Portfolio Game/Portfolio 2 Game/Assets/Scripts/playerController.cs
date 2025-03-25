using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerController : MonoBehaviour, IDamage, IPickup
{

    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [Header("----- Stats -----")]
    [Range(1, 10000)][SerializeField] int HP; // Set back to 10 after testing is over
    [Range(2, 5)][SerializeField] int speed;
    [Range(2, 4)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(15, 45)][SerializeField] int gravity;



    [Header("----- Weapon -----")]
    [SerializeField] public List<Weapon> weaponList = new List<Weapon>();
    [SerializeField] GameObject weaponModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    [Header("----- Grapple -----")]
    [SerializeField] int grappleDist;
    [SerializeField] int grappleSpeed;
    [SerializeField] LineRenderer grappleLine;

    int HPOrig;
    int jumpCount;
    public int weaponListPos;
    float shootTimer;

    public float rotationSpeed;

    Vector3 moveDir;
    Vector3 playerVelocity;

    public Transform handTransform;
    public Weapon currentWeapon;
    public Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        animator = GetComponent<Animator>();
        updatePlayerUI();

    }

    // Update is called once per frame
    void Update()
    {

        if (currentWeapon != null)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * currentWeapon.shootDist, Color.red);

            movement();
            updatePlayerUI();
            

            animator.SetBool("isGrounded", controller.isGrounded);
            animator.SetFloat("velocityY", playerVelocity.y + 0.001f);
        }

    }
    void movement()
    {
        shootTimer += Time.deltaTime;
        sprint();

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;

        }

        // Basic Movement & Camera Rotation

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraForward.Normalize();


        Vector3 moveDir = (cameraForward * verticalInput) + (cameraRight * horizontalInput);
        moveDir.Normalize();

        controller.Move(moveDir * speed * Time.deltaTime);
        if (moveDir != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        jump();
        isGrappling();

        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && weaponList.Count > 0 && weaponList[weaponListPos].ammoCur > 0 && shootTimer >= shootRate)
            shoot();


        selectWeapon();
        reloadWeapon();


        // Animation movement controls

        animator.SetFloat("Horizontal", horizontalInput);
        animator.SetFloat("Vertical", verticalInput);
        animator.SetBool("IsSprinting", Input.GetButton("Sprint"));
        animator.SetFloat("Speed", Mathf.Round(moveDir.magnitude * 100f) / 100f);


    }


    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
            animator.SetBool("IsJumping", true);

        }
        if (controller.isGrounded && animator.GetBool("IsJumping") == true && playerVelocity.y + 0.001f <= 0.1f)
        {
            animator.SetBool("IsJumping", false);
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
        if (Input.GetButton("Sprint") && Input.GetButton("Fire2"))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grappleDist))
            {
                if (hit.collider.CompareTag("GrapplePoint"))
                {
                    grappleLine.enabled = true;
                    grappleLine.SetPosition(0, transform.position);
                    grappleLine.SetPosition(1, hit.point);

                    if (Input.GetButtonDown("Fire1"))
                    {
                        controller.Move((hit.point - transform.position).normalized * grappleSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    grappleLine.enabled = false;
                }
            }
            else
            {
                grappleLine.enabled = false;


            }

        }
        else
        {
            grappleLine.enabled = false;
        }

        
    }


        public void shoot()
    {
        shootTimer = 0;

        weaponList[weaponListPos].ammoCur--;
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
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());

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
        gamemanager.instance.UpdateWeaponUI();
    }

    void reloadWeapon()
    {
        if (Input.GetButtonDown("Reload"))
        {
            weaponList[weaponListPos].ammoCur = weaponList[weaponListPos].ammoMax;
        }
    }

}