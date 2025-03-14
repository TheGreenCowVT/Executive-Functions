using UnityEngine;

public class PlayerAiming : MonoBehaviour
{

    public GameObject bow;
    public GameObject reticle;
    public float aimSpeed = 5f;
    public Vector3 aimPos;

    private Vector3 hipPos;
    private bool isAiming = false;
    private PlayerController playerController;

    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hipPos = bow.transform.localPosition;
        reticle.SetActive(false);
        playerController = GetComponent<PlayerController>();
       

        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        AimingLogic();
        BowTransition();



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

            playerController.shoot();
            animator.SetBool("atReady", false);
        }
    }

    void BowTransition()
    {
        if (isAiming)
        {
            bow.transform.localPosition = Vector3.Lerp(bow.transform.localPosition, aimPos, Time.deltaTime * aimSpeed);
        }
        else
        {
            bow.transform.localPosition = Vector3.Lerp(bow.transform.localPosition, aimPos, Time.deltaTime * aimSpeed);
        }
    }

}
