using TMPro; // Import TextMeshPro namespace
using UnityEngine;

public class ClockHandler : MonoBehaviour
{
    // Starting time in hours and minutes (can be customized)
    public Vector2 startTime = new Vector2(12, 0); // Starting at 12 (AM)
    public bool isAM = true; // Set initial period (AM/PM)

    // Time speed multiplier (1 = real time, >1 = faster, <1 = slower)
    public float timeSpeed = 60f; // 1 minute per second (60x speed of real time)

    private float elapsedSeconds = 0f;

    private void Start()
    {
        startTime = GlobalInfo.i.timeVectors[TimeState.Time8];
    }
    void Update()
    {
        // Increment elapsed time based on the timeSpeed factor
        elapsedSeconds += Time.deltaTime * timeSpeed;

        // Convert elapsed time into minutes and update clock accordingly
        if (elapsedSeconds >= 60f) // 60 seconds = 1 minute
        {
            IncrementClock();
            elapsedSeconds = 0f; // Reset seconds after each minute update
        }

        // Update the TextMeshPro UGUI clock display
        UpdateClockDisplay();
    }

    // Method to increment the clock time by 1 minute
    void IncrementClock()
    {
        startTime.y += 2;

        // Check if we've passed 60 minutes
        if (startTime.y >= 60)
        {
            startTime.y = 0;
            startTime.x += 1;

            if (startTime.x <= 12)
            {
                isAM = !isAM; // Toggle AM/PM when it hits 12
            }
        }
    }

    // Method to update the TextMeshPro UGUI text with the current time in HH:MM AM/PM format
    void UpdateClockDisplay()
    {
        string period = isAM ? "AM" : "PM";
        GlobalUIInfo.i.clockText.text = string.Format("{0:00}:{1:00} {2}", startTime.x%12, startTime.y, period);
        GlobalInfo.i.globalTime = startTime;
    }
}
