using System.Collections;
using UnityEngine;

public class FireSpinnerEvent : MonoBehaviour
{

    [SerializeField] GameObject spinner;
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spinner = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spinner.SetActive(true);
        }

        DisableTrap();
    }

    IEnumerator DisableTrap()
    {
        
        spinner.SetActive(false);
        yield return new WaitForSeconds(10f);
        
    }
        
    
}
