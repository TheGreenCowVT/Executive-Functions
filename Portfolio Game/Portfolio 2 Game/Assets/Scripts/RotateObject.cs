using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Slider Controls")]
    [Range(0, 360)][SerializeField] private float xAxisSlider = 0f;
    [Range(0, 360)][SerializeField] private float yAxisSlider = 0f;
    [Range(0, 360)][SerializeField] private float zAxisSlider = 0f;
    [Range(0, 360)][SerializeField] private float speedSlider = 100f;

    [Header("Trigger")]
    [SerializeField] private bool isRotating = false;

    private void Update()
    {
        if (isRotating)
        {
            rotationAxis = new Vector3(xAxisSlider, yAxisSlider, zAxisSlider).normalized;
            rotationSpeed = speedSlider;
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            isRotating = true;


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {

            isRotating = false;
        }
    }
}