using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static TMPro.Examples.TMP_ExampleScript_01;



public class AIUIController : MonoBehaviour
{
    [Serializable]
    public struct AITipPrefabPair
    {
        public HandPose aiTipType;
        public GameObject aiTipPrefab;
    }

    [SerializeField] private Transform _aiTipsParent;
    public List<AITipPrefabPair> prefabList = new List<AITipPrefabPair>();
    private Dictionary<HandPose, GameObject> _prefabRepo = new Dictionary<HandPose, GameObject>();
    private Dictionary<HandPose, AiTip> _currentPrefabs = new Dictionary<HandPose, AiTip>();
    private Dictionary<HandPose, bool> _finished = new Dictionary<HandPose, bool>();
    [SerializeField] private HandController _hands;

    private bool _enabled = true;

    void Start()
    {
        foreach (var pair in prefabList)
        {
            _prefabRepo[pair.aiTipType] = pair.aiTipPrefab;
        }

        foreach (HandPose pose in Enum.GetValues(typeof(HandPose)))
        {
            _finished[pose] = false;
        }
    }

    void Update()
    {


        if (_enabled)
        {
            if (!_finished[HandPose.ScatterPouring])
            {
                UpdatePrefab(HandPose.ScatterPouring, _hands._poseCounter[HandPose.ScatterPouring]);

                if (_currentPrefabs[HandPose.ScatterPouring].Finished)
                {
                    DeletePrefab(HandPose.ScatterPouring);
                }
            }

            if (!_finished[HandPose.SkinnyPouring])
            {
                UpdatePrefab(HandPose.SkinnyPouring, _hands._poseCounter[HandPose.SkinnyPouring]);

                if (_currentPrefabs[HandPose.SkinnyPouring].Finished)
                {
                    DeletePrefab(HandPose.SkinnyPouring);
                }
            }

            if (_finished[HandPose.ScatterPouring] && _finished[HandPose.SkinnyPouring])
            {

                if (!_finished[HandPose.PalmRubbing])
                {
                    UpdatePrefab(HandPose.PalmRubbing, _hands._poseCounter[HandPose.PalmRubbing]);

                    if (_currentPrefabs[HandPose.PalmRubbing].Finished)
                    {
                        DeletePrefab(HandPose.PalmRubbing);
                    }
                }

                if (!_finished[HandPose.HandSweeping])
                {
                    UpdatePrefab(HandPose.HandSweeping, _hands._poseCounter[HandPose.HandSweeping]);

                    if (_currentPrefabs[HandPose.HandSweeping].Finished)
                    {
                        DeletePrefab(HandPose.HandSweeping);
                    }
                }


                if (_finished[HandPose.PalmRubbing] && _finished[HandPose.HandSweeping])
                {
                    if (!_finished[HandPose.ToolHolding])
                    {
                        UpdatePrefab(HandPose.ToolHolding, _hands._poseCounter[HandPose.ToolHolding]);

                        if (_currentPrefabs[HandPose.ToolHolding].Finished)
                        {
                            DeletePrefab(HandPose.ToolHolding);
                        }
                    }

                    if (!_finished[HandPose.FingertipTracing])
                    {
                        UpdatePrefab(HandPose.FingertipTracing, _hands._poseCounter[HandPose.FingertipTracing]);

                        if (_currentPrefabs[HandPose.FingertipTracing].Finished)
                        {
                            DeletePrefab(HandPose.FingertipTracing);
                        }
                    }


                    if (!_finished[HandPose.FingerCarving])
                    {
                        UpdatePrefab(HandPose.FingerCarving, _hands._poseCounter[HandPose.FingerCarving]);

                        if (_currentPrefabs[HandPose.FingerCarving].Finished)
                        {
                            DeletePrefab(HandPose.FingerCarving);
                        }
                    }
                }


            }

        }

    }



    public void DeletePrefab(HandPose objectType)
    {
        if (_currentPrefabs.ContainsKey(objectType))
        {
            AiTip existingPrefab = _currentPrefabs[objectType];
            _finished[objectType] = true;
            _currentPrefabs.Remove(objectType);
            //existingPrefab.PlayFloatAnimation();
            StartCoroutine(DestroyPrefabWithDelay(existingPrefab.gameObject, 2.0f));

        }
    }

    private IEnumerator DestroyPrefabWithDelay(GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(prefab);
    }

    public void UpdatePrefab(HandPose objectType, float _test)
    {
        if (_currentPrefabs.ContainsKey(objectType))
        {
            AiTip tip = _currentPrefabs[objectType];

            if (tip != null)
            {
                Debug.Log("Prefab with ObjectType " + objectType + " already exists.");
                tip.gameObject.transform.SetAsFirstSibling();
                tip.SetTime(_test);
                return;
            }
        }

        // 生成新的Prefab
        GameObject newPrefabInstance = Instantiate(_prefabRepo[objectType], _aiTipsParent);
        _currentPrefabs[objectType] = newPrefabInstance.GetComponent<AiTip>();
        _currentPrefabs[objectType].SetTime(0.0f);
    }




}
