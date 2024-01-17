using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modularify.LoadingBars3D
{
    /// <summary>
    /// Class responsible for managing the LoadingBarParts gameobject
    /// </summary>
    public class LoadingBarParts : MonoBehaviour
    {
        #region Variables
        [Header("Inner Part Reference")]
        [SerializeField]
        private GameObject _loadingPartRef;

        [Header("Loading Bar Settings")]
        [SerializeField]
        private int _numberOfParts;

        [SerializeField]
        private float _distanceBetweenParts = 0.5f;

        [SerializeField]
        private BarColoringMode _barColoringMode = BarColoringMode.Single;

        [SerializeField]
        private OuterPartVisibility _outerPartEmptyVisibility = OuterPartVisibility.Hidden;

        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _percentage = 0.5f;

        [SerializeField]
        private AnimationCurve _animationCurve = AnimationCurve.EaseInOut(0, 0, 1.0f, 1);

        private List<GameObject> _loadingParts;

        [Header("Inner Colors")]
        [ColorUsage(true, true)]
        [SerializeField]
        private Color _innerColorEmpty = new Color(1, 0, 0, 1);

        [ColorUsage(true, true)]
        [SerializeField]
        private Color _innerColorFull = new Color(0, 1, 0, 1);

        [Header("Outer Color")]
        [ColorUsage(true, true)]
        [SerializeField]
        private Color _outerColor = new Color(0, 1, 0, 1);

        [SerializeField]
        private bool _highlightOuterPartWithPercentage = false;
        #endregion

        #region methods
        private void Awake()
        {
            _loadingParts = new List<GameObject>();
        }

        void Start()
        {
            Initialize();
        }

        void Update()
        {
            SetPercentage(_percentage);
        }

        /// <summary>
        /// Initializes the loading bar parts destroying and re-instantiating them with the correct values
        /// </summary>
        public void Initialize()
        {
            if (_numberOfParts < 1)
            {
                _numberOfParts = 1;
            }
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            _loadingParts = new List<GameObject>();

            for (int i = 0; i < _numberOfParts; i++)
            {
                Vector3 newPosition = new Vector3(transform.position.x + i * _distanceBetweenParts, transform.position.y, transform.position.z);
                GameObject part = GameObject.Instantiate(_loadingPartRef, newPosition, Quaternion.Euler(Vector3.zero), transform);
                _loadingParts.Add(part);
                part.GetComponent<LoadingPart>().Initialize(_innerColorEmpty, _innerColorFull, _outerColor, _highlightOuterPartWithPercentage);

            }

            SetPercentage(_percentage);

        }

        /// <summary>
        /// Activates only those parts which are under the given percentage as a parameter
        /// </summary>
        /// <param name="newPercentage"> Current percentage of the loading circle </param>
        public void SetPercentage(float newPercentage)
        {
            if (_numberOfParts != transform.childCount)
            {
                Initialize();
            }
            else
            {
                Mathf.Clamp01(newPercentage);
                _percentage = newPercentage;
                for (int i = 0; i < _numberOfParts; i++)
                {
                    if (i < _numberOfParts * _percentage)
                    {
                        _loadingParts[i].SetActive(true);
                        if (i < Mathf.Floor(_numberOfParts * _percentage))
                        {
                            if (_barColoringMode == BarColoringMode.Single)
                            {
                                _loadingParts[i].GetComponent<LoadingPart>().SetPartPercentage(1, _percentage, _barColoringMode);
                            }
                            else
                            {
                                _loadingParts[i].GetComponent<LoadingPart>().SetPartPercentage(1, _percentage, _barColoringMode);
                            }

                        }
                        else
                        {
                            _loadingParts[i].GetComponent<LoadingPart>().SetPartPercentage((_numberOfParts * _percentage) % 1, _percentage, _barColoringMode);
                        }
                    }
                    else
                    {
                        _loadingParts[i].GetComponent<LoadingPart>().SetPartPercentage(0, _percentage, _barColoringMode);

                        if (_outerPartEmptyVisibility == OuterPartVisibility.Hidden)
                        {
                            _loadingParts[i].SetActive(false);
                        }

                    }
                }
            }

        }

        public AnimationCurve GetAnimationCurve()
        {
            return _animationCurve;
        }
        #endregion
    }

    #region custom enums
    public enum BarColoringMode
    {
        Single,
        Whole
    }

    public enum OuterPartVisibility
    {
        Visible,
        Hidden
    }
    #endregion
}

