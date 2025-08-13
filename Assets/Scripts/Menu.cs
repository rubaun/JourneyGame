using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private static float tempo = 0.5f;
    [SerializeField] private AudioClip somBackgroundMenu;
    [SerializeField] private AudioClip somBotao;
    [Header("Cenas de Sorte e Azar")]
    [SerializeField] private string nomeCenaSorte = "CenaSorte";
    [SerializeField] private string nomeCenaAzar = "CenaAzar";
    private GameObject painelConfiguracao;
    private bool paused = false;
    private SoundPlayer soundPlayer;
    private WaitForSeconds tempoDeEspera = new WaitForSeconds(tempo);
    private PlayerPrefsGame playerPrefsGame;
    private string nomeCenaProxima;

    private void Awake()
    {
        playerPrefsGame = GameObject.Find("PlayerPrefsGame").GetComponent<PlayerPrefsGame>();
        
        if (SceneManager.GetActiveScene().name == "Vitoria")
        {
            nomeCenaProxima = PlayerPrefs.GetString("NomeCenaProxima");
            Debug.Log("Próxima Cena: " + nomeCenaProxima);
        }
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

        if(SceneManager.GetActiveScene().name == "Vitoria")
        {
            GameObject.Find("BotaoMudarFase").GetComponent<Button>().onClick.AddListener(() => CarregarCenaJogo(nomeCenaProxima));
        }   
    }

    public void CarregarCenaJogo(string nomeCena)
    {
        StartCoroutine(CarregarCena(nomeCena));
    }

    public void CarregarCenaSorte()
    {
        StartCoroutine(CarregarCenaComSorte(nomeCenaSorte, nomeCenaAzar));
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

    IEnumerator CarregarCenaComSorte(string nomeCenaSorte, string nomeCenaAzar)
    {
        int sorte = Random.Range(1, 5);

        if (soundPlayer != null)
        {
            soundPlayer.PlaySound(somBotao);
        }

        yield return tempoDeEspera;

        if (sorte == 1)
        {
            SceneManager.LoadScene(nomeCenaSorte);
        }
        else
        {
            SceneManager.LoadScene(nomeCenaAzar);
        }    
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
