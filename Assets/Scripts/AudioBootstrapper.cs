using UnityEngine;

public static class AudioBootstrapper
{
    public static void GarantirAudioManager(GameObject audioManagerPrefab)
    {
        SoundPlayer existente = Object.FindObjectOfType<SoundPlayer>();
        if (existente != null) return;

        if (audioManagerPrefab == null)
        {
            Debug.LogWarning("AudioBootstrapper: prefab do AudioManager não foi definido.");
            return;
        }

        Object.Instantiate(audioManagerPrefab);
    }
}