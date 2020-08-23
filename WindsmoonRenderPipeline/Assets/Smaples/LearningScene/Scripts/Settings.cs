using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    #region fields
    [SerializeField]
    private float crossFadeTime = 0.5f;
    #endregion

    #region unity methods
    private void OnValidate()
    {
        LODGroup.crossFadeAnimationDuration = crossFadeTime;
    }
    #endregion
}
