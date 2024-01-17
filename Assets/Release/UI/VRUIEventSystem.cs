using CurvedUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class VRUIEventSystem : MonoBehaviour
{
    public CurvedUISettings CurvedLeftUISettings;
    public Canvas LeftUICanvas;

    public Transform TestPoint;
    //public TextMeshProUGUI UITextMeshPro;
    public RectTransform CursorTransform;
   

    GameObject previousGameObject = null;

    
    public void ProcessPointEvent(Vector3 point, Vector3 fingerVelocity)
    {
        Ray pointRay = new Ray();
        float distance = 10.0f;
        GameObject hittedObject = null;
        Vector2 pointOnCanvas; // world space
        if (CurvedLeftUISettings.PointToCylinderRadius(point, out distance, out pointRay, out pointOnCanvas))
        {
            CursorTransform.gameObject.SetActive(true);
            CursorTransform.anchoredPosition = pointOnCanvas - (Vector2)LeftUICanvas.transform.position;
            //UITextMeshPro.text = "";
            foreach (GameObject currentGameObject in CurvedLeftUISettings.GetObjectsHitByRay(pointRay))
            {
                hittedObject = currentGameObject;
                //UITextMeshPro.text += currentGameObject.name + " " + distance;
                if (distance < 0.01f && Vector3.Dot(fingerVelocity, pointRay.direction) > 0.0f)
                {
                    IPointerClickHandler clickHandler = currentGameObject.GetComponent<IPointerClickHandler>();
                    if (clickHandler != null)
                    {
                        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                        pointerEventData.position = pointOnCanvas;
                        //UITextMeshPro.text += " "+ pointerEventData.position;
                        clickHandler.OnPointerClick(pointerEventData);
                    }
                }               
            }

            if (previousGameObject != hittedObject)
            {
                if (hittedObject != null)
                {
                    //UITextMeshPro.text += "enter != null";
                    IPointerEnterHandler enterHandler = hittedObject.GetComponent<IPointerEnterHandler>();
                    if (enterHandler != null)
                    {
                        enterHandler.OnPointerEnter(null);
                    }
                }

                if (previousGameObject != null)
                {
                    IPointerExitHandler exitHandler = previousGameObject.GetComponent<IPointerExitHandler>();
                    if (exitHandler != null)
                    {
                        exitHandler.OnPointerExit(null);
                    }
                }
            }
            previousGameObject = hittedObject;
        }
        else
        {
            CursorTransform.gameObject.SetActive(false);
        }

    }
}
