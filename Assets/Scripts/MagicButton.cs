using TMPro;
using UnityEngine;

public class MagicButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI spellNameText;
    [SerializeField] TextMeshProUGUI spellCostText;

    public void Setup(string _spellName, string _spellCostText)
    {
        spellNameText.SetText(_spellName);
        spellCostText.SetText(_spellCostText);
    }
}
