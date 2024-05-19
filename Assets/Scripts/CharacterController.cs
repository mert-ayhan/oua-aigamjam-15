using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private int initialTime = 1000;
    private int currentTime;
    private readonly string timeUnit = "SAAT";

    [SerializeField] private TextMeshProUGUI _remainingTimeText;

    void Start()
    {
        currentTime = initialTime;
        _remainingTimeText.text = currentTime + " " + timeUnit;
    }
    
    void Update()
    {
    }

    public void DecreaseTime(int time)
    {
        currentTime -= time;
        _remainingTimeText.text = currentTime + " " + timeUnit;
    }

    public void IncreaseTime(int time)
    {
        currentTime += time;
        _remainingTimeText.text = currentTime + " " + timeUnit;
    }
}