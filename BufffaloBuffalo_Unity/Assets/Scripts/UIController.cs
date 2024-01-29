using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private static UIController instance;
    public static UIController Instance
    {
        get
        {
            instance = FindObjectOfType<UIController>();
            return instance;
        }
    }

    public Image TimeDisplay;

    public Image DominanceBarRed;
    public Image DominanceBarBrown;
    public Image DominanceBarBlue;
    public Image DominanceBarGreen;
    public Image DominanceBarYellow;

    public int RedCount;
    public int BrownCount;
    public int BlueCount;
    public int GreenCount;
    public int YellowCount;

    public void UpdateClock(float SetTime)
    {
        TimeDisplay.fillAmount = SetTime;
    }

    public static void UpdateSpawnCount(UnitColor _Color, bool _IsDead)
    {
        int CountChange = (_IsDead) ? -1 : 1;

        switch (_Color)
        {
            case UnitColor.RED:
                Instance.RedCount += CountChange;
                break;

            case UnitColor.BROWN:
                Instance.BrownCount += CountChange;
                break;

            case UnitColor.BLUE:
                Instance.BlueCount += CountChange;
                break;

            case UnitColor.GREEN:
                Instance.GreenCount += CountChange;
                break;

            case UnitColor.YELLOW:
                Instance.YellowCount += CountChange;
                break;
        }

        Instance.UpdateDominanceBars();
    }

    void UpdateDominanceBars()
    {
        float TotalCount = RedCount + BrownCount + BlueCount + GreenCount + YellowCount;

        DominanceBarRed.fillAmount = RedCount / TotalCount;
        DominanceBarBrown.fillAmount = BrownCount / TotalCount;
        DominanceBarBlue.fillAmount = BlueCount / TotalCount;
        DominanceBarGreen.fillAmount = GreenCount / TotalCount;
        DominanceBarYellow.fillAmount = YellowCount / TotalCount;

    }
}
