using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ButtonVisualFeedback : MonoBehaviour
{
    public Image accelerateButtonImage;
    public Image brakeButtonImage;
    public Image handBreakeButtonImage;
    public float pressedAlpha = 0.5f;
    private Color originalAccelColor;
    private Color originalBrakeColor;
    private Color originalHandBreakeColor;
    private InputAction moveAction;
    private InputAction handBreakeAction;

    private void Start()
    {
        CarController controller = FindObjectOfType<CarController>();

        if (controller == null)
        {
            return;
        }

        moveAction = controller.GetMoveAction();
        handBreakeAction = controller.GetHandBreakeAction();

        if (moveAction == null)
        {
            return;
        }

        originalAccelColor = accelerateButtonImage.color;
        originalBrakeColor = brakeButtonImage.color;
        originalHandBreakeColor = handBreakeButtonImage.color;
    }

    private void Update()
    {
        float yInput = moveAction.ReadValue<Vector2>().y;
        float handBreakeInput = handBreakeAction.ReadValue<float>();

        UpdateButtonColor(yInput, accelerateButtonImage, originalAccelColor);

        UpdateButtonColor(-yInput, brakeButtonImage, originalBrakeColor);

        UpdateButtonColor(handBreakeInput, handBreakeButtonImage, originalHandBreakeColor);
    }

    private void UpdateButtonColor(float input, Image buttonImage, Color originalColor)
    {
        Color color = buttonImage.color;
        color.a = input > 0 ? originalColor.a * pressedAlpha : originalColor.a;
        buttonImage.color = color;
    }
}
