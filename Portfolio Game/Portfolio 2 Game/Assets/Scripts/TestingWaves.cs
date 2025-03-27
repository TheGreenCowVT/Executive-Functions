using System.Collections;
using UnityEngine;

public class TestingWaves : MonoBehaviour
{
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if(other.CompareTag("Player") && timer >= 3)
        //{
        //    timer = 0;
        //    StartCoroutine(waveAdvance());
        //    gamemanager.instance.waveNum++;
        //    gamemanager.instance.waveNumberText.text = gamemanager.instance.waveNum.ToString("F0");
        //    gamemanager.instance.UpdateWaveBar();
        //}
        
    }

    IEnumerator waveAdvance()
    {
        Debug.Log("Waiting Happened");
        yield return new WaitForSeconds(3);
    }
}
