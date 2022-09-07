using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGifPlayer : MonoBehaviour
{
    public RawImage img;
    public string gifPath;
    public static MyGifPlayer Instance { get; private set; }
    public RenderTexture rt;

    private void Awake()
    {
        Instance = this;    
    }

    void Start()
    {
#if UNITY_WEBGL
        ProGifManager.Instance.SetAdvancedPlayerDecodeSettings(ProGifPlayerComponent.Decoder.ProGif_Coroutines);
#endif
        ProGifManager.Instance.PlayGif(gifPath, img, (progress) =>
        {
            if (progress >= 1)
                img.texture = rt;
        }, shouldSaveFromWeb: false);
    }
}
