using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiretorBatalhaMagia : MonoBehaviour
{
    [Header("Bootstrap")]
    [SerializeField] private GameObject audioManagerPrefab;

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
    [SerializeField] private Button botaoItem;
    [SerializeField] private InventarioBatalha inventarioPlayer;
    [SerializeField] private InventarioBatalha inventarioInimigo;
    [Header("Inimigo UI")]
    [SerializeField] Mago inimigo;
    [SerializeField] TextMeshProUGUI nomeInimigo;
    [SerializeField] TextMeshProUGUI vidaInimigo;
    [SerializeField] Slider manaInimigo;

    [SerializeField, Range(0, 100)] private int chanceContraAtaque = 20;

    string turno = "Player";
    bool verificadorDeTurno = true;
    bool verificadorDoContador = true;
    bool jogadorAgiu = false;
    bool batalhaFinalizada = false;
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
        manaPlayer.maxValue = player.GetMana();
        manaPlayer.value = player.GetMana();
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

        if (SessaoJogoManager.Instance != null)
            SessaoJogoManager.Instance.MarcarBatalhaEmAndamento(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        AtualizaDadosTela();

        if(turno == "Player" && verificadorDeTurno && player.VerificaVida())
        {
            player.AtualizarDefesaPorTurno();
            ((IPersonagemBatalha)player).AtualizarEfeitosPorTurno();

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
            verificadorDeTurno = false;
            StartCoroutine(AtaqueInimigo());
        }

        VerificaVitoria();
    }

    private void DefinirCorBotaoDesabilitado()
    {
        ColorBlock ca = botaoAtaque.colors;
        ColorBlock ce = botaoEspecial.colors;
        ColorBlock cd = botaoDefesa.colors;

        ca.disabledColor = new Color(0f, 0f, 0f, 0.5f);
        ce.disabledColor = new Color(0f, 0f, 0f, 0.5f);
        cd.disabledColor = new Color(0f, 0f, 0f, 0.5f);

        botaoAtaque.colors = ca;
        botaoEspecial.colors = ce;
        botaoDefesa.colors = cd;
    }

    private void DesabilitarBotoes()
    {
        botaoAtaque.interactable = false;
        botaoEspecial.interactable = false;
        botaoDefesa.interactable = false;
    }

    public void AtaqueNormalPlayer()
    {
        if (jogadorAgiu) return;
        jogadorAgiu = true;
        StopContador();
        DesabilitarBotoes();
        StartCoroutine(ExecutarAtaqueNormal());
    }

    public void AtaqueEspecial()
    {
        if (jogadorAgiu) return;
        jogadorAgiu = true;
        StopContador();
        DesabilitarBotoes();
        StartCoroutine(ExecutarAtaqueEspecial());
    }

    public void DefesaEsquiva()
    {
        if (jogadorAgiu) return;
        jogadorAgiu = true;
        StopContador();
        DesabilitarBotoes();

        player.DefesaEsquiva();
        StartCoroutine(DefesaPlayer());
    }

    private void TentarContraAtaqueCorpoACorpo(Mago atacante, Mago defensor, int danoAtaque)
    {
        if (danoAtaque <= 0) return;
        if (!atacante.VerificaVida() || !defensor.VerificaVida()) return;

        // Só corpo-a-corpo: ambos sem magia equipada
        if (atacante.EhMago() || defensor.EhMago()) return;

        if (Random.Range(0, 100) >= chanceContraAtaque) return;

        RecebeTexto($"{defensor.GetNomePersonagem()} contra-ataca!");
        int danoContra = defensor.AtaqueNormal();
        if (danoContra > 0)
        {
            atacante.LevarDano(danoContra);
        }
    }

    private IEnumerator ExecutarAtaqueNormal()
    {
        int dano = player.AtaqueNormal();

        // Se é mago, espera o projétil atingir o alvo antes de aplicar dano
        if (player.EhMago() && dano > 0)
        {
            yield return player.LancarMagiaAtaque();
        }

        inimigo.LevarDano(dano);

        if (!player.EhMago())
        {
            TentarContraAtaqueCorpoACorpo(player, inimigo, dano);
        }

        yield return StartCoroutine(AtaquePlayer());
    }

    private IEnumerator ExecutarAtaqueEspecial()
    {
        int dano = player.Especial();

        // Se é mago, espera o projétil especial atingir o alvo
        if (player.EhMago() && dano > 0)
        {
            yield return player.LancarMagiaEspecial();
        }

        inimigo.LevarDano(dano);
        yield return StartCoroutine(AtaquePlayer());
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
        indicadorTempo.text = contador.ToString();

        if (turno == "Player" && verificadorDeTurno)
        {
            while (verificadorDoContador && contador > 0)
            {
                yield return new WaitForSeconds(1f);
                contador--;
                indicadorTempo.text = contador.ToString();
                Debug.Log($"Contador: {contador}");
            }

            if (contador <= 0 && !jogadorAgiu)
            {
                informativo.text = "Tempo esgotado!";
                jogadorAgiu = true;
                DesabilitarBotoes();
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
        inimigo.AtualizarDefesaPorTurno();
        ((IPersonagemBatalha)inimigo).AtualizarEfeitosPorTurno();

        inimigo.FalaDoPersonagem("Ataque");
        yield return new WaitForSeconds(1.5f);

        if (turno != "Inimigo") yield break;

        DesabilitarBotoes();

        bool consumiuTurnoComItem = TentarUsarItemInimigoMagia();

        if (!consumiuTurnoComItem)
        {
            int escolhaInimigo = Random.Range(1, 4);

            if (escolhaInimigo == 1 && inimigo.VerificaEspecial())
            {
                int dano = inimigo.Especial();
                if (inimigo.EhMago() && dano > 0)
                    yield return inimigo.LancarMagiaEspecial();

                player.LevarDano(dano);
            }
            else if (escolhaInimigo == 2)
            {
                int dano = inimigo.AtaqueNormal();
                if (inimigo.EhMago() && dano > 0)
                    yield return inimigo.LancarMagiaAtaque();

                player.LevarDano(dano);

                if (!inimigo.EhMago())
                {
                    TentarContraAtaqueCorpoACorpo(inimigo, player, dano);
                }
            }
            else
            {
                inimigo.DefesaEsquiva();
            }
        }

        yield return new WaitForSeconds(2f);
        EncerrarTurnoInimigoMagia();
    }

    private void EncerrarTurnoInimigoMagia()
    {
        jogadorAgiu = false;
        verificadorDoContador = true;
        verificadorDeTurno = true;
        turno = "Player";
        contadorCoroutine = StartCoroutine(ContadorRoundPlayer());
    }

    private bool TentarUsarItemInimigoMagia()
    {
        if (inventarioInimigo == null) return false;

        ItemBatalhaData itemEscolhido = EscolherItemInimigoMagia();
        if (itemEscolhido == null) return false;

        if (!inventarioInimigo.TentarConsumir(itemEscolhido)) return false;

        AplicarItem(inimigo, itemEscolhido);
        return itemEscolhido.consomeTurno;
    }

    private ItemBatalhaData EscolherItemInimigoMagia()
    {
        // chance base de usar item no turno
        if (Random.value > 0.55f) return null;

        bool vidaCritica = inimigo.GetVida() <= 30;
        bool manaBaixa = inimigo.GetMana() <= 25f;
        bool oponenteForte = player.GetVida() > 35;

        if (vidaCritica)
        {
            var cura = BuscarItemInimigoPorEfeito(EfeitoItemBatalha.CuraVida);
            if (cura != null) return cura;

            var escudo = BuscarItemInimigoPorEfeito(EfeitoItemBatalha.Escudo);
            if (escudo != null) return escudo;
        }

        if (manaBaixa)
        {
            var mana = BuscarItemInimigoPorEfeito(EfeitoItemBatalha.RecuperaMana);
            if (mana != null) return mana;
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

    private IEnumerator AtaquePlayer()
    {
        StopContador();
        verificadorDeTurno = false;
        DesabilitarBotoes();
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
        DesabilitarBotoes();
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
        if (batalhaFinalizada) return;

        if (!inimigo.VerificaVida())
        {
            batalhaFinalizada = true;
            StartCoroutine(TelaVitoria());
        }
        else if (!player.VerificaVida())
        {
            batalhaFinalizada = true;
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
            jogadorAgiu = true;
            StopContador();
            DesabilitarBotoes();
            StartCoroutine(AtaquePlayer());
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
}
