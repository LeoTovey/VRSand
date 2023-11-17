using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public int textureWidth = 2048;
    public int textureHeight = 1024;
    public string savePath = "Assets/_Dev/";

    private void Start()
    {
        for (float threshold = 0.0f; threshold < 1.0f; threshold += 0.1f)
        {
            // ����һ���µ�����
            Texture2D noiseTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.R16, false);

            // ���ɰ���������
            Color[] noisePixels = new Color[textureWidth * textureHeight];
            for (int i = 0; i < noisePixels.Length; i++)
            {
                float randomValue = Random.Range(0.0f, 1.0f);
                if (randomValue > threshold)
                {
                    noisePixels[i] = new Color(1.0f, 1.0f, 1.0f);
                }
                else
                {
                    noisePixels[i] = new Color(0.0f, 0.0f, 0.0f);
                }

            }

            // ������������
            noiseTexture.SetPixels(noisePixels);
            noiseTexture.Apply();

            // ��������ΪAsset
            byte[] bytes = noiseTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(savePath + "Noise_" + threshold + "_T.png", bytes);
        }





    }
}

