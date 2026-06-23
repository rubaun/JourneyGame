using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiretorBatalha : MonoBehaviour
{
    [Header("Bootstrap")]
    [SerializeField] private GameObject audioManagerPrefab;

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
    [SerializeField] private Button botaoItem;
    [SerializeField] private InventarioBatalha inventarioPlayer;
    [SerializeField] private InventarioBatalha inventarioInimigo;
    [SerializeField, Range(0, 100)] private int chanceContraAtaque = 20;
    string turno = "Player";
    bool verificadorDeTurno = true;
    bool verificadorDoContador = true;
    Coroutine contadorCoroutine;
    int contador;

    private void Awake()
    {
        AudioBootstrapper.GarantirAudioManager(audioManagerPrefab);
    }

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

        if (SessaoJogoManager.Instance != null)
            SessaoJogoManager.Instance.MarcarBatalhaEmAndamento(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        AtualizaDadosTela();

        if (turno == "Player" && verificadorDeTurno && player.VerificaVida())
        {
            ((IPersonagemBatalha)player).AtualizarEfeitosPorTurno();
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
            ((IPersonagemBatalha)inimigo).AtualizarEfeitosPorTurno();
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
        int dano = player.Ataque();
        inimigo.LevarDano(dano);
        TentarContraAtaqueCorpoACorpo(player, inimigo, dano);
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

        if (turno != "Inimigo") yield break;

        botaoAtaque.interactable = false;
        botaoEspecial.interactable = false;

        bool consumiuTurnoComItem = TentarUsarItemInimigo();

        if (!consumiuTurnoComItem)
        {
            // IA simples sem magia: ataca ou usa especial se disponível
            if (inimigo.VerificaEspecial() && Random.value < 0.35f)
            {
                player.LevarDano(inimigo.Especial());
            }
            else
            {
                int dano = inimigo.Ataque();
                player.LevarDano(dano);
                TentarContraAtaqueCorpoACorpo(inimigo, player, dano);
            }
        }

        yield return new WaitForSeconds(2f);
        verificadorDoContador = true;
        verificadorDeTurno = true;
        turno = "Player";
        contadorCoroutine = StartCoroutine(ContadorRoundPlayer());
    }

    private bool TentarUsarItemInimigo()
    {
        if (inventarioInimigo == null) return false;

        ItemBatalhaData itemEscolhido = EscolherItemInimigo();
        if (itemEscolhido == null) return false;

        if (!inventarioInimigo.TentarConsumir(itemEscolhido)) return false;

        AplicarItem(inimigo, itemEscolhido);
        return itemEscolhido.consomeTurno;
    }

    private ItemBatalhaData EscolherItemInimigo()
    {
        if (Random.value > 0.50f) return null;

        bool vidaCritica = inimigo.GetVida() <= 30;
        bool oponenteForte = player.GetVida() > 35;

        if (vidaCritica)
        {
            var cura = BuscarItemInimigoPorEfeito(EfeitoItemBatalha.CuraVida);
            if (cura != null) return cura;

            var escudo = BuscarItemInimigoPorEfeito(EfeitoItemBatalha.Escudo);
            if (escudo != null) return escudo;
        }

        if (oponenteForte)
        {
            var buffDef = BuscarItemInimigoPorEfeito(EfeitoItemBatalha.BuffDefesa);
            if (buffDef != null) return buffDef;
        }

        var buffAtk = BuscarItemInimigoPorEfeito(EfeitoItemBatalha.BuffAtaque);
        if (buffAtk != null) return buffAtk;

        return null;
    }

    private ItemBatalhaData BuscarItemInimigoPorEfeito(EfeitoItemBatalha efeito)
    {
        var slots = inventarioInimigo.ObterSlots();
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (slot.item == null) continue;
            if (slot.quantidade <= 0) continue;
            if (!slot.item.utilizavelEmBatalha) continue;
            if (slot.item.efeitoBatalha != efeito) continue;

            return slot.item;
        }

        return null;
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

        if (SessaoJogoManager.Instance != null)
            SessaoJogoManager.Instance.ConcluirBatalhaAtual();

        SceneManager.LoadScene("Vitoria");
    }

    IEnumerator TelaMorte()
    {
        yield return new WaitForSeconds(1.0f);
        player.PlaySomMorte();
        SceneManager.LoadScene("Derrota");
    }

    public void UsarItemPlayer(ItemBatalhaData item)
    {
        if (item == null || inventarioPlayer == null) return;
        if (!inventarioPlayer.TentarConsumir(item)) return;

        IPersonagemBatalha alvo = player;
        AplicarItem(alvo, item);

        if (item.consomeTurno)
        {
            StartCoroutine(AtaqueP());
        }
    }

    private void AplicarItem(IPersonagemBatalha alvo, ItemBatalhaData item)
    {
        if (!item.utilizavelEmBatalha) return;

        switch (item.efeitoBatalha)
        {
            case EfeitoItemBatalha.CuraVida:
                alvo.CurarVida(item.valor);
                break;
            case EfeitoItemBatalha.RecuperaMana:
                alvo.RecuperarMana(item.valor);
                break;
            case EfeitoItemBatalha.BuffAtaque:
                alvo.AplicarBuffAtaque(item.valor, item.duracaoTurnos);
                break;
            case EfeitoItemBatalha.BuffDefesa:
                alvo.AplicarBuffDefesa(item.valor, item.duracaoTurnos);
                break;
            case EfeitoItemBatalha.Escudo:
                alvo.AtivarEscudoItem(item.valor, item.duracaoTurnos);
                break;
        }

        alvo.MostrarTextoAcao(item.nomeExibicao);
        RecebeTexto($"{alvo.NomePersonagem} usou {item.nomeExibicao}");
    }

    private void TentarContraAtaqueCorpoACorpo(Player atacante, Player defensor, int danoAtaque)
    {
        if (danoAtaque <= 0) return; // ataque errou
        if (!atacante.VerificaVida() || !defensor.VerificaVida()) return;
        if (Random.Range(0, 100) >= chanceContraAtaque) return;

        RecebeTexto($"{defensor.GetNomePersonagem()} contra-ataca!");
        int danoContra = defensor.Ataque();
        if (danoContra > 0)
        {
            atacante.LevarDano(danoContra);
        }
    }
}
