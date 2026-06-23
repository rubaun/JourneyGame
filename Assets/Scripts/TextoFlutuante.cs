using TMPro;
using UnityEngine;

public class TextoFlutuante : MonoBehaviour
{
    [SerializeField] private TextMeshPro texto;
    [SerializeField] private float velocidade = 1.2f;
    [SerializeField] private float duracao = 1.0f;

    public void Exibir(string mensagem, Color cor)
    {
        if (texto != null)
        {
            texto.text = mensagem;
            texto.color = cor;
        }

        Destroy(gameObject, duracao);
    }

    private void Update()
    {
        transform.position += Vector3.up * (velocidade * Time.deltaTime);
    }
}