using TMPro;
using UnityEngine;

public class BattleRewardManager : MonoBehaviour
{
    public static BattleRewardManager instance;

    [SerializeField] GameObject rewardPanel;
    [SerializeField] TextMeshProUGUI EXPText;

    [Header("Items Pillaged")]
    [SerializeField] Transform itemsPillagedParent;
    [SerializeField] BattleRewardUIElement battleRewardPrefab;

    [Space]
    [SerializeField] BattleRewardWithQuantity[] rewardItemsTest;

    BattleRewardWithQuantity[] rewardItems;
    int EXPEarned;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Y))
        //    OpenRewardsScreen(55, rewardItemsTest);
    }

    public void OpenRewardsScreen(int _EXPEarned, BattleRewardWithQuantity[] _rewardItems)
    {
        GameManager.instance.battleActive = false;
        EXPEarned = _EXPEarned;
        rewardItems = _rewardItems;

        EXPText.SetText("Everyone earned " + EXPEarned + " EXP!");

        if (itemsPillagedParent.childCount > 0)
        {
            for (int i = 0; i < itemsPillagedParent.childCount; i++)
                Destroy(itemsPillagedParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < rewardItems.Length; i++)
        {
            BattleRewardUIElement battleReward = Instantiate(battleRewardPrefab, itemsPillagedParent);
            battleReward.Setup(sprite: rewardItems[i].Item.ItemSprite, itemName: rewardItems[i].Item.ItemName, quantity: "x" + rewardItems[i].Quantity);
        }

        rewardPanel.SetActive(true);
    }

    public void CloseRewardsScreen()
    {
        for (int i = 0; i < GameManager.instance.PlayerStatsList.Length; i++)
        {
            if (GameManager.instance.PlayerStatsList[i].gameObject.activeInHierarchy)
                GameManager.instance.PlayerStatsList[i].AddEXP(EXPEarned);
        }

        for (int i = 0; i < rewardItems.Length; i++)
            GameManager.instance.AddItemToInventory(rewardItems[i].Item, rewardItems[i].Quantity);

        UIController.instance.UpdateInfoHolderStats(); // to update the UI to reflect newly gained EXP
        rewardPanel.SetActive(false);
    }
}
