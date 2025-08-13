using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class EscreveTexto : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI texto; // Referência ao componente TextMeshProUGUI
    [TextArea] // Campo de texto para editar a mensagem completa no Inspector
    [SerializeField] private string mensagemCompleta; // Texto completo a ser exibido
    [SerializeField] private float velocidadeDigitacao = 0.08f; // Tempo entre letras
    [SerializeField] AudioClip narrativa;
    private SoundPlayer tocadorSom;


    void Start()
    {
        tocadorSom = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundPlayer>(); // Obtém o componente AudioSource
        texto.text = ""; // Limpa o texto atual
        StartCoroutine(DigitarTexto());     
    }

    private IEnumerator DigitarTexto() // Coroutina para digitar o texto
    {
        if (narrativa != null)
        {
            tocadorSom.PlayNarrativa(narrativa);
        }

        foreach (char letra in mensagemCompleta) // Itera sobre cada letra da mensagem
        {
            texto.text += letra;
            yield return new WaitForSeconds(velocidadeDigitacao); // Espera antes de escrever a próxima letra
        }
    }

}
