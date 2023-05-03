using UnityEngine;
using Zenject;

public class WinController : MonoBehaviour
{
    [SerializeField] private GameContext _context;

    // Start is called before the first frame update
    void Start()
    {
        _context.OnStateChanged += OnStateChanged;
        gameObject.SetActive(false);
    }

    private void OnStateChanged(GameStates e)
    {
        if (e == GameStates.Win)
            gameObject.SetActive(true);
    }

    public void Next()
    {
        //_game.NextLevel();
    }
}