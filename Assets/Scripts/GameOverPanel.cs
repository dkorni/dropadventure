using UnityEngine;
using Zenject;

public class GameOverPanel : MonoBehaviour
{
    [Inject] private Game _game;

    // Start is called before the first frame update
    void Start()
    {
        _game.OnStateChanged += OnStateChanged;
        gameObject.SetActive(false);
    }

    private void OnStateChanged(object sender, GameStates e)
    {
       if(e == GameStates.GameOver)
           gameObject.SetActive(true);
    }

    public void Retry()
    {
        _game.Retry();
    }
}
