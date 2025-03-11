using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour, IDamage
{

    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [Range(1, 10)][SerializeField] int HP;
    [Range(2, 5)][SerializeField] int speed;
    [Range(2, 4)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(15, 45)][SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    int HPOrig;
    int jumpCount;

    float shootTimer;

    private bool isLanding = false;

    Vector3 moveDir;
    Vector3 playerVelocity;

    public Animator animator;
    public RuntimeAnimatorController playerArcher;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        movement();

        sprint();
    }
    void movement()
    {
        shootTimer += Time.deltaTime;


        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);
        jump();
        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;



        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
            animator.SetTrigger("Shoot");
        }

        // Animation movement controls

        animator.SetFloat("Speed", moveDir.magnitude);
        animator.SetFloat("Horizontal", horizontalInput);
        animator.SetFloat("Vertical", verticalInput);
        animator.SetBool("IsSprinting", Input.GetButton("Sprint"));



    }

        void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
            animator.SetBool("IsJumping", true);

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

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(shootDamage);
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

}