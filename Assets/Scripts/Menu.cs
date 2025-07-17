using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private static float tempo = 0.5f;
    [SerializeField] private AudioClip somBackgroundMenu;
    [SerializeField] private AudioClip somBotao;
    [SerializeField] private GameObject painelConfiguracao;
    private bool paused = false;
    private SoundPlayer player;
    private WaitForSeconds tempoDeEspera = new WaitForSeconds(tempo);


    void Start()
    {
        player = GameObject.Find("Audio Source").GetComponent<SoundPlayer>();
        painelConfiguracao = GameObject.Find("PainelConfiguracao");
        if (painelConfiguracao != null)
        {
            painelConfiguracao.SetActive(false);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name == "MenuPrincipal" && SceneManager.GetActiveScene().buildIndex == level)
        {
            if (somBackgroundMenu != null && player != null)
            {
                player.PlaySoundBackground(somBackgroundMenu);
            }
        }
        else if (SceneManager.GetActiveScene().name == "EscolhaFloresta" && SceneManager.GetActiveScene().buildIndex == level)
        {
            if (somBackgroundMenu != null && player != null)
            {
                player.PlaySoundBackground(somBackgroundMenu);
            }
        }
        else if (SceneManager.GetActiveScene().name == "FlorestaEsquerdaBatalha" && SceneManager.GetActiveScene().buildIndex == level)
        {
            if (somBackgroundMenu != null && player != null)
            {
                player.PlaySoundBackground(somBackgroundMenu);
            }
        }
        else if (SceneManager.GetActiveScene().name == "FlorestaDireitaBatalha" && SceneManager.GetActiveScene().buildIndex == level)
        {
            if (somBackgroundMenu != null && player != null)
            {
                player.PlaySoundBackground(somBackgroundMenu);
            }
        }
        else if (SceneManager.GetActiveScene().name == "FlorestaDireitaPortal" && SceneManager.GetActiveScene().buildIndex == level)
        {
            if (somBackgroundMenu != null && player != null)
            {
                player.PlaySoundBackground(somBackgroundMenu);
            }
        }
        else if (SceneManager.GetActiveScene().name == "FlorestaSaidaEsquerda" && SceneManager.GetActiveScene().buildIndex == level)
        {
            if (somBackgroundMenu != null && player != null)
            {
                player.PlaySoundBackground(somBackgroundMenu);
            }
        }
        
    }

    private void Update()
    {
        OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
    }

    public void CarregarCenaJogo(string nomeCena)
    {
        StartCoroutine(CarregarCena(nomeCena));
    }

    IEnumerator CarregarCena(string nomeCena)
    {
        if (player != null)
        {
            player.PlaySound(somBotao);
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

    public void SairJogo()
    {
        Application.Quit();
    }

}
