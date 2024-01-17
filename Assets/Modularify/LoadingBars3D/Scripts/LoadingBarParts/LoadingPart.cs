using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Modularify.LoadingBars3D
{
    /// <summary>
    /// Class responsible for handling the inner parts of the LoadingBarParts game object
    /// </summary>
    public class LoadingPart : MonoBehaviour
    {
        #region variables
        private AnimationCurve _animationCurve;
        private IEnumerator _animationCoroutine;

        [SerializeField]
        private Material _innerMaterialRef;

        [SerializeField]
        private Material _outerMaterialRef;

        [SerializeField]
        private MeshRenderer _innerPartMeshRenderer;

        [SerializeField]
        private MeshRenderer _outerPartMeshRenderer;

        private Material _ownInnerMat;
        private Material _ownOuterMat;

        private float _currentPercentage = 0;
        private float _previousPercentage = 0;
        #endregion

        #region methods
        /// <summary>
        /// Method called from the LoadingBarParts script in order to update the inner parts values 
        /// </summary>
        /// <param name="innerColorEmpty">The color of the inner part when empty</param>
        /// <param name="innerColorFull">The color of the inner part when full</param>
        /// <param name="outerColor">The color of the outer part</param>
        /// <param name="highlightOuterPartWithPercentage">Whether or not the outer part should highlight based on the percentage of the bar</param>
        public void Initialize(Color innerColorEmpty, Color innerColorFull, Color outerColor, bool highlightOuterPartWithPercentage)
        {

            _animationCurve = transform.parent.GetComponent<LoadingBarParts>().GetAnimationCurve();
            _animationCoroutine = ScalingAnimationCoroutine();

            _ownInnerMat = new Material(_innerMaterialRef);
            _ownOuterMat = new Material(_outerMaterialRef);

            _ownInnerMat.SetColor("EmptyColor", innerColorEmpty);
            _ownInnerMat.SetColor("FullColor", innerColorFull);
            _ownOuterMat.SetColor("BaseColor", outerColor);

            _ownInnerMat.SetFloat("Percentage", 1);
            _ownOuterMat.SetFloat("Percentage", 1);

            _ownOuterMat.SetFloat("HighlightWIthPercentage", highlightOuterPartWithPercentage ? 1 : 0);

            _innerPartMeshRenderer.material = _ownInnerMat;
            _outerPartMeshRenderer.material = _ownOuterMat;


        }

        /// <summary>
        /// Sets the part fullness and color. The color is based on the BarColoringMode value passed as parameter
        /// </summary>
        /// <param name="_innerPercentage"></param>
        /// <param name="_totalPercentage"></param>
        /// <param name="barColoringMode"></param>
        public void SetPartPercentage(float _innerPercentage, float _totalPercentage, BarColoringMode barColoringMode)
        {

            _currentPercentage = _innerPercentage;

            if (Application.isPlaying)
            {
                if (_currentPercentage != _previousPercentage && _currentPercentage == 1)
                {
                    //Play animation
                    _animationCoroutine = ScalingAnimationCoroutine();
                    StartCoroutine(_animationCoroutine);
                }
            }

            if (barColoringMode == BarColoringMode.Single)
            {
                _ownInnerMat.SetFloat("ColorPercentage", _currentPercentage);
            }
            else
            {
                _ownInnerMat.SetFloat("ColorPercentage", _totalPercentage);
            }

            _ownInnerMat.SetFloat("Percentage", _currentPercentage);
            _ownOuterMat.SetFloat("Percentage", _totalPercentage);

            _previousPercentage = _currentPercentage;

            if (_innerPercentage < 0.01f)
            {
                _innerPartMeshRenderer.enabled = false;
            }
            else
            {
                _innerPartMeshRenderer.enabled = true;
            }
        }

        /// <summary>
        /// Animation coroutine which is fired when this specific inner bar reaches 100%
        /// 
        /// Called in the LoadingBarParts script
        /// </summary>
        /// <returns></returns>
        private IEnumerator ScalingAnimationCoroutine()
        {
            float startingTime = Time.time;
            while (Time.time - startingTime < 1)
            {
                float percentage = Time.time - startingTime;
                float curveValueAtPercentage = _animationCurve.Evaluate(percentage);
                transform.localScale = new Vector3(1.0f, curveValueAtPercentage, curveValueAtPercentage);
                yield return new WaitForEndOfFrame();
            }
            transform.localScale = Vector3.one;
        }
        #endregion
    }
}

