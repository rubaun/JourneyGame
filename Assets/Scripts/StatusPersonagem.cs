using UnityEngine;
using UnityEngine.SceneManagement;

public class StatusPersonagem : MonoBehaviour
{
    [Header("Informações do Personagem")]
    [SerializeField] private string nomeP;
    [SerializeField] private int vidaP;
    [SerializeField] private int ataqueP;
    [SerializeField] private int defesaP;
    [SerializeField] private string especialP;
    [SerializeField] private string classeP;
    [SerializeField] private string descritoP;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "Paladino")
        {
            nomeP = "Anduino";
            vidaP = 100;
            ataqueP = 40;
            defesaP = 75;
            especialP = "Julgamento da Luz Celeste";
            classeP = "Paladino";
            descritoP = "Paladinos são guerreiros sagrados que unem força e fé, lutando em nome da justiça, da honra e da luz, jurando proteger os inocentes e combater as forças do mal a qualquer custo.";
            EncontrarCamposTexto();
        }
        else if(SceneManager.GetActiveScene().name == "Mago")
        {
            nomeP = "Edgar";
            vidaP = 60;
            ataqueP = 80;
            defesaP = 30;
            especialP = "Chama da Realidade";
            classeP = "Mago";
            descritoP = "O mago é o mestre do arcano, moldando a realidade com o poder das palavras e da vontade, onde cada feitiço é um fio de conhecimento entre o mundo visível e o invisível.";
            EncontrarCamposTexto();
        }
        else if(SceneManager.GetActiveScene().name == "Druida")
        {
            nomeP = "Sylvaran";
            vidaP = 80;
            ataqueP = 50;
            defesaP = 50;
            especialP = "Raízes do Mundo Antigo";
            classeP = "Druida";
            descritoP = "O Druida representa a harmonia entre a natureza e a magia, canalizando o poder da terra para curar aliados, controlar o ambiente e invocar a fúria selvagem contra seus inimigos.";
            EncontrarCamposTexto();
        }
    }

    private void EncontrarCamposTexto()
    {
        GameObject.Find("NomeP").GetComponent<TMPro.TextMeshProUGUI>().text = "Nome: " + nomeP;
        GameObject.Find("VidaP").GetComponent<TMPro.TextMeshProUGUI>().text = "Vida: " + vidaP.ToString();
        GameObject.Find("AtaqueP").GetComponent<TMPro.TextMeshProUGUI>().text = "Ataque: " + ataqueP.ToString();
        GameObject.Find("DefesaP").GetComponent<TMPro.TextMeshProUGUI>().text = "Defesa: " + defesaP.ToString();
        GameObject.Find("EspecialP").GetComponent<TMPro.TextMeshProUGUI>().text = "Especial: " + especialP;
        GameObject.Find("ClasseP").GetComponent<TMPro.TextMeshProUGUI>().text = "Classe: " + classeP;
        GameObject.Find("DescritoP").GetComponent<TMPro.TextMeshProUGUI>().text = descritoP;
    }
}
