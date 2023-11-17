using UnityEngine;
using UnityEditor;


public class GenerateCube : MonoBehaviour
{
    public GameObject squareCylinderPrefab;
    public Color color1 = new Color(223f / 255f, 244f / 255f, 190f / 255f); // 第一种颜色
    public Color color2 = new Color(186f / 255f, 207f / 255f, 227f / 255f); // 第二种颜色
    public int numberOfCylinders = 10; 
    public int HeightMapWidth = 100;
    public int HeightMapHeight = 100;
    public Vector2 PlaneSize;

    public float heightMultiplier = 5.0f; 
    public float spacing = 1.0f;

    /*
    void Start()
    {
        GenerateCylindersFromHeightMap();
    }

    void GenerateCylindersFromHeightMap()
    {
        int width = HeightMapWidth;
        int height = HeightMapHeight;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Color pixelColor = heightMap.GetPixel(x, y);
                //float heightValue = pixelColor.grayscale * heightMultiplier; // 从纹理图获取高度值
                float heightValue = 1.0f;
                Vector3 position = new Vector3(x * spacing, heightValue / 2, y * spacing); // 根据高度值计算柱体位置

                GameObject cylinder = Instantiate(squareCylinderPrefab, position, Quaternion.identity); // 生成柱体

                cylinder.transform.localScale = new Vector3(1, heightValue, 1); // 设置柱体高度
            }
        }
    }
    */
}
