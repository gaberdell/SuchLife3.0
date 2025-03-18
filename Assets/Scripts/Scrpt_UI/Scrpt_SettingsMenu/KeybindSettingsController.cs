using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

namespace SuchLife_UI
{
    public class KeybindSettingsController : MonoBehaviour
    {

        [SerializeField]
        Button exitButton;

        [SerializeField]
        PlayerInput mainPlayerInput;

        [SerializeField]
        InputActionAsset bigAction;

        [SerializeField]
        SavableInputAction[] savableInputActions;

        const String GameKeySave = "gameKeysSaveJSON";

        void OnEnable()
        {
            exitButton.onClick.AddListener(SaveKeybinds);
            foreach (var savableInputAction in savableInputActions)
            {
                savableInputAction.model.onClick.AddListener(savableInputAction.startRebinding);
                savableInputAction.resetModel.onClick.AddListener(savableInputAction.reset);
            }
        }

        void OnDisable()
        {
            exitButton.onClick.RemoveListener(SaveKeybinds);
            foreach (var savableInputAction in savableInputActions)
            {
                savableInputAction.model.onClick.RemoveListener(savableInputAction.startRebinding);
                savableInputAction.resetModel.onClick.RemoveListener(savableInputAction.reset);
            }
        }

        //TODO: Saving and reloding key binds first make basic player controller
        void Start()
        {
            LoadKeyBinds();
        }

        //Save
        void SaveKeybinds()
        {
            string rebinds = mainPlayerInput.actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(GameKeySave, rebinds);
        }

        void LoadKeyBinds()
        {
            foreach (var savableInputAction in savableInputActions)
            {
                savableInputAction.load();
            }
        }

    }

    [System.Serializable]
    class SavableInputAction
    {
        [SerializeField]
        public Button model;

        [SerializeField]
        public Button resetModel;

        [SerializeField]
        KeybindSettingsView view;

        [SerializeField]
        ResetKeybindView resetView;

        [SerializeField]
        InputActionReference inputActionRef;

        [SerializeField]
        bool isMultiAction = false;

        [SerializeField]
        int multiActionIndex = 0;

        [SerializeField]
        string saveString = string.Empty;

        RebindingOperation rebindingOperation;

        bool wasEnabled = false;

        public void startRebinding()
        {
            if (inputActionRef.action.enabled)
            {
                wasEnabled = true;
                inputActionRef.action.Disable();
            }

            //Brute force I know I should make these seperate classes and inheritence but im lazy lol
            if (isMultiAction)
            {
                rebindingOperation = inputActionRef.action.PerformInteractiveRebinding()
                    .WithTargetBinding(multiActionIndex)
                    .WithControlsExcluding("Mouse")
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(operation => endRebinding())
                    .Start();
            }
            else
            {
                rebindingOperation = inputActionRef.action.PerformInteractiveRebinding()
                    .WithControlsExcluding("Mouse")
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(operation => endRebinding())
                    .Start();
            }


            view.SetText("Rebind Input...");
        }

        public void endRebinding()
        {
            //manually dispose of its memory idk why this isnt garbage collected
            rebindingOperation.Dispose();

            save();

            updateView();

            if (wasEnabled)
            {
                inputActionRef.action.Enable();
                wasEnabled = false;
            }

            /* Just some test code incase you need to see what count of controls is
             * for (int i = 0; i < inputActionRef.action.controls.Count; i++)
            {
                Debug.Log($"Index {i} : {inputActionRef.action.controls[i].path}");
            }*/

        }


        //Potentially reset idk tho
        public void reset()
        {
            int bindingIndex;
            if (isMultiAction)
            {
                bindingIndex = multiActionIndex;
                inputActionRef.action.ApplyBindingOverride(multiActionIndex, inputActionRef.action.bindings[bindingIndex].path);
            }
            else
            {
                bindingIndex = inputActionRef.action.GetBindingIndexForControl(inputActionRef.action.controls[0]);
                inputActionRef.action.ApplyBindingOverride(inputActionRef.action.bindings[bindingIndex].path);
            }
            
            string setText = InputControlPath.ToHumanReadableString(
                    inputActionRef.action.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);

            save();

            view.SetText(setText);
            resetView.SetRestButtonColor(true);
        }

        public void updateView()
        {
            //The inputActionRef.action.controls[0] gets the currently active InputControl
            int bindingIndex = inputActionRef.action.GetBindingIndexForControl(inputActionRef.action.controls[0]);

            string effectivePath;
            string path;

            if (isMultiAction)
            {
                effectivePath = inputActionRef.action.bindings[multiActionIndex].effectivePath;
                path = inputActionRef.action.bindings[multiActionIndex].path;
            }
            else
            {
                effectivePath = inputActionRef.action.bindings[bindingIndex].effectivePath;
                path = inputActionRef.action.bindings[bindingIndex].path;
            }

            string setText = InputControlPath.ToHumanReadableString(
                effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

            //If effective path matches actually path then that means the effective path
            //aka the override equals the actual path or the base value
            view.SetText(setText);
            resetView.SetRestButtonColor(effectivePath == path);
        }

        public void load()
        {
            string rebinds = PlayerPrefs.GetString(saveString);

            if (!string.IsNullOrEmpty(rebinds))
                inputActionRef.asset.LoadBindingOverridesFromJson(rebinds);
            updateView();
        }

        public void save()
        {
            string rebinds = inputActionRef.asset.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(saveString, rebinds);
        }
    }
}
