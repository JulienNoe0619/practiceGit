using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAudioSequencer : MonoBehaviour
{
    // Structure personnalisée pour mimer un dictionnaire dans l'Inspecteur
    [Serializable]
    public struct SequenceStep
    {
        [Tooltip("Nom de l'étape pour s'y retrouver dans l'inspecteur")]
        public string stepName;

        [Tooltip("Le clip audio à jouer")]
        public AudioClip audioClip;

        [Tooltip("Le nom du Trigger dans l'Animator")]
        public string animatorTrigger;

        [Tooltip("Le temps d'attente AVANT de passer à l'étape suivante (en secondes)")]
        public float delayBeforeNext;
    }

    [Header("References")]
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private AudioSource audioSource;

    [Header("Sequence Settings")]
    [SerializeField] private List<SequenceStep> sequenceSteps = new List<SequenceStep>();
    [SerializeField] private bool playOnStart = false;

    private void Start()
    {
        // Optionnel : configure automatiquement l'AudioSource s'il n'est pas assigné
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (playOnStart)
        {
            StartSequence();
        }
    }

    /// <summary>
    /// Appelle cette méthode pour lancer la séquence (via un bouton, un trigger, etc.)
    /// </summary>
    public void StartSequence()
    {
        if (sequenceSteps.Count == 0)
        {
            Debug.LogWarning("La liste de séquence est vide !");
            return;
        }

        StartCoroutine(PlaySequenceCoroutine());
    }

    private IEnumerator PlaySequenceCoroutine()
    {
        foreach (SequenceStep step in sequenceSteps)
        {
            // 1. Déclenchement de l'animation (si un trigger est renseigné)
            if (targetAnimator != null && !string.IsNullOrEmpty(step.animatorTrigger))
            {
                targetAnimator.SetTrigger(step.animatorTrigger);
            }

            // 2. Lecture du son (si un clip est renseigné)
            if (audioSource != null && step.audioClip != null)
            {
                audioSource.PlayOneShot(step.audioClip);
            }

            // 3. Attente avant la prochaine étape
            yield return new WaitForSeconds(step.delayBeforeNext);
        }

        Debug.Log("Séquence terminée !");
    }
}