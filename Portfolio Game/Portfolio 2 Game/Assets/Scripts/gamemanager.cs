using UnityEngine;

public class gamemanager : MonoBehaviour
{

    public static gamemanager instance;



    public GameObject playerDamageScreen;
    public GameObject player;
    public PlayerController playerScript;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {



    }


}
