using UnityEngine;

public class turnOnOff : MonoBehaviour
{
    [SerializeField] GameObject levelItem;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            levelItem.SetActive(true);
           

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            levelItem.SetActive(false);
          
        }
    }
}
