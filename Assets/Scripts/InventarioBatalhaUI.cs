using UnityEngine;
using UnityEngine.UI;

public class InventarioBatalhaUI : MonoBehaviour
{
    [Header("Referências UI")]
    [SerializeField] private GameObject painelInventario;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform content;
    [SerializeField] private ItemSlotBatalhaUI slotPrefab;

    [Header("Dados")]
    [SerializeField] private InventarioBatalha inventarioPlayer;

    [Header("Diretores (preencha um ou ambos)")]
    [SerializeField] private DiretorBatalha diretorSemMagia;
    [SerializeField] private DiretorBatalhaMagia diretorMagia;

    [Header("Integração com menu")]
    [SerializeField] private Button botaoConfiguracoes;
    [SerializeField] private GameObject painelConfiguracoes;

    [SerializeField] private bool fecharAposUsar = true;

    private bool pausadoPorInventario;

    private void Awake()
    {
        ResolverReferenciasCena();
        ResolverReferenciasScroll();
    }

    private void Start()
    {
        if (painelInventario != null)
            painelInventario.SetActive(false);

        // segurança: evita cena ficar pausada indevidamente
        Time.timeScale = 1f;
        pausadoPorInventario = false;
    }

    private void OnDisable()
    {
        if (pausadoPorInventario)
            RetomarJogo();
    }

    private void ResolverReferenciasCena()
    {
        if (inventarioPlayer == null)
            inventarioPlayer = GetComponent<InventarioBatalha>();

        if (inventarioPlayer == null)
            inventarioPlayer = FindObjectOfType<InventarioBatalha>();

        if (diretorSemMagia == null)
            diretorSemMagia = FindObjectOfType<DiretorBatalha>();

        if (diretorMagia == null)
            diretorMagia = FindObjectOfType<DiretorBatalhaMagia>();
    }

    private void ResolverReferenciasScroll()
    {
        if (scrollRect == null && painelInventario != null)
            scrollRect = painelInventario.GetComponentInChildren<ScrollRect>(true);

        if (scrollRect == null) return;

        if (scrollRect.viewport == null)
        {
            Transform viewport = scrollRect.transform.Find("Viewport");
            if (viewport != null)
                scrollRect.viewport = viewport as RectTransform;
        }

        if (scrollRect.content == null && scrollRect.viewport != null)
        {
            Transform contentEncontrado = scrollRect.viewport.Find("Content");
            if (contentEncontrado != null)
                scrollRect.content = contentEncontrado as RectTransform;
        }

        if (scrollRect.content != null)
            content = scrollRect.content;
    }

    public void AlternarPainel()
    {
        if (painelInventario == null) return;

        if (painelInventario.activeSelf) FecharPainel();
        else AbrirPainel();
    }

    private void AbrirPainel()
    {
        painelInventario.SetActive(true);
        AtualizarLista();
        PausarJogo();
        BloquearConfiguracoes(true);
    }

    public void FecharPainel()
    {
        if (painelInventario != null)
            painelInventario.SetActive(false);

        RetomarJogo();
        BloquearConfiguracoes(false);
    }

    private void PausarJogo()
    {
        Time.timeScale = 0f;
        pausadoPorInventario = true;
    }

    private void RetomarJogo()
    {
        Time.timeScale = 1f;
        pausadoPorInventario = false;
    }

    private void BloquearConfiguracoes(bool bloquear)
    {
        if (botaoConfiguracoes != null)
            botaoConfiguracoes.interactable = !bloquear;

        if (bloquear && painelConfiguracoes != null && painelConfiguracoes.activeSelf)
            painelConfiguracoes.SetActive(false);
    }

    public void AtualizarLista()
    {
        ResolverReferenciasCena();
        ResolverReferenciasScroll();

        if (content == null || slotPrefab == null || inventarioPlayer == null)
        {
            Debug.LogWarning("InventarioBatalhaUI: referência ausente (content/slotPrefab/inventarioPlayer).");
            return;
        }

        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Transform filho = content.GetChild(i);
            if (filho.GetComponent<ItemSlotBatalhaUI>() != null)
                Destroy(filho.gameObject);
        }

        var slots = inventarioPlayer.ObterSlots();
        for (int i = 0; i < slots.Count; i++)
        {
            // Segurança extra: não renderiza item zerado
            if (slots[i] == null || slots[i].item == null || slots[i].quantidade <= 0)
                continue;

            GameObject slotGO = Instantiate(slotPrefab.gameObject);
            slotGO.transform.SetParent(content, false);

            ItemSlotBatalhaUI slotUI = slotGO.GetComponent<ItemSlotBatalhaUI>();
            if (slotUI != null)
                slotUI.Configurar(slots[i], OnItemSelecionado);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content as RectTransform);
    }

    private void OnItemSelecionado(ItemBatalhaData item)
    {
        if (item == null) return;

        bool aplicado = false;

        if (diretorMagia != null)
        {
            diretorMagia.UsarItemPlayer(item);
            aplicado = true;
        }

        if (diretorSemMagia != null)
        {
            diretorSemMagia.UsarItemPlayer(item);
            aplicado = true;
        }

        if (!aplicado)
            Debug.LogWarning("InventarioBatalhaUI: nenhum diretor de batalha encontrado na cena.");

        AtualizarLista();

        if (fecharAposUsar)
            FecharPainel();
    }
}