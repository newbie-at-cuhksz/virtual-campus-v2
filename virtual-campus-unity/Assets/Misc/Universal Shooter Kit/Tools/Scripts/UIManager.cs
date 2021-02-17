using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	public class UIManager : MonoBehaviour
	{

        [Serializable]
		public class multiplayerGameLobby
		{
			public UIPlaceholder weaponPlaceholder;
			public UIPlaceholder gameModePlaceholder;
			public UIPlaceholder mapPlaceholder;
			public UIPlaceholder avatarPlaceholder;
			public UIPlaceholder roomInfoPlaceholder;

			public UIHelper.LobbyMainUI MainMenu;
			public UIHelper.LobbyGameModesUI GameModesMenu;
			public UIHelper.LobbyMapsUI MapsMenu;
			public UIHelper.LobbyLoadoutUI LoadoutMenu;
			public UIHelper.AvatarsMenu AvatarsMenu;
			public UIHelper.LobbyCharactersMenu CharactersMenu;
			public UIHelper.AllRoomsMenu AllRoomsMenu;
			public UIHelper.CreateRoomMenu CreateRoomMenu;
            public UIHelper.CreditsMenu CreditsMenu;
            public UIHelper.SettingsMenu SettingsMenu;
		}

		[Serializable]
		public class multiplayerGameRoom
		{
			public UIPlaceholder playerInfoPlaceholder;
			public UIPlaceholder matchStatsPlaceholder;
			public PUNHelper.GameOverMenu GameOverMenu;
			public PUNHelper.SpectateMenu SpectateMenu;
			public PUNHelper.MatchStats MatchStats;
			public PUNHelper.PauseMenu PauseMenu;
			public PUNHelper.StartMenu StartMenu;
			public PUNHelper.TimerBeforeMatch TimerBeforeMatch;
			public PUNHelper.TimerAfterDeath TimerAfterDeath;
			public PUNHelper.PreMatchMenu PreMatchMenu;
            public PUNHelper.LoadingScreen LoadingScreen;
		}

		public multiplayerGameLobby MultiplayerGameLobby;
		public multiplayerGameRoom MultiplayerGameRoom;
		public UIHelper.CharacterUI CharacterUI;
		public UIHelper.singlePlayerGame SinglePlayerGame;
		
		public Button[] uiButtons = new Button[17];

		public GameObject UIButtonsMainObject;
		public GameObject moveStick;
		public GameObject moveStickOutline;
		public GameObject cameraStick;
		public GameObject cameraStickOutline;

        #region InspectorVariables

        public int inspectorTab;

		public int multiplayerGameInspectorTab;
		
		public int roomInspectorTabTop;
		public int roomInspectorTabMid;
        public int roomInspectorTabDown;
        public int currentRoomInspectorTab;
		public int roomMatchStatsTab;

		public int lobbyInspectorTabTop;
		public int lobbyInspectorTabMiddle;
		public int lobbyInspectorTabBottom;
		public int currentLobbyInspectorTab;
		
		public int characterUiInspectorTab;
		public int curWeaponSlot;

        #endregion

        

		private void Awake()
		{
			if(MultiplayerGameLobby.weaponPlaceholder)
				MultiplayerGameLobby.weaponPlaceholder.gameObject.SetActive(false);
			
			if(MultiplayerGameLobby.mapPlaceholder)
				MultiplayerGameLobby.mapPlaceholder.gameObject.SetActive(false);
			
			if(MultiplayerGameLobby.gameModePlaceholder)
				MultiplayerGameLobby.gameModePlaceholder.gameObject.SetActive(false);
			
			if(MultiplayerGameLobby.avatarPlaceholder)
				MultiplayerGameLobby.avatarPlaceholder.gameObject.SetActive(false);

			if(MultiplayerGameRoom.matchStatsPlaceholder)
				MultiplayerGameRoom.matchStatsPlaceholder.gameObject.SetActive(false);
			
			if(MultiplayerGameRoom.playerInfoPlaceholder)
				MultiplayerGameRoom.playerInfoPlaceholder.gameObject.SetActive(false);
			
			foreach (var marker in CharacterUI.hitMarkers)
			{
				if (marker)
					marker.gameObject.SetActive(false);
			}
		}

		public void HideAllMultiplayerLobbyUI()
		{
			MultiplayerGameLobby.MainMenu.DisableAll();
			MultiplayerGameLobby.LoadoutMenu.DisableAll();
			MultiplayerGameLobby.MapsMenu.DisableAll();
			MultiplayerGameLobby.GameModesMenu.DisableAll();
			MultiplayerGameLobby.AvatarsMenu.DisableAll();
			MultiplayerGameLobby.CharactersMenu.DisableAll();
			MultiplayerGameLobby.AllRoomsMenu.DisableAll();
			MultiplayerGameLobby.CreateRoomMenu.DisableAll();
            MultiplayerGameLobby.CreditsMenu.DisableAll();

            if (MultiplayerGameLobby.MainMenu.BGMAudioSource) MultiplayerGameLobby.MainMenu.BGMAudioSource.SetActive(false);

        }
		
		public void HideAllMultiplayerRoomUI()
		{
			MultiplayerGameRoom.SpectateMenu.DisableAll();
            
			MultiplayerGameRoom.MatchStats.DisableAll();

			MultiplayerGameRoom.GameOverMenu.DisableAll();
            
			MultiplayerGameRoom.PauseMenu.DisableAll();
            
			MultiplayerGameRoom.StartMenu.DisableAll();
            
			MultiplayerGameRoom.TimerBeforeMatch.DisableAll();
            
			MultiplayerGameRoom.TimerAfterDeath.DisableAll();
           
			MultiplayerGameRoom.PreMatchMenu.DisableAll();

            MultiplayerGameRoom.LoadingScreen.DisableAll();

            if (MultiplayerGameRoom.MatchStats.KillStatsContent)
				MultiplayerGameRoom.MatchStats.KillStatsContent.gameObject.SetActive(false);
		}
	}
}
