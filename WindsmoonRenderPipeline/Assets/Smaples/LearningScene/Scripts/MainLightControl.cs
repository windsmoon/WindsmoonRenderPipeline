using System;
using UnityEngine;

namespace WindsmoonRP.Samples.LearningScene
{
    public class MainLightControl : MonoBehaviour
    {
        #region unity methods
        private void Update()
        {
            transform.Rotate(0, Time.deltaTime * 36, 0, Space.World);
        }
        #endregion
    }
}