using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiretorBatalha : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Player inimigo;
    [SerializeField] int tempoRoundPlayer = 20;
    [SerializeField] TextMeshProUGUI vidaPlayer;
    [SerializeField] TextMeshProUGUI vidaInimigo;
    [SerializeField] TextMeshProUGUI nomePlayer;
    [SerializeField] TextMeshProUGUI nomeInimigo;
    [SerializeField] TextMeshProUGUI indicadorTempo;
    [SerializeField] TextMeshProUGUI informativo;
    [SerializeField] TextMeshProUGUI indicadorEspecial;
    [SerializeField] Button botaoEspecial;
    [SerializeField] Button botaoAtaque;
    string turno = "Player";
    bool verificadorDeTurno = true;
    bool verificadorDoContador = true;
    Coroutine contadorCoroutine;
    int contador;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vidaPlayer = GameObject.Find("VidaPlayer").GetComponent<TextMeshProUGUI>();
        vidaPlayer.text = player.GetVida().ToString();
        vidaInimigo = GameObject.Find("VidaInimigo").GetComponent<TextMeshProUGUI>();
        vidaInimigo.text = inimigo.GetVida().ToString();
        nomePlayer = GameObject.Find("NomePlayer").GetComponent<TextMeshProUGUI>();
        nomePlayer.text = player.GetNomePersonagem();
        nomeInimigo = GameObject.Find("NomeInimigo").GetComponent<TextMeshProUGUI>();
        nomeInimigo.text = inimigo.GetNomePersonagem();
        indicadorEspecial = GameObject.Find("IndicadorEspecial").GetComponentInChildren<TextMeshProUGUI>();
        indicadorEspecial.text = player.ValorEspecial().ToString();
        indicadorTempo = GameObject.Find("IndicadorTempo").GetComponent<TextMeshProUGUI>();
        indicadorTempo.text = tempoRoundPlayer.ToString();
        botaoEspecial.interactable = false;
        DefinirCorBotaoDesabilitado();
        contadorCoroutine = StartCoroutine(ContadorRoundPlayer());
    }

    void Update()
    {
        AtualizaDadosTela();

        if(turno == "Player" && verificadorDeTurno && player.VerificaVida())
        {
            botaoAtaque.interactable = true;

            if (player.VerificaEspecial())
            {
                botaoEspecial.interactable = true;
            }
            else
            {
                botaoEspecial.interactable = false;
            }

            verificadorDeTurno = false;
        }
        else if (turno == "Inimigo" && verificadorDeTurno && inimigo.VerificaVida())
        {
            StartCoroutine(AtaqueInimigo());
        }

        VerificaVitoria();
    }

    private void DefinirCorBotaoDesabilitado()
    {
        // Acessa o ColorBlock do botão
        ColorBlock ca = botaoAtaque.colors;
        ColorBlock ce = botaoEspecial.colors;

        // Altera a cor para o estado desabilitado
        ca.disabledColor = new Color(0f, 0f, 0f, 0.5f);
        ce.disabledColor = new Color(0f, 0f, 0f, 0.5f);

        // Aplica de volta ao botão
        botaoAtaque.colors = ca;
        botaoEspecial.colors = ce;
    }
    public void AtaquePlayer()
    {
        inimigo.LevarDano(player.Ataque());
        StartCoroutine(AtaqueP());
    }

    public void AtaqueEspecial()
    {
        inimigo.LevarDano(player.Especial());
        StartCoroutine(AtaqueP());
    }

    private void AtualizaDadosTela()
    {
        vidaPlayer.text = player.GetVida().ToString();
        vidaInimigo.text = inimigo.GetVida().ToString();
    }

    public void RecebeTexto(string texto)
    {
        StartCoroutine(ExibeTexto(texto));
    }

    private IEnumerator ContadorRoundPlayer()
    {
        Debug.Log("Contador Iniciado");

        contador = tempoRoundPlayer;

        if (turno == "Player" && verificadorDeTurno)
        {
            while (verificadorDoContador && contador > 0)
            {
                yield return new WaitForSeconds(1f);
                contador--;
                indicadorTempo.text = contador.ToString();
                Debug.Log($"Contador: {contador}");
            }

            if (contador <= 0)
            {
                informativo.text = "Tempo esgotado!";
                StartCoroutine(AtaqueP());
            }
        }
    }

    private void StopContador()
    {
        if (contadorCoroutine != null)
        {
            StopCoroutine(contadorCoroutine);
            contadorCoroutine = null;
        }
        verificadorDoContador = false;
        indicadorTempo.text = "20";
    }

    private IEnumerator ExibeTexto(string texto)
    {
        informativo.text += texto + "\n";
        yield return new WaitForSeconds(3f);
        informativo.text = "";
    }

    private IEnumerator AtaqueInimigo()
    {
        StopContador();
        verificadorDeTurno = false;

        if (turno == "Inimigo")
        {
            botaoAtaque.interactable = false;
            botaoEspecial.interactable = false;
            player.LevarDano(inimigo.Ataque());
            yield return new WaitForSeconds(3f);
            verificadorDoContador = true;
            verificadorDeTurno = true;
            turno = "Player";
            contadorCoroutine = StartCoroutine(ContadorRoundPlayer());
        }
    }

    private IEnumerator AtaqueP()
    {
        StopContador();
        verificadorDeTurno = false;
        botaoAtaque.interactable = false;
        botaoEspecial.interactable = false;
        indicadorEspecial.text = player.ValorEspecial().ToString();

        if (turno == "Player")
        {
            yield return new WaitForSeconds(3f);
            verificadorDeTurno = true;
            turno = "Inimigo";
        }
    }

    public void VerificaVitoria()
    {

        if (!inimigo.VerificaVida())
        {
            StartCoroutine(TelaVitoria());
        }
        else if (!player.VerificaVida())
        {
            StartCoroutine(TelaMorte());
        }
    }

    IEnumerator TelaVitoria()
    {
        yield return new WaitForSeconds(1.0f);
        player.PlaySomVitoria();
        SceneManager.LoadScene("Vitoria");
    }

    IEnumerator TelaMorte()
    {
        yield return new WaitForSeconds(1.0f);
        player.PlaySomMorte();
        SceneManager.LoadScene("Derrota");
    }

    
}
