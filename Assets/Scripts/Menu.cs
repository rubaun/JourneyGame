using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private static float tempo = 0.5f;
    [SerializeField] private AudioClip somBackgroundMenu;
    [SerializeField] private AudioClip somBotao;

    [Header("Fluxo inicial")]
    [SerializeField] private string cenaSelecaoPersonagem = "SelecionaPersonagens";
    [SerializeField] private Button botaoContinuar;

    [Header("Cenas de Sorte e Azar")]
    [SerializeField] private string nomeCenaSorte = "CenaSorte";
    [SerializeField] private string nomeCenaAzar = "CenaAzar";

    private GameObject painelConfiguracao;
    private bool paused = false;
    private SoundPlayer soundPlayer;
    private WaitForSeconds tempoDeEspera = new WaitForSeconds(tempo);
    private PlayerPrefsGame playerPrefsGame;
    private string nomeCenaProxima;
    private TextMeshProUGUI textoVersao;

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

        textoVersao = GameObject.Find("version")?.GetComponent<TextMeshProUGUI>();
        if (textoVersao != null)
        {
            textoVersao.text = $"v{Application.version}";
        }

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

        StartCoroutine(AtualizarEstadoBotaoContinuar());
    }

    private void OnEnable()
    {
        if (SessaoJogoManager.Instance != null)
            SessaoJogoManager.Instance.OnCheckpointAtualizado += AtualizarBotaoContinuar;
    }

    private void OnDisable()
    {
        if (SessaoJogoManager.Instance != null)
            SessaoJogoManager.Instance.OnCheckpointAtualizado -= AtualizarBotaoContinuar;
    }

    private IEnumerator AtualizarEstadoBotaoContinuar()
    {
        if (botaoContinuar == null) yield break;

        botaoContinuar.interactable = false;

        float timeout = 5f;
        float elapsed = 0f;

        while ((SessaoJogoManager.Instance == null || !SessaoJogoManager.Instance.CheckpointSincronizado) && elapsed < timeout)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        AtualizarBotaoContinuar();
    }

    private void AtualizarBotaoContinuar()
    {
        if (botaoContinuar == null) return;

        bool habilitar = SessaoJogoManager.Instance != null &&
                         SessaoJogoManager.Instance.TemJogoEmAndamento();

        botaoContinuar.interactable = habilitar;
    }

    public void CarregarCenaJogo(string nomeCena)
    {
        StartCoroutine(CarregarCena(nomeCena));
    }

    public void CarregarCenaSorte()
    {
        StartCoroutine(CarregarCenaComSorte(nomeCenaSorte, nomeCenaAzar));
    }

    private string ResolverNomeCena(string nomeCena)
    {
        if (string.IsNullOrWhiteSpace(nomeCena))
            return cenaSelecaoPersonagem;

        // Compatibilidade com nomes antigos
        if (nomeCena == "SelecaoPersonagem")
            return "SelecionaPersonagens";

        return nomeCena;
    }

    IEnumerator CarregarCena(string nomeCena)
    {
        if (soundPlayer != null)
        {
            soundPlayer.PlaySound(somBotao);
        }
        yield return tempoDeEspera;

        nomeCena = ResolverNomeCena(nomeCena);

        if (!Application.CanStreamedLevelBeLoaded(nomeCena))
        {
            Debug.LogError($"Cena '{nomeCena}' não existe ou não está no Build Profiles.");
            yield break;
        }

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
        if (paused)
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

    // Botão JOGAR: inicia novo jogo e descarta progresso atual.
    public void JogarNovoJogo()
    {
        if (SessaoJogoManager.Instance != null)
            SessaoJogoManager.Instance.DescartarJogoAtual();

        StartCoroutine(CarregarCena(cenaSelecaoPersonagem));
    }

    // Botão CONTINUAR: volta para a batalha em checkpoint.
    public void ContinuarJogo()
    {
        if (SessaoJogoManager.Instance == null) return;

        string cenaDestino = SessaoJogoManager.Instance.ObterCenaParaContinuar(cenaSelecaoPersonagem);
        StartCoroutine(CarregarCena(cenaDestino));
    }
}
