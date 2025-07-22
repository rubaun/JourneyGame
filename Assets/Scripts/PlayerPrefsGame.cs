using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPrefsGame : MonoBehaviour
{
    [Header("Informações da Cena Atual")]
    [SerializeField] private string nomeCena;
    [SerializeField] private int nivelAtual;
    [SerializeField] private string nomeCenaAnterior;
    [SerializeField] private int nivelAnterior;
    [SerializeField] private bool cenaBatalha;
    [Header("Informações da Proxima Cena")]
    [SerializeField] private string nomeCenaProxima;




    void Start()
    {
        nomeCena = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("NomeCena", nomeCena);
        nivelAtual = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("NivelAtual", nivelAtual);
        PlayerPrefs.SetString("NomeCenaProxima", nomeCenaProxima);
        PlayerPrefs.Save();

        if (cenaBatalha)
        {
            PlayerPrefs.SetString("NomeCenaAnterior", nomeCena);
            PlayerPrefs.SetInt("NivelAnterior", nivelAtual);
            PlayerPrefs.Save();
        }
        else 
        {
           nomeCenaAnterior = PlayerPrefs.GetString("NomeCenaAnterior");
           nivelAnterior = PlayerPrefs.GetInt("NivelAnterior");
        }
    }

    public string GetCenaAnterior()
    {
        if (PlayerPrefs.HasKey("NomeCenaAnterior"))
        {
            nomeCenaAnterior = PlayerPrefs.GetString("NomeCenaAnterior");
            return nomeCenaAnterior;
        }
        return null;
    }

    public int GetNivelAnterior()
    {
        if (PlayerPrefs.HasKey("NivelAnterior"))
        {
            nivelAnterior = PlayerPrefs.GetInt("NivelAnterior");
            return nivelAnterior;
        }
        return 0;
    }

    

}
