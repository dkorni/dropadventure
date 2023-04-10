using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BalanceLabel : MonoBehaviour
{
    [Inject] CoinBank coinBank;
    [SerializeField] Text text;

    // Start is called before the first frame update
    void Start()
    {
        text.text = coinBank.Amount.ToString();

        coinBank.OnBalanceUpdated += UpdateAmount;
    }

    private void UpdateAmount(int amount)
    {
        text.text = amount.ToString();
    }

    private void Disable()
    {
        coinBank.OnBalanceUpdated -= UpdateAmount;
    }
}
