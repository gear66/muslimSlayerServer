using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Prototype.NetworkLobby
{
    //List of players in the lobby
    public class LobbyPlayerList : MonoBehaviour
    {
        public static LobbyPlayerList _instance = null;

        public RectTransform playerListContentTransform;
        public GameObject warningDirectPlayServer;
        public Transform addButtonRow;

        protected VerticalLayoutGroup _layout;
        protected List<LobbyPlayer> _players = new List<LobbyPlayer>();

        public void OnEnable()
        {
            _instance = this;
            _layout = playerListContentTransform.GetComponent<VerticalLayoutGroup>();
            _players = new List<LobbyPlayer>();
        }

        public void ClearPlayres()
        {
            _players.ForEach((player) =>
            {
                Destroy(player);
            });
            _players = new List<LobbyPlayer>();
        }

        public void DisplayDirectServerWarning(bool enabled)
        {
            if(warningDirectPlayServer != null)
                warningDirectPlayServer.SetActive(enabled);
        }

        void Update()
        {
            //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
            //sometime to child being assigned before layout was enabled/init, leading to broken layouting)
            
            if(_layout)
                _layout.childAlignment = Time.frameCount%2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
        }

        public void AddPlayer(LobbyPlayer player)
        {
            if (_players.Contains(player))
                return;

            _players.Add(player);

            player.transform.SetParent(playerListContentTransform, false);
            addButtonRow.transform.SetAsLastSibling();

            PlayerListModified();
        }

        public void TogglePlayerVideo(string name, bool setToggle)
        {
            LobbyPlayer lobbyPlayer = _players.Find(player => player.nameInput.text == name);
            if (lobbyPlayer != null)
            {
                lobbyPlayer.ToggleField(setToggle);
            }
        }

        public void SetPlayerDuration(string name, float dur)
        {
            LobbyPlayer lobbyPlayer = _players.Find(player => player.nameInput.text == name);
            if (lobbyPlayer != null)
            {
                lobbyPlayer.SetDuration(dur); 
            }
        }
        
        public void RemovePlayer(LobbyPlayer player)
        { 
            _players.Remove(player);
            Destroy(player.gameObject);
            //player.gameObject.SetActive(false);
            PlayerListModified();
        }

        public void RemovePlayer(string playerName)
        {
            LobbyPlayer lobbyPlayer = _players.Find(player => player.nameInput.text == playerName);
            _players.Remove(lobbyPlayer);
            Destroy(lobbyPlayer);
            //player.gameObject.SetActive(false);
            PlayerListModified();
        }

        public void PlayerListModified()
        {
            int i = 0;
            foreach (LobbyPlayer p in _players)
            {
                p.OnPlayerListChanged(i);
                ++i;
            }
        }

        public void UpdatePlayer(User user, string data)
        {
            LobbyPlayer lobbyPlayer = _players.Find(player => player.nameInput.text == user.userName);
            lobbyPlayer.OnDataChanged(data);
        }
    }
}
