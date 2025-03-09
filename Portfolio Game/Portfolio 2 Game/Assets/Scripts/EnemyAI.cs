using UnityEngine;
using System.Collections;



public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;

    [Range(1, 10)][SerializeField] int HP;

  

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        if ((HP<= 0))
        {
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
}
