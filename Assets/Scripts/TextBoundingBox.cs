using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(TMP_Text))]
public class TextBoundingBox : MonoBehaviour
{
    [SerializeField]
    private Image backgroundBox;

    [SerializeField]
    private Vector2 margin = new Vector2(10, 10);

    private TMP_Text textComponent;

    public void Awake()
    {
        this.textComponent = GetComponent<TMP_Text>();
    }

    public void LateUpdate()
    {
        var textBounds = this.textComponent.textBounds;
        var targetSize = new Vector2(textBounds.size.x, textBounds.size.y) + 2 * margin;

        if (backgroundBox != null)
        {
            this.backgroundBox.transform.position = transform.position + textBounds.center;
            this.backgroundBox.rectTransform.sizeDelta = targetSize;

            bool targetActiveState = this.textComponent.text.Trim().Length > 0;
            if (targetActiveState != this.backgroundBox.IsActive())
            {
                this.backgroundBox.gameObject.SetActive(targetActiveState);
            }
        }
    }
}