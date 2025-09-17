using UnityEngine;

public class Magias : MonoBehaviour
{
    [SerializeField] private string nomeMagiaAtaque;
    [SerializeField] private int danoMagiaAtaque;
    [SerializeField] private int custoManaMagiaAtaque;
    [SerializeField] private GameObject efeitoMagiaAtaque;
    [SerializeField] private GameObject efeitoMagiaAtaqueArea;
    [SerializeField] private string nomeMagiaDefesa;
    [SerializeField] private int defesaMagiaDefesa;
    [SerializeField] private int custoManaMagiaDefesa;
    [SerializeField] private GameObject efeitoMagiaDefesaArea;
    [SerializeField] private string nomeMagiaEspecial;
    [SerializeField] private int danoMagiaEspecial;
    [SerializeField] private int custoManaMagiaEspecial;
    [SerializeField] private GameObject efeitoMagiaEspecial;
    [SerializeField] private GameObject efeitoMagiaEspecialArea;
    [SerializeField] private GameObject mira;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LigarEfeitoDefesa()
    {
        if (efeitoMagiaDefesaArea != null)
        {
            efeitoMagiaDefesaArea.GetComponent<ParticleSystem>().Play();
        }
    }

    public void DesligarEfeitoDefesa()
    {
        if (efeitoMagiaDefesaArea != null)
        {
            efeitoMagiaDefesaArea.GetComponent<ParticleSystem>().Stop();
        }
    }

    private void LigarEfeitoArcano()
    {
        if (efeitoMagiaEspecialArea != null)
        {
            efeitoMagiaEspecialArea.GetComponent<ParticleSystem>().Play();
        }
    }

    public void LancarPoderArcado()
    {
        //Procurar o inimigo na cena
        GameObject inimigo = GameObject.FindGameObjectWithTag("Inimigo");

        if (efeitoMagiaAtaque != null && mira != null)
        {
            LigarEfeitoArcano();
            //Instanciar o efeito da magia no inimigo
            GameObject efeito = Instantiate(efeitoMagiaAtaque, mira.transform.position, Quaternion.identity);
            efeito.GetComponent<ParticleSystem>().Play();
            Destroy(efeito, 2f);
        }
    }

    public void LigarEfeitoEspecial()
    {
        if (efeitoMagiaEspecialArea != null)
        {
            efeitoMagiaEspecialArea.GetComponent<ParticleSystem>().Play();
        }
    }

    public void DesligarEfeitoEspecial()
    {
        if (efeitoMagiaEspecialArea != null)
        {
            efeitoMagiaEspecialArea.GetComponent<ParticleSystem>().Stop();
        }
    }
}
