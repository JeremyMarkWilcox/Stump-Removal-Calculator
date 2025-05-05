using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StumpCalculator : MonoBehaviour
{
    [SerializeField] private TMP_InputField diameterInput;
    [SerializeField] private TMP_Dropdown accessibilityDropdown;
    [SerializeField] private TMP_Dropdown rootDepthDropdown;
    [SerializeField] private Toggle debrisToggle;
    [SerializeField] private TMP_Dropdown backfillDropdown;
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
        backfillDropdown.value = 0;

        // Configure input fields for numeric keypad on Android
        diameterInput.contentType = TMP_InputField.ContentType.DecimalNumber;
        diameterInput.keyboardType = TouchScreenKeyboardType.NumberPad;
        diameterInput.inputType = TMP_InputField.InputType.Standard; // Ensures no auto-correction or suggestions

        travelDistanceInput.contentType = TMP_InputField.ContentType.DecimalNumber;
        travelDistanceInput.keyboardType = TouchScreenKeyboardType.NumberPad;
        travelDistanceInput.inputType = TMP_InputField.InputType.Standard;
    }

    public void CalculateCost()
    {
        if (string.IsNullOrEmpty(diameterInput.text) || !float.TryParse(diameterInput.text, out float diameter) || diameter <= 0)
        {
            resultText.text = "Please enter a valid diameter.";
            return;
        }
        float travelDistance = string.IsNullOrEmpty(travelDistanceInput.text) ? 0f : float.Parse(travelDistanceInput.text);

        // Base cost: $5 per inch
        float baseCost = diameter * 5f;

        // Accessibility multiplier: Medium 20%, Hard 50%
        float accessMultiplier = accessibilityDropdown.value switch
        {
            0 => 1.0f, // Under 50 Yards
            1 => 1.2f, // 50 - 100 Yards
            2 => 1.5f, // 100 Yards or More
            _ => 1.0f
        };

        // Root depth multiplier: Medium 30%, Deep 60% 
        float rootMultiplier = rootDepthDropdown.value switch
        {
            0 => 1.0f, // 2 - 4"
            1 => 1.3f, // 5 - 8"
            2 => 1.6f, // 9"
            _ => 1.0f
        };

        // Debris removal cost: $4 per inch if toggle is on
        float debrisCost = debrisToggle.isOn ? diameter * 4f *accessMultiplier : 0f;

        // Backfill cost: $100 per yard, based on dropdown
        float backfillCost = backfillDropdown.value switch
        {
            1 => 100f,  // 1 yard
            2 => 200f,  // 2 yards
            3 => 300f,  // 3 yards
            _ => 0f     
        };
        backfillCost *= accessMultiplier;

        // Travel fee: $0.75 per mile
        float travelCost = travelDistance * 0.75f;
        float jobCost = (baseCost * accessMultiplier + debrisCost + backfillCost) * rootMultiplier;
        float totalCost = jobCost + travelCost;

        // Display result with breakdown
        string debrisLabel = debrisToggle.isOn ? "Included" : "None";
        string backfillLabel = backfillDropdown.options[backfillDropdown.value].text;
        resultText.text = $"Estimated Cost: ${totalCost:F2}\n" +
                          $"Base: ${(baseCost * accessMultiplier * rootMultiplier):F2}\n" +
                          $"Debris ({debrisLabel}): ${(debrisCost * rootMultiplier):F2}\n" +
                          $"Backfill ({backfillLabel}): ${(backfillCost * rootMultiplier):F2}\n" +
                          $"Travel: ${travelCost:F2}";
    }

    public void Reset()
    {
        diameterInput.text = "";
        travelDistanceInput.text = "";
        accessibilityDropdown.value = 0;
        rootDepthDropdown.value = 0;
        debrisToggle.isOn = false;
        backfillDropdown.value = 0; // Default to None
        resultText.text = "Enter values and click Calculate.";
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

}