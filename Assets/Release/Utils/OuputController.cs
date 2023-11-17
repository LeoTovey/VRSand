using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OuputController : MonoBehaviour
{
    public RenderTexture renderTexture;
    public Hand LeftHand;

    private int test = 0;


    void Start()
    {
    }
    private void Update()
    {
        if (LeftHand.CurrentHandPose == HandPose.ToolHolding)
        {
            // 保存Render Texture中的内容为纹理
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();


            string fileName = "OutputImage_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";

            // 保存为JPG文件
            byte[] bytes = texture.EncodeToJPG();
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName), bytes);
            test = 1;
        }
    }


}
