using UnityEngine;

public class Flicker : MonoBehaviour
{

    [SerializeField] GameObject torch;
    [SerializeField] Light light;
    [Range(0,30), SerializeField] float lightMax;
    [Range(0,30), SerializeField] float lightMin;
    private float flickerTimer;
    [SerializeField] float flickerTimerMin;
    [SerializeField] float flickerTimeMax;

    [Range(0,5), SerializeField] float flickerFrequency;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        light = torch.GetComponent<Light>();
    }
    // Update is called once per frame
    void Update()
    {
        // Constantly increasing
        flickerTimer += Time.deltaTime;

        if (flickerTimer >= flickerFrequency)
        {
            light.intensity = Random.Range(lightMin, lightMax);
            flickerTimer = 0;

            flickerFrequency = Random.Range(0f, .3f);
        }
        

    }
}
