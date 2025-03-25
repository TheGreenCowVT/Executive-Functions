using UnityEngine;

public class TestingEXP : MonoBehaviour
{
    [SerializeField] public playerController player;
    [Range(0,100), SerializeField] int expTestingAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player.expAmount += expTestingAmount;
        }
    }
}
