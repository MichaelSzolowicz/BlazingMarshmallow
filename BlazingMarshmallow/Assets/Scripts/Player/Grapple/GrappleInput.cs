//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Scripts/Player/Grapple/GrappleInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GrappleInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GrappleInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GrappleInput"",
    ""maps"": [
        {
            ""name"": ""Grapple"",
            ""id"": ""9f1d2ed4-631d-45da-892b-918cceca80c9"",
            ""actions"": [
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""1f9ca257-2bec-454d-ae32-b3e07fe3cb93"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SwivelCursor"",
                    ""type"": ""Value"",
                    ""id"": ""9149f241-1d03-449f-bf56-5581edbae3bc"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1d931e80-adf1-4487-8705-84d5775ea94c"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b488b7cb-c44a-4986-bff5-881e33db630e"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwivelCursor"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Grapple
        m_Grapple = asset.FindActionMap("Grapple", throwIfNotFound: true);
        m_Grapple_Fire = m_Grapple.FindAction("Fire", throwIfNotFound: true);
        m_Grapple_SwivelCursor = m_Grapple.FindAction("SwivelCursor", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Grapple
    private readonly InputActionMap m_Grapple;
    private List<IGrappleActions> m_GrappleActionsCallbackInterfaces = new List<IGrappleActions>();
    private readonly InputAction m_Grapple_Fire;
    private readonly InputAction m_Grapple_SwivelCursor;
    public struct GrappleActions
    {
        private @GrappleInput m_Wrapper;
        public GrappleActions(@GrappleInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Fire => m_Wrapper.m_Grapple_Fire;
        public InputAction @SwivelCursor => m_Wrapper.m_Grapple_SwivelCursor;
        public InputActionMap Get() { return m_Wrapper.m_Grapple; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GrappleActions set) { return set.Get(); }
        public void AddCallbacks(IGrappleActions instance)
        {
            if (instance == null || m_Wrapper.m_GrappleActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GrappleActionsCallbackInterfaces.Add(instance);
            @Fire.started += instance.OnFire;
            @Fire.performed += instance.OnFire;
            @Fire.canceled += instance.OnFire;
            @SwivelCursor.started += instance.OnSwivelCursor;
            @SwivelCursor.performed += instance.OnSwivelCursor;
            @SwivelCursor.canceled += instance.OnSwivelCursor;
        }

        private void UnregisterCallbacks(IGrappleActions instance)
        {
            @Fire.started -= instance.OnFire;
            @Fire.performed -= instance.OnFire;
            @Fire.canceled -= instance.OnFire;
            @SwivelCursor.started -= instance.OnSwivelCursor;
            @SwivelCursor.performed -= instance.OnSwivelCursor;
            @SwivelCursor.canceled -= instance.OnSwivelCursor;
        }

        public void RemoveCallbacks(IGrappleActions instance)
        {
            if (m_Wrapper.m_GrappleActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGrappleActions instance)
        {
            foreach (var item in m_Wrapper.m_GrappleActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GrappleActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GrappleActions @Grapple => new GrappleActions(this);
    public interface IGrappleActions
    {
        void OnFire(InputAction.CallbackContext context);
        void OnSwivelCursor(InputAction.CallbackContext context);
    }
}
