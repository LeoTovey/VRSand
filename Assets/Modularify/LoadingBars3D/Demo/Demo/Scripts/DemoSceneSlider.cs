using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Modularify.LoadingBars3D;

namespace Modularify.DemoScripts
{
    public class DemoSceneSlider : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private LoadingBarSegments _loadingSegments;
        [SerializeField]
        private LoadingBarStraight _loadingBarStraight;
        [SerializeField]
        private LoadingBarParts _loadingBarParts;

        public void UpdateLoadingBarsPercentage()
        {
            _loadingSegments.SetPercentage(_slider.value);
            _loadingBarStraight.SetPercentage(_slider.value);
            _loadingBarParts.SetPercentage(_slider.value);
        }
    }
}

