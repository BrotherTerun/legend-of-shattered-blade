using System.Collections;
using TMPro;
using UnityEngine;

public class LineByLineIntroBlur : MonoBehaviour
{
    [System.Serializable]
    public class TextBlock
    {
        public TMP_Text textComponent;
        public float lineDelay = 0.3f;    // задержка между строками
        public float fadeDuration = 0.5f; // время фейда каждой строки
        public float displayTime = 2f;    // время, сколько текст виден после полного появления
        public float maxBlur = 1.5f;      // максимальная размытие при появлении
    }

    public TextBlock[] textBlocks;

    private void Start()
    {
        foreach (var block in textBlocks)
            block.textComponent.alpha = 0f;

        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        foreach (var block in textBlocks)
        {
            yield return StartCoroutine(FadeInLines(block));
            yield return new WaitForSeconds(block.displayTime);
            yield return StartCoroutine(FadeOutLines(block));
        }

        Debug.Log("Intro finished");
    }

    private IEnumerator FadeInLines(TextBlock block)
    {
        TMP_Text text = block.textComponent;
        text.ForceMeshUpdate();

        int linesCount = text.textInfo.lineCount;

        for (int i = 0; i < linesCount; i++)
        {
            yield return StartCoroutine(FadeLineWithBlur(text, i, 0f, 1f, block.maxBlur, 0f, block.fadeDuration));
            yield return new WaitForSeconds(block.lineDelay);
        }
    }

    private IEnumerator FadeOutLines(TextBlock block)
    {
        TMP_Text text = block.textComponent;
        int linesCount = text.textInfo.lineCount;

        for (int i = 0; i < linesCount; i++)
        {
            yield return StartCoroutine(FadeLineWithBlur(text, i, 1f, 0f, 0f, block.maxBlur, block.fadeDuration));
            yield return new WaitForSeconds(block.lineDelay);
        }
    }

    private IEnumerator FadeLineWithBlur(TMP_Text text, int lineIndex, float startAlpha, float endAlpha, float startBlur, float endBlur, float duration)
    {
        float elapsed = 0f;
        TMP_TextInfo textInfo = text.textInfo;
        int startChar = textInfo.lineInfo[lineIndex].firstCharacterIndex;
        int endChar = textInfo.lineInfo[lineIndex].lastCharacterIndex;

        Material mat = text.fontMaterial;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            float blur = Mathf.Lerp(startBlur, endBlur, elapsed / duration);

            for (int i = startChar; i <= endChar; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                int meshIndex = charInfo.materialReferenceIndex;
                var vertexColors = text.textInfo.meshInfo[meshIndex].colors32;
                for (int v = 0; v < 4; v++)
                {
                    Color32 c = vertexColors[charInfo.vertexIndex + v];
                    c.a = (byte)(alpha * 255);
                    vertexColors[charInfo.vertexIndex + v] = c;
                }
            }

            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            // применяем размытие через Glow/Softness
            mat.SetFloat("_GlowSoftness", blur);

            yield return null;
        }

        // финальное значение
        SetLineAlpha(text, lineIndex, endAlpha);
        mat.SetFloat("_GlowSoftness", endBlur);
    }

    private void SetLineAlpha(TMP_Text text, int lineIndex, float alpha)
    {
        TMP_TextInfo textInfo = text.textInfo;
        int startChar = textInfo.lineInfo[lineIndex].firstCharacterIndex;
        int endChar = textInfo.lineInfo[lineIndex].lastCharacterIndex;

        for (int i = startChar; i <= endChar; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            int meshIndex = charInfo.materialReferenceIndex;
            var vertexColors = text.textInfo.meshInfo[meshIndex].colors32;
            for (int v = 0; v < 4; v++)
            {
                Color32 c = vertexColors[charInfo.vertexIndex + v];
                c.a = (byte)(alpha * 255);
                vertexColors[charInfo.vertexIndex + v] = c;
            }
        }
        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}