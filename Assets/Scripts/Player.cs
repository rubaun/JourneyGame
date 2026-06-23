using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IPersonagemBatalha
{
    [SerializeField] private int vidaMax = 100;
    [SerializeField] private TextoFlutuante textoFlutuantePrefab;

    private int buffAtaque;
    private int buffDefesa;
    private int turnosBuffAtaque;
    private int turnosBuffDefesa;

    public string NomePersonagem => nomePersonagem;
    public bool EstaVivo => estahVivo;

    public void CurarVida(int valor)
    {
        if (valor <= 0 || !estahVivo) return;
        vida = Mathf.Min(vida + valor, vidaMax);
    }

    public void RecuperarMana(float valor)
    {
        // Player sem mana: manter sem efeito
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
        AplicarBuffDefesa(valorEscudo, turnos);
    }

    public void AtualizarEfeitosPorTurno()
    {
        if (turnosBuffAtaque > 0 && --turnosBuffAtaque == 0) buffAtaque = 0;
        if (turnosBuffDefesa > 0 && --turnosBuffDefesa == 0) buffDefesa = 0;
    }

    private void ExibirTextoFlutuante(string texto, Color cor)
    {
        if (textoFlutuantePrefab == null) return;
        TextoFlutuante t = Instantiate(textoFlutuantePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        t.Exibir(texto, cor);
    }

    public void MostrarTextoAcao(string texto)
    {
        ExibirTextoFlutuante(texto, Color.yellow);
    }

    [SerializeField] private string nomePersonagem;
    [SerializeField] private int vida;
    [SerializeField] private int ataque;
    [SerializeField] private int defesa;
    [SerializeField] private int especial;
    [SerializeField] private bool estahVivo = true;
    [SerializeField] private DiretorBatalha dB;
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
    [SerializeField] private AudioClip somEspecialPronto;
    [SerializeField] private GameObject cameraC;

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private SoundPlayer audioSource;
    private FalasPersonagem falasPersonagem;

    private void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundPlayer>();

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

    public void FalaDoPersonagem(string tipoDeFala)
    {
        if(falasPersonagem != null && !ehHeroi && tipoDeFala == "Ataque")
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

    public bool VerificaVida() //retorna true se o jogador esta vivo e false se esta morto
    {
        return estahVivo;
    }

    public bool VerificaEspecial() //retorna true se o jogador tem especial e false se nao tem
    {
        if (especial >= 3)
        {
            dB.RecebeTexto($"{nomePersonagem} especial pronto!");
            audioSource.PlaySound(somEspecialPronto);
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

    public int Ataque()
    {
        int ataqueTotal = Mathf.Max(1, ataque + buffAtaque);
        int valorAtaque = Random.Range(0,ataqueTotal);

        especial++;

        AnimaAtaque();

        if (valorAtaque > 0)
        {
            FalaDoPersonagem("Ataque");
            dB.RecebeTexto($"{nomePersonagem} ataca com {valorAtaque}");
            PlaySomAtaque();
        }
        else
        {
            dB.RecebeTexto($"{nomePersonagem} erra o ataque.");
            PlaySomErroAtaque();
            ExibirTextoFlutuante("errou", Color.gray);
        }


        return valorAtaque;
    }

    public int Defesa()
    {
        int defesaTotal = Mathf.Max(1, defesa + buffDefesa);
        int valorDefesa = Random.Range(0, defesaTotal);

        if(valorDefesa > 0)
        {
            FalaDoPersonagem("Defesa");
            dB.RecebeTexto($"{nomePersonagem} defende com {valorDefesa}");
        }
        else
        {
            dB.RecebeTexto($"{nomePersonagem} nao consegue defender.");
        }
        

        return valorDefesa;
    }

    public int Especial()
    {
        int valorEspecial = Random.Range(20, ataque);
        int chanceDeDobrar = Random.Range(0, 100);
        int fatorMultiplicador = especial;

        AnimaAtaque();

        if (chanceDeDobrar >= 90 && especial >= 3)
        {
            int valorEspecialDobrado = (valorEspecial * 2) + fatorMultiplicador;
            FalaDoPersonagem("Ataque");
            dB.RecebeTexto($"{nomePersonagem} ataca com {valorEspecialDobrado}");
            PlaySomEspecial();
            especial = 0;
            return valorEspecialDobrado;
        }
        else if (chanceDeDobrar < 90 && especial >= 3)
        {
            FalaDoPersonagem("Ataque");
            dB.RecebeTexto($"{nomePersonagem} ataca com {valorEspecial}");
            PlaySomAtaque();
            especial = 0;
            return valorEspecial;
        }
        else
        {
            dB.RecebeTexto("Seu especial nao esta carregado!");
            return 0;
        }
    }

    public void LevarDano(int dano)
    {
        int danoFinal = dano - Defesa();

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
    private void DefineVida() //Verifica o valor da vida e define como morto
    {
        if (vida <= 0)
        {
            spriteRenderer.sprite = spriteDerrota;
            vida = 0;
            estahVivo = false; //Ta morto
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
        dB.RecebeTexto($"{nomePersonagem} consegue se defender!");
        anim.SetTrigger("Defesa");
        yield return new WaitForSeconds(0.5f);
        PlaySomDefesa();
        ParticulaDefesa();
        ExibirTextoFlutuante("defesa", Color.cyan);
    }

    IEnumerator TocarDanoNormal(int danoFinal)
    {
        dB.RecebeTexto($"{nomePersonagem} leva dano de {danoFinal}.");
        anim.SetTrigger("Dano");
        yield return new WaitForSeconds(0.5f);
        PlaySomDano();
        ParticulaSangrar();
        vida -= danoFinal; //vida = vida - danoFinal;
        DefineVida();
        ExibirTextoFlutuante($"-{danoFinal}", Color.red);
    }

    IEnumerator TocarDanoMaximo(int danoFinal)
    {
        dB.RecebeTexto($"{nomePersonagem} toma uma porrada de {danoFinal}.");
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
}
