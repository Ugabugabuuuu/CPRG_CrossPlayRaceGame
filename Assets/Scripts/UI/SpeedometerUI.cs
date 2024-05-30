using TMPro;
using UnityEngine;

public class SpeedometerUI : MonoBehaviour
{
    private Rigidbody target;
    [SerializeField] private float maxSpeed = 0.0f;
    [SerializeField] private float minSpeedArrowAngle;
    [SerializeField] private float maxSpeedArrowAngle;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private RectTransform arrowTransform;

    private float speed = 0.0f;
    private void Start()
    {
        GameManager.Instance.OnStateChanged += Instance_OnStateChanged;
    }

    private void Instance_OnStateChanged(object sender, System.EventArgs e)
    {
        if(GameManager.Instance.IsGameOver())
        {
            Hide();
        }
    }

    private void Update()
    {
        target = CarController.LocalInstance.GetComponent<Rigidbody>();
        if (GameManager.Instance.IsGamePlaying())
        {
            UpdateSpeedometer();
        }
        else return;

    }
    private void UpdateSpeedometer()
    {
        speed = target.velocity.magnitude * 3.6f;
        if(speedText!=null)
        {
            speedText.text = ((int)speed) + " km/h";
        }
        if (arrowTransform != null)
        {
            arrowTransform.localEulerAngles = new Vector3(0,0,Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, speed/maxSpeed));
        }

    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
