using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [System.Serializable]
    public class SpawnData
    {
        public string UnitLabel;
        public GameObject UnitPrefab;
        public Transform[] SpawnPoints;
        public float SpawnRate;

        public DaytimeModifiers Morning;
        public DaytimeModifiers Afternoon;
        public DaytimeModifiers Evening;

        internal DaytimeModifiers CurrentSpawnModifier;

        public float GetSpawnRate()
        {
            return SpawnRate + CurrentSpawnModifier.ActiveSpawnModifier;
        }
    }

    [System.Serializable]
    public class DaytimeModifiers
    {
        public bool CanSpawn;
        public Vector2 SpawnRate;
        internal float ActiveSpawnModifier;
    }


    public float SpawnRateModifier = 5f;
    public SpawnData[] UnitSpawnData;

    private void Awake()
    {
        DayCycleController.Instance.TimeOfDayChange.AddListener(OnDayTimeChange);
    }

    void Start()
    {
        // start the spawn sequences based on spawn rates
        for (int i = 0; i < UnitSpawnData.Length; i++)
        {
            StartCoroutine(SpawnRoutine(UnitSpawnData[i]));
        }
    }

    IEnumerator SpawnRoutine(SpawnData _SpawnData)
    {
        yield return null;

        while (true)
        {
            yield return new WaitForSeconds(SpawnRateModifier - _SpawnData.GetSpawnRate());

            if (_SpawnData.CurrentSpawnModifier.CanSpawn)
            {
                Vector3 SpawnOffset = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
                Transform SpawnPoint = _SpawnData.SpawnPoints[Random.Range(0, _SpawnData.SpawnPoints.Length)];

                Instantiate(_SpawnData.UnitPrefab.gameObject, SpawnPoint.position + SpawnOffset, Quaternion.identity, transform);
            }
        }
    }

    public void OnDayTimeChange()
    {
        for (int i = 0; i < UnitSpawnData.Length; i++)
        {
            switch (DayCycleController.Instance.CurrentTimeOfDay)
            {
                case TIMEOFDAY.NONE:
                    break;

                case TIMEOFDAY.MORNING:
                    UnitSpawnData[i].CurrentSpawnModifier = UnitSpawnData[i].Morning;
                    break;

                case TIMEOFDAY.AFTERNOON:
                    UnitSpawnData[i].CurrentSpawnModifier = UnitSpawnData[i].Afternoon;
                    break;

                case TIMEOFDAY.EVENING:
                    UnitSpawnData[i].CurrentSpawnModifier = UnitSpawnData[i].Evening;
                    break;
            }


            UnitSpawnData[i].CurrentSpawnModifier.ActiveSpawnModifier = Random.Range(UnitSpawnData[i].CurrentSpawnModifier.SpawnRate.x, UnitSpawnData[i].CurrentSpawnModifier.SpawnRate.y);
        }
    }
}
