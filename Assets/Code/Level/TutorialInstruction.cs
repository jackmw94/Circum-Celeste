using System.Collections;
using TMPro;
using UnityEngine;
using UnityExtras.Code.Core;
using UnityExtras.Code.Optional.Singletons;

public class TutorialInstruction : SingletonMonoBehaviour<TutorialInstruction>
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _text;

    private Coroutine _showHideCoroutine = null;

    private void Awake()
    {
        _canvasGroup.alpha = 0f;
    }

    public void Hide() => SetText(string.Empty);
    
    public void SetText(string text)
    {
        _text.text = text;

        if (_showHideCoroutine != null)
        {
            StopCoroutine(_showHideCoroutine);
        }

        bool showInstruction = !string.IsNullOrEmpty(text);
        _showHideCoroutine = StartCoroutine(ShowHideInstruction(showInstruction));
    }

    private IEnumerator ShowHideInstruction(bool show)
    {
        yield return Utilities.LerpOverTime(_canvasGroup.alpha, show ? 1f : 0f, 0.33f, f =>
        {
            _canvasGroup.alpha = f;
        });
    }
}