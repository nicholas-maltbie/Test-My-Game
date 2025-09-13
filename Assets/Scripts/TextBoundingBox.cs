using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TextBoundingBox : MonoBehaviour
{
    [SerializeField]
    private Image backgroundBox;

    [SerializeField]
    private TMP_Text textComponent;

    [SerializeField]
    private Vector2 margin = new Vector2(10, 10);

    public string CurrentText => this.textComponent.text;

    public void LateUpdate()
    {
        var textBounds = this.textComponent.textBounds;
        var targetSize = new Vector2(textBounds.size.x, textBounds.size.y) + 2 * margin;

        if (backgroundBox != null)
        {
            this.backgroundBox.rectTransform.localPosition = this.textComponent.rectTransform.localPosition + textBounds.center;
            this.backgroundBox.rectTransform.sizeDelta = targetSize;

            bool targetActiveState = this.textComponent.text.Trim().Length > 0;
            if (targetActiveState != this.backgroundBox.IsActive())
            {
                this.backgroundBox.gameObject.SetActive(targetActiveState);
            }
        }
    }

    public void UpdateText(string text)
    {
        this.textComponent.text = text;
        this.textComponent.ForceMeshUpdate();
    }
}