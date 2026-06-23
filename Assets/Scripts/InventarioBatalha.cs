using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotItemBatalha
{
    public ItemBatalhaData item;
    public int quantidade;
}

public class InventarioBatalha : MonoBehaviour
{
    [SerializeField] private List<SlotItemBatalha> slots = new List<SlotItemBatalha>();

    public IReadOnlyList<SlotItemBatalha> ObterSlots()
    {
        return slots;
    }

    public bool TentarConsumir(ItemBatalhaData item)
    {
        if (item == null) return false;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item && slots[i].quantidade > 0)
            {
                slots[i].quantidade--;

                // Quando acabar, remove o slot do inventário
                if (slots[i].quantidade <= 0)
                {
                    slots.RemoveAt(i);
                }

                return true;
            }
        }

        return false;
    }

    public bool TemItemUtilizavelEmBatalha()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item != null &&
                slots[i].item.utilizavelEmBatalha &&
                slots[i].quantidade > 0)
            {
                return true;
            }
        }
        return false;
    }

    public ItemBatalhaData ObterItemUtilizavelAleatorio()
    {
        List<ItemBatalhaData> candidatos = new List<ItemBatalhaData>();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item != null &&
                slots[i].item.utilizavelEmBatalha &&
                slots[i].quantidade > 0)
            {
                candidatos.Add(slots[i].item);
            }
        }

        if (candidatos.Count == 0) return null;
        return candidatos[Random.Range(0, candidatos.Count)];
    }
}