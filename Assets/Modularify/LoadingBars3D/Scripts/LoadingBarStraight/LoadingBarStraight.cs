using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modularify.LoadingBars3D
{
    public class LoadingBarStraight : MonoBehaviour
    {
        #region variables
        [Header("Inner Part Reference")]
        [SerializeField]
        private GameObject _innerPart;

        private Material _innerPartMaterial;

        [Header("Loading Bar Settings and Colors")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _percentage = 0.5f;

        [ColorUsage(true, true)]
        [SerializeField]
        private Color _emptyColor = new Color(1, 0, 0, 1);

        [ColorUsage(true, true)]
        [SerializeField]
        private Color _fullColor = new Color(0, 1, 0, 1);
        #endregion

        #region methods

        private void Awake()
        {
            Initialize();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            _innerPartMaterial.SetFloat("Percentage", _percentage);
        }

        /// <summary>
        /// Sets the percentage of the progress bar
        /// </summary>
        /// <param name="percentage">float value clamped between 0 and 1</param>
        public void SetPercentage(float percentage)
        {
            _percentage = Mathf.Clamp01(percentage);
        }

        public void Initialize()
        {
            _innerPartMaterial = new Material(_innerPart.GetComponent<MeshRenderer>().sharedMaterial);
            _innerPart.GetComponent<MeshRenderer>().material = _innerPartMaterial;
            _innerPartMaterial.SetFloat("Percentage", _percentage);
            _innerPartMaterial.SetColor("SideColorEmpty", _emptyColor);
            _innerPartMaterial.SetColor("SideColorFull", _fullColor);
        }
        #endregion
    }
}

