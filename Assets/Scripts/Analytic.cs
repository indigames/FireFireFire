using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analytic : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        GameAnalyticsSDK.GameAnalytics.Initialize();
    }
}
