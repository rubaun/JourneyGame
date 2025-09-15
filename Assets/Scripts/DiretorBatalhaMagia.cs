using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiretorBatalhaMagia : MonoBehaviour
{

    [Header("Timer UI")]
    [SerializeField] int tempoRoundPlayer = 20;
    [SerializeField] TextMeshProUGUI indicadorTempo;
    [Header("Player UI")]
    [SerializeField] Mago player;
    [SerializeField] TextMeshProUGUI nomePlayer;
    [SerializeField] TextMeshProUGUI vidaPlayer;
    [SerializeField] Slider manaPlayer;
    [SerializeField] TextMeshProUGUI informativo;
    [SerializeField] TextMeshProUGUI indicadorEspecial;
    [SerializeField] Button botaoEspecial;
    [SerializeField] Button botaoAtaque;
    [SerializeField] Button botaoDefesa;
    [Header("Inimigo UI")]
    [SerializeField] Mago inimigo;
    [SerializeField] TextMeshProUGUI nomeInimigo;
    [SerializeField] TextMeshProUGUI vidaInimigo;
    [SerializeField] Slider manaInimigo;

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
        //manaPlayer = GameObject.Find("BarraManaPlayer").GetComponent<Slider>();
        manaPlayer.maxValue = player.GetMana();
        manaPlayer.value = player.GetMana();
        //manaInimigo = GameObject.Find("BarraManaInimigo").GetComponent<Slider>();
        manaInimigo.maxValue = inimigo.GetMana();
        manaInimigo.value = inimigo.GetMana();
        indicadorEspecial = GameObject.Find("IndicadorEspecial").GetComponentInChildren<TextMeshProUGUI>();
        indicadorEspecial.text = player.ValorEspecial().ToString();
        indicadorTempo = GameObject.Find("IndicadorTempo").GetComponent<TextMeshProUGUI>();
        indicadorTempo.text = tempoRoundPlayer.ToString();
        botaoEspecial.interactable = false;
        botaoDefesa.interactable = false;
        DefinirCorBotaoDesabilitado();
        contadorCoroutine = StartCoroutine(ContadorRoundPlayer());
    }

    void Update()
    {
        AtualizaDadosTela();

        if(turno == "Player" && verificadorDeTurno && player.VerificaVida())
        {
            botaoAtaque.interactable = true;
            botaoDefesa.interactable = true;

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
        ColorBlock cd = botaoDefesa.colors;

        // Altera a cor para o estado desabilitado
        ca.disabledColor = new Color(0f, 0f, 0f, 0.5f);
        ce.disabledColor = new Color(0f, 0f, 0f, 0.5f);
        cd.disabledColor = new Color(0f, 0f, 0f, 0.5f);

        // Aplica de volta ao botão
        botaoAtaque.colors = ca;
        botaoEspecial.colors = ce;
        botaoDefesa.colors = cd;
    }
    public void AtaqueNormalPlayer()
    {
        inimigo.LevarDano(player.AtaqueNormal());
        StartCoroutine(AtaquePlayer());
    }

    public void AtaqueEspecial()
    {
        inimigo.LevarDano(player.Especial());
        StartCoroutine(AtaquePlayer());
    }

    public void DefesaEsquiva()
    {
        player.DefesaEsquiva();
        StartCoroutine(DefesaPlayer());
    }

    private void AtualizaDadosTela()
    {
        vidaPlayer.text = player.GetVida().ToString();
        vidaInimigo.text = inimigo.GetVida().ToString();
        manaPlayer.value = player.GetMana();
        manaInimigo.value = inimigo.GetMana();
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
                StartCoroutine(AtaquePlayer());
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

        int escolhaInimigo = Random.Range(1, 3);

        yield return new WaitForSeconds(2f);

        if (turno == "Inimigo")
        {
            botaoAtaque.interactable = false;
            botaoEspecial.interactable = false;
            botaoDefesa.interactable = false;

            if (escolhaInimigo == 1 && inimigo.VerificaEspecial())
            {
                player.LevarDano(inimigo.Especial());
            }
            else if(escolhaInimigo == 2 )
            {
                player.LevarDano(inimigo.AtaqueNormal());
            }
            else
            {
                inimigo.DefesaEsquiva();
            }
            
            yield return new WaitForSeconds(3f);
            verificadorDoContador = true;
            verificadorDeTurno = true;
            turno = "Player";
            contadorCoroutine = StartCoroutine(ContadorRoundPlayer());
        }
    }

    private IEnumerator AtaquePlayer()
    {
        StopContador();
        verificadorDeTurno = false;
        botaoAtaque.interactable = false;
        botaoEspecial.interactable = false;
        botaoDefesa.interactable = false;
        indicadorEspecial.text = player.ValorEspecial().ToString();

        if (turno == "Player")
        {
            yield return new WaitForSeconds(3f);
            verificadorDeTurno = true;
            turno = "Inimigo";
        }
    }

    private IEnumerator DefesaPlayer()
    {
        StopContador();
        verificadorDeTurno = false;
        botaoAtaque.interactable = false;
        botaoEspecial.interactable = false;
        botaoDefesa.interactable = false;
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
