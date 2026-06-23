using System.Collections;
using UnityEngine;

public class Mago : MonoBehaviour, IPersonagemBatalha
{
    [SerializeField] private string nomePersonagem;
    [SerializeField] private int vida;
    [SerializeField] private int ataque;
    [SerializeField] private int defesa;
    [SerializeField] private int especial;
    [SerializeField] private float mana;
    [SerializeField] private bool estahVivo = true;
    [SerializeField] private DiretorBatalhaMagia dB;
    [SerializeField] private Sprite spriteDerrota;
    [SerializeField] private bool ehHeroi;
    [SerializeField] private GameObject pDesefa;
    [SerializeField] private GameObject pSangrar;
    [SerializeField] private AudioClip[] somAtaque;
    [SerializeField] private AudioClip[] somDefesa;
    [SerializeField] private AudioClip[] somEspecial;
    [SerializeField] private AudioClip[] somErroAtaque;
    [SerializeField] private AudioClip[] somDano;
    [SerializeField] private AudioClip somVitoria;
    [SerializeField] private AudioClip somMorte;
    [SerializeField] private AudioClip somespecialPronto;
    [SerializeField] private GameObject cameraC;
    [SerializeField] private int turnosDefesaMax = 2;
    [SerializeField] private int vidaMax = 100;
    [SerializeField] private TextoFlutuante textoFlutuantePrefab;

    private Magias magias;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private SoundPlayer audioSource;
    private FalasPersonagem falasPersonagem;
    private bool defesaAtiva;
    private int defesaTemp;
    private int turnosDefesaRestantes;

    private int buffAtaque;
    private int buffDefesa;
    private int turnosBuffAtaque;
    private int turnosBuffDefesa;

    private void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundPlayer>();
        magias = GetComponent<Magias>();

        //Procura a camera
        if (cameraC == null)
        {
            cameraC = GameObject.FindGameObjectWithTag("MainCamera");
        }

        if (!falasPersonagem && !ehHeroi)
        {
            falasPersonagem = GetComponent<FalasPersonagem>();
        }
    }

    public bool EhMago()
    {
        return magias != null;
    }

    public Coroutine LancarMagiaAtaque()
    {
        if (magias != null)
            return StartCoroutine(magias.LancarMagiaAtaque());
        return null;
    }

    public Coroutine LancarMagiaEspecial()
    {
        if (magias != null)
            return StartCoroutine(magias.LancarMagiaEspecial());
        return null;
    }

    public void AtualizarDefesaPorTurno()
    {
        if (defesaAtiva && turnosDefesaRestantes > 0)
        {
            turnosDefesaRestantes--;
            if (turnosDefesaRestantes <= 0)
            {
                defesaAtiva = false;
                defesaTemp = 0;
                if (magias != null) magias.DesligarEfeitoDefesa();
                dB.RecebeTexto($"{nomePersonagem}: defesa expirou!");
            }
        }
    }

    public void FalaDoPersonagem(string tipoDeFala)
    {
        if (falasPersonagem != null && !ehHeroi && tipoDeFala == "Ataque")
        {
            falasPersonagem.FalaDeAtaque();
        }
        else if (falasPersonagem != null && !ehHeroi && tipoDeFala == "Defesa")
        {
            falasPersonagem.FalaDeDefesa();
        }
    }

    public string GetNomePersonagem()
    {
        return nomePersonagem;
    }

    public int GetVida()
    {
        return vida;
    }

    public bool VerificaVida()
    {
        return estahVivo;
    }

    public bool VerificaEspecial()
    {
        if (especial >= 3)
        {
            dB.RecebeTexto($"Especial carregado: {nomePersonagem}");
            audioSource.PlaySound(somespecialPronto);
            return true;
        }
        else
        {
            return false;
        }
    }

    public int ValorEspecial()
    {
        return especial;
    }

    public float GetMana()
    {
        return mana;
    }

    private void UseMana(int manaGasto)
    {
        if (mana - manaGasto >= 0)
        {
            mana -= manaGasto;
        }
    }

    public void RegeneraMana(float manaGanho)
    {
        mana = Mathf.Min(mana + manaGanho, 100f);
    }

    private void ExibirTextoFlutuante(string texto, Color cor)
    {
        if (textoFlutuantePrefab == null) return;
        TextoFlutuante t = Instantiate(textoFlutuantePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        t.Exibir(texto, cor);
    }

    public int AtaqueNormal()
    {
        int ataqueBase = Mathf.Max(1, ataque + buffAtaque);
        int valorAtaque = Random.Range(0, ataqueBase);

        especial++;

        // Magos não executam animação de ataque físico
        if (magias == null)
        {
            AnimaAtaque();
        }

        if (valorAtaque > 0 && valorAtaque <= mana)
        {
            UseMana(valorAtaque);
            dB.RecebeTexto($"{nomePersonagem} ataca: {valorAtaque}");
            PlaySomAtaque();
        }
        else if (valorAtaque <= 0)
        {
            dB.RecebeTexto($"{nomePersonagem} erra o ataque.");
            PlaySomErroAtaque();
            ExibirTextoFlutuante("errou", Color.gray);
        }
        else
        {
            dB.RecebeTexto($"{nomePersonagem}: sem mana suficiente.");
        }

        return valorAtaque;
    }

    private int CalcularEsquiva()
    {
        int defesaTotal = Mathf.Max(1, defesa + buffDefesa);
        int valorEsquiva = Random.Range(0, defesaTotal + (defesaTotal / 2));

        if (valorEsquiva > 0)
        {
            dB.RecebeTexto($"{nomePersonagem} esquiva: {valorEsquiva}");
        }
        else
        {
            dB.RecebeTexto($"{nomePersonagem}: não consegue esquivar.");
        }

        return valorEsquiva;
    }

    public int DefesaEsquiva()
    {
        int defesaTotal = Mathf.Max(1, defesa + buffDefesa);
        int valorDefesa = Random.Range(0, defesaTotal + (defesaTotal / 2));
        defesaTemp = valorDefesa;
            
        if (valorDefesa > 0 && mana >= valorDefesa)
        {
            defesaAtiva = true;
            turnosDefesaRestantes = turnosDefesaMax;
            UseMana(valorDefesa);
            StartCoroutine(RecarregarMana());
            if (magias != null) magias.LigarEfeitoDefesa();
            dB.RecebeTexto($"{nomePersonagem} carrega defesa: {valorDefesa}");
        }
        else if (valorDefesa > 0 && mana < valorDefesa)
        {
            defesaAtiva = false;
            StartCoroutine(RecarregarMana());
            dB.RecebeTexto($"{nomePersonagem}: Mana insuficiente!");
        }
        else
        {
            defesaAtiva = false;
            dB.RecebeTexto($"{nomePersonagem}: não consegue defender.");
            StartCoroutine(RecarregarMana());
            especial++;
        }


        return valorDefesa;
    }

    public bool DefensaAtiva()
    {
        if(defesaAtiva)
        {
            defesaAtiva = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public int Especial()
    {
        int ataqueTotal = Mathf.Max(1, ataque + buffAtaque);
        int valorEspecial = Random.Range((int)Mathf.Floor(ataqueTotal * 0.2f), ataqueTotal + (int)Mathf.Floor(ataqueTotal * 0.3f));
        int chanceDeDobrar = Random.Range(0, 100);
        int fatorMultiplicador = especial;

        // Magos não executam animação de ataque físico
        if (magias == null)
        {
            AnimaAtaque();
        }

        if (chanceDeDobrar >= 90 && especial >= 3 && mana == 100)
        {
            int valorEspecialDobrado = (valorEspecial * 2) + fatorMultiplicador;
            dB.RecebeTexto($"{nomePersonagem} MEGA ESPECIAL: {valorEspecialDobrado}");
            PlaySomEspecial();
            especial = 0;
            UseMana(100);
            return valorEspecialDobrado;
        }
        else if (chanceDeDobrar < 90 && especial >= 3 && mana >= valorEspecial)
        {
            dB.RecebeTexto($"{nomePersonagem} usa especial: {valorEspecial}");
            PlaySomAtaque();
            especial = 0;
            UseMana(valorEspecial);
            return valorEspecial;
        }
        else if(especial >= 3 && mana <= valorEspecial)
        {
            dB.RecebeTexto("Mana insuficiente!");
            return 0;
        }
        else
        {
            dB.RecebeTexto("Especial não esta carregado!");
            return 0;
        }
    }

    public void LevarDano(int dano)
    { 
        int danoFinal;

        if (defesaAtiva)
        {
            danoFinal = dano - defesaTemp;

            // Escudo só quebra se o dano for maior que a defesa carregada
            if (danoFinal > 0)
            {
                defesaAtiva = false;
                turnosDefesaRestantes = 0;
                if (magias != null) magias.DesligarEfeitoDefesa();
            }
        }
        else
        {
            // Esquiva passiva — SEM ativar escudo ou efeito de defesa
            danoFinal = dano - CalcularEsquiva();
        }
           
         
        if (danoFinal <= 0)
        {
            StartCoroutine(TocarDefesa());
        }
        else if (danoFinal <= 25)
        {
            StartCoroutine(TocarDanoNormal(danoFinal));
        }
        else
        {
            StartCoroutine(TocarDanoMaximo(danoFinal));
        }

        if (estahVivo)
        {
            Debug.Log($"{nomePersonagem}, vida: {vida}");
        }
        else
        {
            dB.RecebeTexto($"{nomePersonagem}, morreu!");
        }

    }
    
    private void DefineVida()
    {
        if (vida <= 0)
        {
            spriteRenderer.sprite = spriteDerrota;
            vida = 0;
            estahVivo = false;
        }
    }

    private void AnimaAtaque()
    {
        if (ehHeroi)
        {
            anim.SetTrigger("AtaqueInimigo");
        }
        else
        {
            anim.SetTrigger("AtaqueHeroi");
        }
    }

    private void PlaySomDano()
    {
        int som = Random.Range(0, somDano.Length);
        audioSource.PlaySound(somDano[som]);
    }

    private void ParticulaDefesa()
    {
        pDesefa.GetComponent<ParticleSystem>().Play();
    }

    private void ParticulaSangrar()
    {
        pSangrar.GetComponent<ParticleSystem>().Play();
    }

    private void PlaySomAtaque()
    {
        int som = Random.Range(0, somAtaque.Length);
        audioSource.PlaySound(somAtaque[som]);
    }
    private void PlaySomErroAtaque()
    {
        int som = Random.Range(0, somErroAtaque.Length);
        audioSource.PlaySound(somErroAtaque[som]);
    }

    private void PlaySomDefesa()
    {
        int som = Random.Range(0, somDefesa.Length);
        audioSource.PlaySound(somDefesa[som]);
    }

    private void PlaySomEspecial()
    {
        int som = Random.Range(0, somEspecial.Length);
        audioSource.PlaySound(somEspecial[som]);
    }

    public void PlaySomMorte()
    {
        audioSource.PlaySound(somMorte);
    }

    public void PlaySomVitoria()
    {
        audioSource.PlaySound(somVitoria);
    }

    IEnumerator TocarDefesa()
    {
        dB.RecebeTexto($"{nomePersonagem}: defende!");
        anim.SetTrigger("Defesa");
        yield return new WaitForSeconds(0.5f);
        PlaySomDefesa();
        ParticulaDefesa();
        ExibirTextoFlutuante("defesa", Color.cyan);
    }

    IEnumerator TocarDanoNormal(int danoFinal)
    {
        dB.RecebeTexto($"{nomePersonagem}, dano: {danoFinal}.");
        anim.SetTrigger("Dano");
        yield return new WaitForSeconds(0.5f);
        PlaySomDano();
        ParticulaSangrar();
        vida -= danoFinal;
        DefineVida();
        ExibirTextoFlutuante($"-{danoFinal}", Color.red);
    }

    IEnumerator TocarDanoMaximo(int danoFinal)
    {
        dB.RecebeTexto($"{nomePersonagem}, dano crítico: {danoFinal}.");
        anim.SetTrigger("Dano");
        yield return new WaitForSeconds(0.5f);
        CameraTreme(0.5f);
        PlaySomDano();
        ParticulaSangrar();
        vida -= danoFinal;
        DefineVida();

        ExibirTextoFlutuante($"-{danoFinal}", Color.red);
        yield return new WaitForSeconds(0.2f); // pequeno delay entre mensagens
        ExibirTextoFlutuante("crítico", Color.yellow);
    }

    private void CameraTreme(float magnitude)
    {
        audioSource.PlaySound(somVitoria);
        cameraC.GetComponent<CameraShake>().ShakeCamera(0.25f, 0.15f);
    }

    IEnumerator RecarregarMana()
    {
        while (mana < 100)
        {
            yield return new WaitForSeconds(1f);
            RegeneraMana(1.5f);
        }
    }

    public string NomePersonagem => nomePersonagem;
public bool EstaVivo => estahVivo;

public void CurarVida(int valor)
{
    if (valor <= 0 || !estahVivo) return;
    vida = Mathf.Min(vida + valor, vidaMax);
}

public void RecuperarMana(float valor)
{
    if (valor <= 0f) return;
    mana = Mathf.Min(mana + valor, 100f);
}

public void AplicarBuffAtaque(int valor, int turnos)
{
    buffAtaque += valor;
    turnosBuffAtaque = Mathf.Max(turnosBuffAtaque, turnos);
}

public void AplicarBuffDefesa(int valor, int turnos)
{
    buffDefesa += valor;
    turnosBuffDefesa = Mathf.Max(turnosBuffDefesa, turnos);
}

public void AtivarEscudoItem(int valorEscudo, int turnos)
{
    defesaAtiva = true;
    defesaTemp = valorEscudo;
    turnosDefesaRestantes = Mathf.Max(1, turnos);
    if (magias != null) magias.LigarEfeitoDefesa();
}

public void AtualizarEfeitosPorTurno()
{
    AtualizarDefesaPorTurno();

    if (turnosBuffAtaque > 0 && --turnosBuffAtaque == 0) buffAtaque = 0;
    if (turnosBuffDefesa > 0 && --turnosBuffDefesa == 0) buffDefesa = 0;
}

public void MostrarTextoAcao(string texto)
{
    ExibirTextoFlutuante(texto, Color.yellow);
}
}
