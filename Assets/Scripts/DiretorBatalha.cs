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
    }

    void Update()
    {
        AtualizaDadosTela();

        if (turno == "Player" && verificadorDeTurno && player.VerificaVida())
        {
            botaoAtaque.interactable = true;
            StartCoroutine(ContadorRoundPlayer());

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
            StopCoroutine(ContadorRoundPlayer());
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
        StopContador();
        inimigo.LevarDano(player.Ataque());
        StartCoroutine(AtaqueP());
    }

    public void AtaqueEspecial()
    {
        StopContador();
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
        contador = tempoRoundPlayer;
        if (turno == "Player")
        {
            while (contador > 0)
            {
                yield return new WaitForSeconds(1f);
                contador--;
                indicadorTempo.text = contador.ToString();
            }
            informativo.text = "Tempo esgotado!";
            StartCoroutine(AtaqueP());
        }
    }

    private void StopContador()
    {
        StopCoroutine(ContadorRoundPlayer());
        indicadorTempo.text = "20";
    }

    private IEnumerator ExibeTexto(string texto)
    {
        informativo.text += texto + "\n";
        yield return new WaitForSeconds(5f);
        informativo.text = "";
    }

    private IEnumerator AtaqueInimigo()
    {
        StopCoroutine(ContadorRoundPlayer());
        verificadorDeTurno = false;

        if (turno == "Inimigo")
        {
            botaoAtaque.interactable = false;
            botaoEspecial.interactable = false;
            player.LevarDano(inimigo.Ataque());
            yield return new WaitForSeconds(5f);
            verificadorDeTurno = true;
            turno = "Player";
        }
    }

    private IEnumerator AtaqueP()
    {
        verificadorDeTurno = false;
        botaoAtaque.interactable = false;
        botaoEspecial.interactable = false;
        indicadorEspecial.text = player.ValorEspecial().ToString();

        if (turno == "Player")
        {
            yield return new WaitForSeconds(5f);
            verificadorDeTurno = true;
            indicadorTempo.text = "20";
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
        //StopSound();
        yield return new WaitForSeconds(1.0f);
        player.PlaySomVitoria();
        SceneManager.LoadScene("Vitoria");
        //yield return new WaitForSeconds(1.0f);
        //textoTextoVitoria.SetActive(true);
    }

    IEnumerator TelaMorte()
    {
        //StopSound();
        yield return new WaitForSeconds(1.0f);
        player.PlaySomMorte();
        SceneManager.LoadScene("Derrota");
        //textoTextoDerrota.SetActive(true);
    }

    
}
