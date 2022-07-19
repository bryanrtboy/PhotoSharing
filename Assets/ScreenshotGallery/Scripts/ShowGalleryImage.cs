using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGalleryImage : MonoBehaviour
{
    public RawImage m_panel;

    float m_alpha = 0f;

    public void ShowImage(RawImage m_image)
    {
        StopAllCoroutines();
        m_alpha = 0f;
        m_panel.color = new Color(m_panel.color.r, m_panel.color.g, m_panel.color.b, m_alpha);
        m_panel.texture = m_image.texture;
        StartCoroutine(FadeIn());
    }

    public void FadeOutPanel()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        while (m_alpha < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            m_alpha = Mathf.Clamp01(m_alpha + Time.deltaTime / 1f);
            m_panel.color = new Color(m_panel.color.r, m_panel.color.g, m_panel.color.b, m_alpha);
        }
    }

    IEnumerator FadeOut()
    {
        m_alpha = m_panel.color.a;

        while (m_alpha > 0.0f)
        {
            yield return new WaitForEndOfFrame();
            m_alpha = Mathf.Clamp01(m_alpha - Time.deltaTime / 1f);
            m_panel.color = new Color(m_panel.color.r, m_panel.color.g, m_panel.color.b, m_alpha);
        }
    }
}
