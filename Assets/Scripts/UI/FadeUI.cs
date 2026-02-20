using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : UIBase
{
    public static string identifier = "FadeUI";
    [SerializeField] float fadeTime;
    [SerializeField] Image fadeImage;

    public override void Initialization(UIData _data)
    {
        FadeUIData t_fadeUIData = _data as FadeUIData;
        if (t_fadeUIData == null)
        {
            Debug.Log("Invalid DataType in FadeUI");
            return;
        }
        this.data = t_fadeUIData;
        StartCoroutine(StartFade());
    }

    public override void Show()
    {
        this.isShow = true;
        this.contents.SetActive(true);
    }

    public override void Hide()
    {
        this.isShow = false;
        this.contents.SetActive(false);
        this.data.onHide?.Invoke();
    }


    IEnumerator StartFade()
    {
        var t_data = this.data as FadeUIData;
        float t_time = 0f;
        Color t_color = fadeImage.color;

        if (t_data.fadeIn)
        {
            t_color.a = 0f;
            fadeImage.color = t_color;
            while (t_time < fadeTime)
            {
                t_time += Time.deltaTime;
                float t_alpha = Mathf.Clamp01(t_time / fadeTime);
                t_color.a = t_alpha;
                fadeImage.color = t_color;
                yield return null;
            }
        }
        if (t_data.fadeOut)
        {
            t_time = 0f;
            t_color.a = 1f;
            fadeImage.color = t_color;
            while (t_time < fadeTime)
            {
                t_time += Time.deltaTime;
                float t_alpha = 1f - Mathf.Clamp01(t_time / fadeTime);
                t_color.a = t_alpha;
                fadeImage.color = t_color;
                yield return null;
            }
        }

        Hide();
    }
}

public class FadeUIData : UIData
{
    public bool fadeIn;
    public bool fadeOut;
}