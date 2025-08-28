using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Configuracao : MonoBehaviour
{
    private Slider somSlider;
    private Slider musicaSlider;
    private Slider narrativaSlider;
    private Toggle caixaMensagem;
    private Toggle efeitosVisuais;
    private int efeitosVisuaisAtivo;
    private GameObject caixaMensagemObj;
    private SoundPlayer soundPlayer;
    private Camera Camera;

    private void Awake()
    {
        somSlider = GameObject.Find("SomVolume").GetComponent<Slider>();
        musicaSlider = GameObject.Find("MusicaVolume").GetComponent<Slider>();
        narrativaSlider = GameObject.Find("NarrativaVolume").GetComponent<Slider>();
        soundPlayer = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundPlayer>();
        Camera = Camera.main;

        if (somSlider != null)
        {
            somSlider.onValueChanged.AddListener(soundPlayer.SetSomVolume);
            somSlider.value = soundPlayer.GetSomVolume();
        }

        if (musicaSlider != null)
        {
            musicaSlider.onValueChanged.AddListener(soundPlayer.SetMusicaVolume);
            musicaSlider.value = soundPlayer.GetMusicaVolume();
        }

        if (narrativaSlider != null)
        {
            narrativaSlider.onValueChanged.AddListener(soundPlayer.SetNarrativaVolume);
            narrativaSlider.value = soundPlayer.GetNarrativaVolume();
        }

        if(SceneManager.GetActiveScene().name != "Configuracoes")
        {
            caixaMensagem = GameObject.Find("InformativoAtiva").GetComponent<Toggle>();
            caixaMensagemObj = GameObject.Find("FundoInformativo");

            caixaMensagemObj.SetActive(caixaMensagem.isOn);
        }
        

        if (caixaMensagem != null)
        {
            caixaMensagem.onValueChanged.AddListener(delegate { AlternarCaixaMensagem(); });
        }

        efeitosVisuais = GameObject.Find("EfeitosCheck").GetComponent<Toggle>();

        if (efeitosVisuais != null)
        {
            efeitosVisuais.onValueChanged.AddListener(delegate { AtivarDesativarEfeitos(); });
        }
    }

    public void SalvarConfig()
    {
        soundPlayer.SaveVolumes();
    }

    // Método para alternar a visibilidade da caixa de mensagem
    public void AlternarCaixaMensagem()
    {
        if (caixaMensagemObj != null)
        {
            caixaMensagemObj.SetActive(caixaMensagem.isOn);
        }
    }

    public void AtivarDesativarEfeitos()
    {
        if (efeitosVisuais != null)
        {
            var cameraData = Camera.GetComponent<UniversalAdditionalCameraData>();

            if (cameraData != null && cameraData.renderPostProcessing == true)
            {
                cameraData.renderPostProcessing = false;
                PlayerPrefs.SetInt("EfeitosVisuais", 0);
                PlayerPrefs.Save();
            }
            else if (cameraData != null && cameraData.renderPostProcessing == false)
            {
                cameraData.renderPostProcessing = true;
                PlayerPrefs.SetInt("EfeitosVisuais", 1);
                PlayerPrefs.Save();
            }
        }
    }


}
