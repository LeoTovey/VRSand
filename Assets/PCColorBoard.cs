using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PCColorBoard : MonoBehaviour, IPointerClickHandler
{
    // Color Board
    Texture2D ColorBoardTex2d;
    public RawImage ColorBoardImage;
    public RawImage ColorHandleImage;
    public RectTransform ColorBoardTransform;
    public RectTransform ColorHandleTransform;
    Color[,] colorBoardArrayColor;
    int colorBoardTexPixelLength = 256;
    int colorBoardTexPixelHeight = 256;

    void Awake()
    {
        colorBoardArrayColor = new Color[colorBoardTexPixelLength, colorBoardTexPixelHeight];
        ColorBoardTex2d = new Texture2D(colorBoardTexPixelLength, colorBoardTexPixelHeight, TextureFormat.RGB24, true);
        ColorBoardImage.texture = ColorBoardTex2d;
        ColorBoardImage.texture.wrapMode = TextureWrapMode.Clamp;

        SetColorBoard(Color.green);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Color[] CalcColorBoardArrayColor(Color endColor)
    {
        UnityEngine.Color value = (endColor - UnityEngine.Color.white) / (colorBoardTexPixelLength - 1);
        for (int i = 0; i < colorBoardTexPixelLength; i++)
        {
            colorBoardArrayColor[i, colorBoardTexPixelHeight - 1] = UnityEngine.Color.white + value * i;
        }

        for (int i = 0; i < colorBoardTexPixelLength; i++)
        {
            value = (colorBoardArrayColor[i, colorBoardTexPixelHeight - 1] - UnityEngine.Color.black) / (colorBoardTexPixelHeight - 1);
            for (int j = 0; j < colorBoardTexPixelHeight; j++)
            {
                colorBoardArrayColor[i, j] = UnityEngine.Color.black + value * j;
            }
        }

        List<UnityEngine.Color> listColor = new List<UnityEngine.Color>();
        for (int i = 0; i < colorBoardTexPixelHeight; i++)
        {
            for (int j = 0; j < colorBoardTexPixelLength; j++)
            {
                listColor.Add(colorBoardArrayColor[j, i]);
            }
        }

        return listColor.ToArray();
    }

    public void SetColorBoard(Color endColor)
    {
        UnityEngine.Color[] CalcArray = CalcColorBoardArrayColor(endColor);
        ColorBoardTex2d.SetPixels(CalcArray);
        ColorBoardTex2d.Apply();
        Color color = GetColorByPosition(ColorHandleTransform.anchoredPosition);
        ColorHandleImage.color = color;
        Debug.Log(color);
    }

    public UnityEngine.Color GetColorByPosition(Vector2 pos)
    {
        Texture2D tempTex2d = (Texture2D)ColorBoardImage.texture;
        Color getColor = tempTex2d.GetPixel((int)(pos.x * colorBoardTexPixelLength / ColorBoardTransform.rect.width),
            (int)(pos.y * colorBoardTexPixelHeight / ColorBoardTransform.rect.height));
        return getColor;
    }

    public Vector2 GetClampPosition(Vector2 touchPos)
    {
        Vector2 vector2 = new Vector2(touchPos.x, touchPos.y);
        vector2.x = Mathf.Clamp(vector2.x, 0.001f, ColorBoardTransform.rect.width);
        vector2.y = Mathf.Clamp(vector2.y, 0.001f, ColorBoardTransform.rect.height);
        return vector2;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 worldPosition ;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(ColorBoardTransform, eventData.position, eventData.pressEventCamera, out worldPosition))
        {
            ColorHandleTransform.position = worldPosition;
        }
        
        ColorHandleTransform.anchoredPosition = GetClampPosition(ColorHandleTransform.anchoredPosition);
        Color color = GetColorByPosition(ColorHandleTransform.anchoredPosition);
        ColorHandleImage.color = color;
    }
}
