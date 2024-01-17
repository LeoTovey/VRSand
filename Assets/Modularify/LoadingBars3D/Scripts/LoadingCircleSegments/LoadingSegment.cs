using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modularify.LoadingBars3D
{
    public class LoadingSegment : MonoBehaviour
    {
        #region variables
        private AnimationCurve _animationCurve;
        private IEnumerator _animationCoroutine;

        [SerializeField]
        private Material _innerMaterialRef;

        [SerializeField]
        private Material _outerMaterialRef;

        private Material _ownInnerMat;
        private Material _ownOuterMat;

        private float _currentPercentage = 0;
        private float _previousPercentage = 0;
        #endregion

        #region methods
        private void Awake()
        {
            //Initialize();
        }

        public void Initialize(Color innerColor, Color outerColor)
        {

            _animationCurve = transform.parent.GetComponent<LoadingBarSegments>().GetAnimationCurve();
            _animationCoroutine = ScalingAnimationCoroutine();

            _ownInnerMat = new Material(_innerMaterialRef);
            _ownOuterMat = new Material(_outerMaterialRef);

            _ownInnerMat.SetColor("BaseColor", innerColor);
            _ownOuterMat.SetColor("BaseColor", outerColor);

            _ownInnerMat.SetFloat("Percentage", 1);
            _ownOuterMat.SetFloat("Percentage", 1);

            Material[] tempMats = { _ownInnerMat, _ownOuterMat };

            GetComponent<MeshRenderer>().sharedMaterials = tempMats;


            if (Application.isPlaying)
            {
                _ownInnerMat = GetComponent<MeshRenderer>().materials[0];
                _ownOuterMat = GetComponent<MeshRenderer>().materials[1];
            }

        }

        public void SetPartPercentage(float percentage)
        {
            _currentPercentage = percentage;

            if (Application.isPlaying)
            {
                _ownInnerMat.SetFloat("Percentage", _currentPercentage);
                _ownOuterMat.SetFloat("Percentage", _currentPercentage);
                if (_currentPercentage != _previousPercentage && _currentPercentage == 1)
                {
                    //Play animation
                    _animationCoroutine = ScalingAnimationCoroutine();
                    StartCoroutine(_animationCoroutine);

                }
            }

            else
            {
                _ownInnerMat.SetFloat("Percentage", _currentPercentage);
                _ownOuterMat.SetFloat("Percentage", _currentPercentage);

                Material[] tempMats = { _ownInnerMat, _ownOuterMat };

                GetComponent<MeshRenderer>().sharedMaterials = tempMats;
            }


            _previousPercentage = _currentPercentage;
        }

        private IEnumerator ScalingAnimationCoroutine()
        {
            float startingTime = Time.time;
            while (Time.time - startingTime < 1)
            {
                float percentage = Time.time - startingTime;
                float curveValueAtPercentage = _animationCurve.Evaluate(percentage);
                transform.localScale = new Vector3(curveValueAtPercentage, curveValueAtPercentage, curveValueAtPercentage);
                yield return new WaitForEndOfFrame();
            }
            transform.localScale = Vector3.one;
        }
        #endregion
    }
}

