using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FalasPersonagem : MonoBehaviour
{
    [Header("Referências UI")]
    [SerializeField] private TextMeshProUGUI falaTexto;
    [SerializeField] private Animator balaoAnimator;
    [SerializeField] private Animator textoAnimator;

    [Header("Tempos (segundos)")]
    [SerializeField] private float atrasoFalaInicial = 0.8f;
    [SerializeField] private float duracaoFalaInicial = 2.8f;
    [SerializeField] private float atrasoFalaAtaque = 0.15f;
    [SerializeField] private float duracaoFalaAtaque = 1.8f;
    [SerializeField] private float atrasoFalaDefesa = 0.05f;
    [SerializeField] private float duracaoFalaDefesa = 1.6f;

    [Header("Falas do Personagem")]
    [Header("0 = Inicial | 1-3 = Defesa | 4-6 = Ataque")]
    [SerializeField] private List<string> falas = new List<string>();

    private Coroutine falaAtual;

    private void Start()
    {
        if (falas.Count > 0)
            IniciarFala(falas[0], atrasoFalaInicial, duracaoFalaInicial);
    }

    public void FalaDeAtaque()
    {
        string texto = ObterFalaAleatoria(4, 7); // 4..6
        if (!string.IsNullOrWhiteSpace(texto))
            IniciarFala(texto, atrasoFalaAtaque, duracaoFalaAtaque);
    }

    public void FalaDeDefesa()
    {
        string texto = ObterFalaAleatoria(1, 4); // 1..3
        if (!string.IsNullOrWhiteSpace(texto))
            IniciarFala(texto, atrasoFalaDefesa, duracaoFalaDefesa);
    }

    private void IniciarFala(string texto, float atraso, float duracao)
    {
        if (falaAtual != null)
            StopCoroutine(falaAtual);

        falaAtual = StartCoroutine(RotinaFala(texto, atraso, duracao));
    }

    private IEnumerator RotinaFala(string texto, float atraso, float duracao)
    {
        if (falaTexto == null || balaoAnimator == null || textoAnimator == null)
            yield break;

        if (atraso > 0f)
            yield return new WaitForSeconds(atraso);

        falaTexto.text = texto;

        // Garante texto antes da animação de abrir
        yield return null;

        balaoAnimator.ResetTrigger("CalaBoca");
        textoAnimator.ResetTrigger("CalaBoca");
        balaoAnimator.SetTrigger("FalaAgora");
        textoAnimator.SetTrigger("FalaAgora");

        yield return new WaitForSeconds(duracao);

        balaoAnimator.ResetTrigger("FalaAgora");
        textoAnimator.ResetTrigger("FalaAgora");
        balaoAnimator.SetTrigger("CalaBoca");
        textoAnimator.SetTrigger("CalaBoca");

        falaTexto.text = "";
        falaAtual = null;
    }

    private string ObterFalaAleatoria(int inicioInclusivo, int fimExclusivo)
    {
        if (falas == null || falas.Count == 0) return null;

        int inicio = Mathf.Clamp(inicioInclusivo, 0, falas.Count);
        int fim = Mathf.Clamp(fimExclusivo, 0, falas.Count);

        if (fim <= inicio) return null;

        return falas[Random.Range(inicio, fim)];
    }
}