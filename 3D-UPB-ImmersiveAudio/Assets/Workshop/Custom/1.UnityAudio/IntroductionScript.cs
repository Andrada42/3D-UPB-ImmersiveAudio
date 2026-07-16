using System.Xml.Serialization;
using UnityEngine;

public class IntroductionScript : MonoBehaviour
{
    
    
    private void Awake()
    {
        Debug.Log("Awake");
    }
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Update");
    }
}
