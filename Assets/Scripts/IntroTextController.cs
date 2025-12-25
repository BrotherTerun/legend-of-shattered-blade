using System.Collections;
using TMPro;
using UnityEngine;

public class IntroLineByLineBlur : MonoBehaviour
{
    [System.Serializable]
    public class TextBlock
    {
        public TMP_Text textComponent;
        public float lineDelay = 0.3f;       // задержка между строками
        public float fadeDuration = 0.5f;    // врем€ фейда построчно
        public float displayTime = 2f;       // врем€ показа текста после по€влени€
        public float fadeOutDuration = 1f;   // врем€ полного исчезновени€ блока
        public float maxBlur = 1.5f;         // максимальна€ размытие при по€влении
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
            // Fade-in построчно с размытие
            yield return StartCoroutine(FadeInLines(block));

            // ∆дЄм некоторое врем€
            yield return new WaitForSeconds(block.displayTime);

            // ѕолный fade-out блока
            yield return StartCoroutine(FadeOutBlock(block));
        }

        Debug.Log("Intro finished");
    }

    private IEnumerator FadeInLines(TextBlock block)
    {
        TMP_Text text = block.textComponent;
        text.ForceMeshUpdate();

        int linesCount = text.textInfo.lineCount;
        Material mat = text.fontMaterial;

        for (int i = 0; i < linesCount; i++)
        {
            yield return StartCoroutine(FadeLineWithBlur(text, mat, i, 0f, 1f, block.maxBlur, 0f, block.fadeDuration));
            yield return new WaitForSeconds(block.lineDelay);
        }
    }

    private IEnumerator FadeOutBlock(TextBlock block)
    {
        TMP_Text text = block.textComponent;
        Material mat = text.fontMaterial;
        float elapsed = 0f;
        float startAlpha = 1f;
        float endAlpha = 0f;
        float startBlur = 0f;
        float endBlur = block.maxBlur;

        while (elapsed < block.fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / block.fadeOutDuration);
            float blur = Mathf.Lerp(startBlur, endBlur, elapsed / block.fadeOutDuration);

            text.alpha = alpha;
            mat.SetFloat("_GlowSoftness", blur);

            yield return null;
        }

        text.alpha = endAlpha;
        mat.SetFloat("_GlowSoftness", endBlur);
    }

    private IEnumerator FadeLineWithBlur(TMP_Text text, Material mat, int lineIndex,
                                         float startAlpha, float endAlpha,
                                         float startBlur, float endBlur,
                                         float duration)
    {
        float elapsed = 0f;
        TMP_TextInfo textInfo = text.textInfo;
        int startChar = textInfo.lineInfo[lineIndex].firstCharacterIndex;
        int endChar = textInfo.lineInfo[lineIndex].lastCharacterIndex;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            float blur = Mathf.Lerp(startBlur, endBlur, elapsed / duration);

            // ”станавливаем alpha построчно
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

            // ѕримен€ем размытие через Glow/Softness
            mat.SetFloat("_GlowSoftness", blur);

            yield return null;
        }

        // ‘инальна€ установка
        for (int i = startChar; i <= endChar; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            int meshIndex = charInfo.materialReferenceIndex;
            var vertexColors = text.textInfo.meshInfo[meshIndex].colors32;
            for (int v = 0; v < 4; v++)
            {
                Color32 c = vertexColors[charInfo.vertexIndex + v];
                c.a = (byte)(endAlpha * 255);
                vertexColors[charInfo.vertexIndex + v] = c;
            }
        }
        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        mat.SetFloat("_GlowSoftness", endBlur);
    }
}