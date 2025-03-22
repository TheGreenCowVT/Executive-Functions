using UnityEngine;

public class PlayerAiming : MonoBehaviour
{

    
    public GameObject reticle;
    public float aimSpeed = 5f;
    public Vector3 aimPos;

    private bool isAiming = false;
    private PlayerController playerController;

    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        reticle.SetActive(false);
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.currentWeapon != null)
        {
            AimingLogic();
            BowTransition();

        }

    }


    void AimingLogic()
    {
        animator.SetBool("atReady", true);

        if (Input.GetButtonDown("Fire2"))
        {
            isAiming = true;
            Debug.Log("isAiming set to true");
            reticle.SetActive(true);
            animator.SetBool("isAiming", true);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            isAiming = false;
            Debug.Log("isAiming set to False");
            reticle.SetActive(false);
            animator.SetBool("isAiming", false);

        }
        if (isAiming && Input.GetButtonDown("Fire1"))
        {
            if (playerController.currentWeapon != null)
            {
                playerController.shoot();
                animator.SetBool("atReady", false);
            }
            }
        }

    void BowTransition()
    {
        if (isAiming)
        {
            playerController.transform.localPosition = Vector3.Lerp(playerController.transform.localPosition, aimPos, Time.deltaTime * aimSpeed);
        }
        else
        {
            playerController.transform.localPosition = Vector3.Lerp(playerController.transform.localPosition, aimPos, Time.deltaTime * aimSpeed);
        }
    }

}
