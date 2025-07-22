using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private static float tempo = 0.5f;
    [SerializeField] private AudioClip somBackgroundMenu;
    [SerializeField] private AudioClip somBotao;
    private GameObject painelConfiguracao;
    private bool paused = false;
    private SoundPlayer soundPlayer;
    private WaitForSeconds tempoDeEspera = new WaitForSeconds(tempo);
    private PlayerPrefsGame playerPrefsGame;

    private void Awake()
    {
        playerPrefsGame = GameObject.Find("PlayerPrefsGame").GetComponent<PlayerPrefsGame>();
    }

    void Start()
    {
        soundPlayer = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundPlayer>();
        painelConfiguracao = GameObject.Find("PainelConfiguracao");


        if (painelConfiguracao != null)
        {
            painelConfiguracao.SetActive(false);
        }

        if (somBackgroundMenu != null && soundPlayer != null)
        {
            soundPlayer.PlaySoundBackground(somBackgroundMenu);
        }
    }

    public void CarregarCenaJogo(string nomeCena)
    {
        StartCoroutine(CarregarCena(nomeCena));
    }

    IEnumerator CarregarCena(string nomeCena)
    {
        if (soundPlayer != null)
        {
            soundPlayer.PlaySound(somBotao);
        }
        yield return tempoDeEspera;
        SceneManager.LoadScene(nomeCena);
    }

    public void PauseGame()
    {
        if(paused)
        {
            Time.timeScale = 1f;
            paused = false;
            if (painelConfiguracao != null)
            {
                painelConfiguracao.SetActive(false);
            }
        }
        else
        {
            Time.timeScale = 0f;
            paused = true;
            if (painelConfiguracao != null)
            {
                painelConfiguracao.SetActive(true);
            }
        }
    }

    public void JogarNovamente()
    {
        StartCoroutine(CarregarCena(playerPrefsGame.GetCenaAnterior()));
    }

    public void SairJogo()
    {
        Application.Quit();
    }

}
