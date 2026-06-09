using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyToTriggerManager : MonoBehaviour
{
    [System.Serializable]
    public struct KeyTriggerMapping
    {
        public string nomDescription;
        public KeyCode toucheClavier;
        public string triggerAnimator;
    }

    [Header("Configuration")]
    [Tooltip("Glisse ici ton Animator Controller depuis le panneau Project.")]
    // C'est cette variable qui te permet le Drag & Drop depuis le panneau Project
    public RuntimeAnimatorController animatorController;

    [Tooltip("L'objet de la scène qui doit être animé (optionnel si ce script est déjà sur l'objet).")]
    public Animator animatorCible;

    [Header("Liaisons Touches -> Triggers")]
    public List<KeyTriggerMapping> listeLiaisons = new List<KeyTriggerMapping>();

    void Awake()
    {
        // Si tu n'as pas assigné d'animator cible, on cherche celui du GameObject actuel
        if (animatorCible == null)
        {
            animatorCible = GetComponent<Animator>();
        }

        // Sécurité : on s'assure que l'animator de la scène utilise bien le bon controller au démarrage
        if (animatorCible != null && animatorController != null)
        {
            animatorCible.runtimeAnimatorController = animatorController;
        }
    }

    void Update()
    {
        if (animatorCible == null || animatorController == null) return;

        foreach (var liaison in listeLiaisons)
        {
            if (Input.GetKeyDown(liaison.toucheClavier))
            {
                if (!string.IsNullOrEmpty(liaison.triggerAnimator))
                {
                    animatorCible.SetTrigger(liaison.triggerAnimator);
                }
            }
        }
    }
}