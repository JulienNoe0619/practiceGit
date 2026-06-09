using System;
using System.Collections.Generic;
using UnityEngine;
// Ajout du namespace pour les Splines de Unity
using UnityEngine.Splines;

public class KeyToTriggerManager : MonoBehaviour
{
    [System.Serializable]
    public struct KeyTriggerMapping
    {
        public string nomDescription;
        public KeyCode toucheClavier;
        public string triggerAnimator;

        // Nouvelle variable : le composant Spline Animate à déclencher pour cette touche
        [Tooltip("Le composant Spline Animate qui doit se lancer avec cette touche (Optionnel)")]
        public SplineAnimate splineA_Declencher;
    }

    [Header("Configuration")]
    public RuntimeAnimatorController animatorController;
    public Animator animatorCible;

    [Header("Liaisons Touches -> Triggers & Splines")]
    public List<KeyTriggerMapping> listeLiaisons = new List<KeyTriggerMapping>();

    void Awake()
    {
        if (animatorCible == null)
        {
            animatorCible = GetComponent<Animator>();
        }

        if (animatorCible != null && animatorController != null)
        {
            animatorCible.runtimeAnimatorController = animatorController;
        }
    }

    void Update()
    {
        foreach (var liaison in listeLiaisons)
        {
            if (Input.GetKeyDown(liaison.toucheClavier))
            {
                // 1. Déclenchement de l'animation (Animator)
                if (animatorCible != null && !string.IsNullOrEmpty(liaison.triggerAnimator))
                {
                    animatorCible.SetTrigger(liaison.triggerAnimator);
                }

                // 2. Déclenchement du mouvement sur la Spline
                if (liaison.splineA_Declencher != null)
                {
                    // Restart(true) remet l'objet au début de la spline et lance le Play.
                    // Si tu veux juste faire "Play" là où l'objet en était, utilise : liaison.splineA_Declencher.Play();
                    liaison.splineA_Declencher.Restart(true);
                }
            }
        }
    }
}