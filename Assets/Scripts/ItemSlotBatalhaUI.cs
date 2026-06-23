using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotBatalhaUI : MonoBehaviour
{
    [SerializeField] private Image icone;
    [SerializeField] private TextMeshProUGUI nomeItem;
    [SerializeField] private TextMeshProUGUI quantidade;
    [SerializeField] private Button botaoUsar;

    private ItemBatalhaData itemAtual;
    private Action<ItemBatalhaData> onClick;

    public void Configurar(SlotItemBatalha slot, Action<ItemBatalhaData> callback)
    {
        itemAtual = slot != null ? slot.item : null;
        onClick = callback;

        bool valido = slot != null && slot.item != null;

        if (!valido)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        nomeItem.text = slot.item.nomeExibicao;
        quantidade.text = "x" + slot.quantidade;

        if (icone != null)
        {
            icone.sprite = slot.item.icone;
            icone.enabled = slot.item.icone != null;
        }

        bool podeUsar = slot.quantidade > 0 && slot.item.utilizavelEmBatalha;
        botaoUsar.interactable = podeUsar;

        botaoUsar.onClick.RemoveAllListeners();
        botaoUsar.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        if (itemAtual == null) return;
        onClick?.Invoke(itemAtual);
    }
}