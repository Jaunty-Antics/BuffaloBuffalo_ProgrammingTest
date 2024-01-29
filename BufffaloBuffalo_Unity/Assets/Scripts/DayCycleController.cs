using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TIMEOFDAY
{
    NONE,
    MORNING,
    AFTERNOON,
    EVENING,
}

public class DayCycleController : MonoBehaviour
{
    private static DayCycleController instgance;
    public static DayCycleController Instance
    {
        get
        {
            instgance = FindObjectOfType<DayCycleController>();
            return instgance;
        }
    }

    public float CurrentDayTime;
    
    public TIMEOFDAY CurrentTimeOfDay;
    private TIMEOFDAY PreviousTimeOfDay;

    public UnityEvent TimeOfDayChange;

    public Light Sun;
    public Material Skybox;


    private void Start()
    {
        SetTime(Random.Range(0f, 1f));
    }

    private void Update()
    {
       if (Input.mouseScrollDelta.magnitude > 0)
        {
            SetTime(CurrentDayTime += Input.mouseScrollDelta.y * 0.1f);
        }
    }

    public void SetTime(float NewTime)
    {
        //Set time value and update enum for character properties
        CurrentDayTime = Mathf.Clamp01(NewTime);


        if(CurrentDayTime < 0.33f) CurrentTimeOfDay = TIMEOFDAY.MORNING;
        else if(CurrentDayTime < 0.66) CurrentTimeOfDay = TIMEOFDAY.AFTERNOON;
        else CurrentTimeOfDay = TIMEOFDAY.EVENING;

        UIController.Instance.UpdateClock(CurrentDayTime);

        if (PreviousTimeOfDay != CurrentTimeOfDay)
        {
            PreviousTimeOfDay = CurrentTimeOfDay;
            if (TimeOfDayChange != null) TimeOfDayChange.Invoke();
        }

        //Set Set Sun and sky values
        float Angle = Mathf.Lerp(-60, 60, CurrentDayTime);
        float Tilt = Mathf.Lerp(5, 45, Mathf.Sin(CurrentDayTime * Mathf.PI) );

        transform.rotation = Quaternion.Euler(0, Angle, 0f);
        Sun.transform.localRotation = Quaternion.Euler(Tilt, 0f, 0f);

        Skybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(0.5f, 1.5f, CurrentDayTime) );
    }
}
