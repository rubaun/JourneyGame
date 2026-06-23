using System.Collections;
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
    [SerializeField] private float velocidadeProjetil = 10f;
    [SerializeField] private float tempoEfeitoArea = 2f;

    public void LigarEfeitoDefesa()
    {
        if (efeitoMagiaDefesaArea != null)
        {
            efeitoMagiaDefesaArea.SetActive(true);
            efeitoMagiaDefesaArea.GetComponent<ParticleSystem>().Play();
        }
    }

    public void DesligarEfeitoDefesa()
    {
        if (efeitoMagiaDefesaArea != null)
        {
            efeitoMagiaDefesaArea.GetComponent<ParticleSystem>().Stop();
            efeitoMagiaDefesaArea.SetActive(false);
        }
    }

    public IEnumerator LancarMagiaAtaque()
    {
        if (efeitoMagiaAtaque == null || mira == null) yield break;

        // Instancia o projétil apontando para a mira
        GameObject projetil = Instantiate(efeitoMagiaAtaque, transform.position, Quaternion.identity);
        ApontarParaAlvo(projetil, transform.position);

        ParticleSystem ps = projetil.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        // Move até o alvo
        while (Vector3.Distance(projetil.transform.position, mira.transform.position) > 0.1f)
        {
            projetil.transform.position = Vector3.MoveTowards(
                projetil.transform.position,
                mira.transform.position,
                velocidadeProjetil * Time.deltaTime);
            yield return null;
        }

        // Projétil chegou: instancia efeito de área no oponente atingido
        InstanciarEfeitoArea(efeitoMagiaAtaqueArea);
        Destroy(projetil, 0.5f);
    }

    public IEnumerator LancarMagiaEspecial()
    {
        if (efeitoMagiaEspecial == null || mira == null) yield break;

        // Instancia o projétil apontando para a mira
        GameObject projetil = Instantiate(efeitoMagiaEspecial, transform.position, Quaternion.identity);
        ApontarParaAlvo(projetil, transform.position);

        ParticleSystem ps = projetil.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        // Move até o alvo
        while (Vector3.Distance(projetil.transform.position, mira.transform.position) > 0.1f)
        {
            projetil.transform.position = Vector3.MoveTowards(
                projetil.transform.position,
                mira.transform.position,
                velocidadeProjetil * Time.deltaTime);
            yield return null;
        }

        // Projétil chegou: instancia efeito de área especial no oponente atingido
        InstanciarEfeitoArea(efeitoMagiaEspecialArea);
        Destroy(projetil, 0.5f);
    }

    private void ApontarParaAlvo(GameObject projetil, Vector3 origem)
    {
        if (mira == null || projetil == null) return;
        Vector3 direcao = (mira.transform.position - origem).normalized;
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        projetil.transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    private void InstanciarEfeitoArea(GameObject efeitoPrefab)
    {
        if (efeitoPrefab == null || mira == null) return;
        GameObject efeitoArea = Instantiate(efeitoPrefab, mira.transform.position, Quaternion.identity);
        ParticleSystem psArea = efeitoArea.GetComponent<ParticleSystem>();
        if (psArea != null) psArea.Play();
        Destroy(efeitoArea, tempoEfeitoArea);
    }
}
