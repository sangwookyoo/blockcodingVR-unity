SteamVR SDK Support is currently experimental. If you require Steam device support (such as Valve Index or HTC Cosmos) this may be a good option for you, otherwise I suggest using the OpenVR plugin along with XR Plugin Management. 

If you want to give SteamVR a go, this integration package can get you started. It includes SteamVR bindings you can use to map SteamVR actions to raw inputs that the InputBridge can read. 

1. First install the SteamVR asset from the asset store : https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647

2. Extract the included SteamVR integration package (SteamVR.unityPackage)

3. Click "Yes" to import the provided Input Actions / Bindings into SteamVR. You can integrate them with your own bindings, or overwrite them completely.

4. Add "STEAM_VR_SDK" (no quotes) to your Scripting Define Symbols (Edit -> Project Settings -> Other -> Scripting Define Symbols).
  - Alternatively, you can go to Window -> VRIF and check "SteamVR Integration"

5. On Player's InputBridge object, make sure "SteamVR" is selected as the input source.

6. On the Player object, add the "SteamVR_ActivateActionSetOnLoad" and make sure the Action Set is set to the VRIF actions that were imported

- If you aren't getting any inputs, make sure yours keys are all mapped by going to Window -> SteamVR Input -> "Open Binding UI" and verify all inputs have been mapped.

Note : This essentially maps SteamVR Actions such as "Grip", "Trigger", etc. so that the InputBridge can convert it to be used as raw input. It's not how SteamVR's input system is intended to be used, but is currently the only way to get input from certain devices until Unity gets full OpenVR or OpenXR support.

- Check out InputBridge.cs to see how Steam Actions are bound to inputs

