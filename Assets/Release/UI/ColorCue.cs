using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorCue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Color Cue Slider
    public RawImage ColorCueBackgroudImage;
    public RawImage ColorCueHandleImage;

    public RectTransform ColorCueTransform;
    public RectTransform ColorCueHandleTransform;

    public ColorBoard MyColorBoard;


    Texture2D colorCueTex;
    int colorCueTexPixelWidth = 952;
    int colorCueTexPixelHeight = 16;
    Color[,] colorCueArrayColor;

    Color[] CalcColorCueArrayColor()
    {
        int addValue = (colorCueTexPixelWidth - 1) / 3;

        for (int i = 0; i < colorCueTexPixelHeight; i++)
        {
            colorCueArrayColor[0, i] = UnityEngine.Color.red;
            colorCueArrayColor[addValue, i] = UnityEngine.Color.green;
            colorCueArrayColor[addValue + addValue, i] = UnityEngine.Color.blue;
            colorCueArrayColor[colorCueTexPixelHeight - 1, i] = UnityEngine.Color.red;
        }
        UnityEngine.Color value = (UnityEngine.Color.green - UnityEngine.Color.red) / addValue;
        for (int i = 0; i < colorCueTexPixelHeight; i++)
        {
            for (int j = 0; j < addValue; j++)
            {
                colorCueArrayColor[j, i] = UnityEngine.Color.red + value * j;
            }
        }

        value = (UnityEngine.Color.blue - UnityEngine.Color.green) / addValue;
        for (int i = 0; i < colorCueTexPixelHeight; i++)
        {
            for (int j = addValue; j < addValue * 2; j++)
            {
                colorCueArrayColor[j, i] = UnityEngine.Color.green + value * (j - addValue);
            }
        }

        value = (UnityEngine.Color.red - UnityEngine.Color.blue) / ((colorCueTexPixelWidth - 1) - addValue - addValue);
        for (int i = 0; i < colorCueTexPixelHeight; i++)
        {
            for (int j = addValue * 2; j < colorCueTexPixelWidth - 1; j++)
            {
                colorCueArrayColor[j, i] = UnityEngine.Color.blue + value * (j - addValue * 2);
            }
        }

        List<UnityEngine.Color> listColor = new List<UnityEngine.Color>();
        for (int i = 0; i < colorCueTexPixelHeight; i++)
        {
            for (int j = 0; j < colorCueTexPixelWidth; j++)
            {
                listColor.Add(colorCueArrayColor[j, i]);
            }
        }

        return listColor.ToArray();
    }


    public void ColorCueChanged(float i)
    {
        float clampValue = Mathf.Clamp(i, 0.001f, 0.999f);
        Color getColor = colorCueTex.GetPixel((int)((colorCueTexPixelWidth - 1) * clampValue), 0);
        ColorCueHandleImage.color = getColor;
        MyColorBoard.SetColorBoard(getColor);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        colorCueArrayColor = new Color[colorCueTexPixelWidth, colorCueTexPixelHeight];
        colorCueTex = new Texture2D(colorCueTexPixelWidth, colorCueTexPixelHeight, TextureFormat.RGB24, true);
        Color[] calcArray = CalcColorCueArrayColor();
        colorCueTex.SetPixels(calcArray);
        colorCueTex.Apply();
        ColorCueBackgroudImage.texture = colorCueTex;
        ColorCueBackgroudImage.texture.wrapMode = TextureWrapMode.Clamp;

        ColorCueChanged(0.0f);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 newPosition = 0.5f * ColorCueTransform.rect.size + (eventData.position - (Vector2)ColorCueTransform.position);
        newPosition.y = ColorCueHandleTransform.anchoredPosition.y;
        ColorCueHandleTransform.anchoredPosition = newPosition;
        ColorCueChanged(newPosition.x / ColorCueTransform.rect.width);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

}
