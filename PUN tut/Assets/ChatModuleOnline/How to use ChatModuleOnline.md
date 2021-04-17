# How to use ChatModuleOnline

1. 将`Chat Panel.prefab`拖入Scene。Scene中会出现聊天输入框和Send按钮等UI

2. 将`ChatBubbleCanvas.prefab`拖入player prefab

3. 将`ChatBubbleManager.cs` component 加入 player prefab，并设置该component的3个public variable

   * photonView ==> `Photon View` component of this player prefab
   * BubbleSpeechObject ==> `ChatBubble` (under `ChatBubbleCanvas`)
   * UpdatedText ==> `ChatBubbleContent` (under `ChatBubbleCanvas` > `ChatBubble`)

   将该component加入 PhotonView component (of this player prefeb) > Observed Components (这一步可能unity会自动完成)

