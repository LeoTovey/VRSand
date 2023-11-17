using System.IO;
using UnityEngine;

public class RenderAndExport : MonoBehaviour
{
    public Camera renderCamera; // 你的摄像机
    public string filename;
    public int width = 1920; // 设置宽度
    public int height = 1080; // 设置高度

    void Start()
    {
        if (renderCamera != null)
        {
            renderCamera.targetTexture = new RenderTexture(width, height, 24);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExportRenderResult();
        }
    }

    public void ExportRenderResult()
    {
        Debug.Log(renderCamera);
        if (renderCamera != null && renderCamera.targetTexture != null)
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = renderCamera.targetTexture;

            Texture2D renderResult = new Texture2D(width, height, TextureFormat.RGB24, false);
            renderResult.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            renderResult.Apply();

            RenderTexture.active = currentRT;

            byte[] bytes = renderResult.EncodeToPNG();
            // 保存到持久化路径
            string filePath = "C:/Users/11937/Desktop/" + filename + ".png";
            File.WriteAllBytes(filePath, bytes);
  
            // 检查文件是否保存成功
            if (File.Exists(filePath))
            {
                Debug.Log("Render result saved to: " + filePath);
            }
            else
            {
                Debug.LogError("Failed to save render result");
            }


        }
    }
}
