using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TreeCalculator : MonoBehaviour
{
    [SerializeField] private TMP_InputField heightInput;
    [SerializeField] private TMP_InputField diameterInput;
    [SerializeField] private TMP_Dropdown accessibilityDropdown;
    [SerializeField] private Toggle climbedToggle;
    [SerializeField] private TMP_Dropdown conditionDropdown;
    [SerializeField] private Toggle debrisToggle;
    [SerializeField] private TMP_InputField travelDistanceInput;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button calculateButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button backButton;

    private void Start()
    {
        calculateButton.onClick.AddListener(CalculateCost);
        resetButton.onClick.AddListener(Reset);
        backButton.onClick.AddListener(ReturnToMainMenu);

        debrisToggle.isOn = false;
        climbedToggle.isOn = false;

        heightInput.contentType = TMP_InputField.ContentType.DecimalNumber;
        heightInput.keyboardType = TouchScreenKeyboardType.NumberPad;
        heightInput.inputType = TMP_InputField.InputType.Standard;

        diameterInput.contentType = TMP_InputField.ContentType.DecimalNumber;
        diameterInput.keyboardType = TouchScreenKeyboardType.NumberPad;
        diameterInput.inputType = TMP_InputField.InputType.Standard;

        travelDistanceInput.contentType = TMP_InputField.ContentType.DecimalNumber;
        travelDistanceInput.keyboardType = TouchScreenKeyboardType.NumberPad;
        travelDistanceInput.inputType = TMP_InputField.InputType.Standard;
    }

    public void CalculateCost()
    {
        if (string.IsNullOrEmpty(heightInput.text) || !float.TryParse(heightInput.text, out float height) || height <= 0)
        {
            resultText.text = "Please enter a valid tree height.";
            return;
        }
        if (string.IsNullOrEmpty(diameterInput.text) || !float.TryParse(diameterInput.text, out float diameter) || diameter <= 0)
        {
            resultText.text = "Please enter a valid diameter.";
            return;
        }
        float travelDistance = string.IsNullOrEmpty(travelDistanceInput.text) ? 0f : float.Parse(travelDistanceInput.text);

        // Base cost: $4 per foot of height + $6 per inch of diameter
        float baseCost = height * 4f + diameter * 15f;

        float accessMultiplier = accessibilityDropdown.value switch
        {
            0 => 1.0f, // Under 50 Yards
            1 => 1.2f, // 50 - 100 Yards
            2 => 1.5f, // 100 Yards or More
            _ => 1.0f
        };

        // Climbed multiplier: 20% if climbed
        float climbedMultiplier = climbedToggle.isOn ? 1.2f : 1.0f;

        // Condition multiplier: Diseased 10%, Dead 30%
        float conditionMultiplier = conditionDropdown.value switch
        {
            0 => 1.0f, // Healthy
            1 => 1.1f, // Diseased
            2 => 1.3f, // Dead
            _ => 1.0f
        };

        float debrisCost = debrisToggle.isOn ? height * 2f * accessMultiplier : 0f;

        float travelCost = travelDistance * 0.75f;

        float jobCost = (baseCost * climbedMultiplier * conditionMultiplier) + (debrisCost * accessMultiplier);
        float totalCost = jobCost + travelCost;

        string debrisLabel = debrisToggle.isOn ? "Included" : "None";
        string climbedLabel = climbedToggle.isOn ? "Included" : "None";
        string conditionLabel = conditionDropdown.options[conditionDropdown.value].text;
        resultText.text = $"Estimated Cost: ${totalCost:F2}\n" +
                          $"Base (Height + Diameter): ${(baseCost * climbedMultiplier * conditionMultiplier):F2}\n" +                         
                          $"Debris ({debrisLabel}): ${(debrisCost * accessMultiplier):F2}\n" +
                          $"Climbed ({climbedLabel}): {(climbedToggle.isOn ? "20% Increase" : "None")}\n" +
                          $"Travel: ${travelCost:F2}";
    }

    public void Reset()
    {
        heightInput.text = "";
        diameterInput.text = "";
        accessibilityDropdown.value = 0;
        climbedToggle.isOn = false;
        conditionDropdown.value = 0;
        debrisToggle.isOn = false;
        travelDistanceInput.text = "";
        resultText.text = "Enter values and click Calculate.";
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}