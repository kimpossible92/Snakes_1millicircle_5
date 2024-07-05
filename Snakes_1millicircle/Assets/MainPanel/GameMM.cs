using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using Photon.Pun.Demo.Asteroids;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine.UIElements;

public class PlayerSpaceship { public string NickName; public int score; }
public class Const1
{
    public const float ASTEROIDS_MIN_SPAWN_TIME = 5.0f;
    public const float ASTEROIDS_MAX_SPAWN_TIME = 10.0f;

    public const float PLAYER_RESPAWN_TIME = 4.0f;

    public const int PLAYER_MAX_LIVES = 3;

    public const string PLAYER_LIVES = "PlayerLives";
    public const string PLAYER_READY = "IsPlayerReady";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

    public static Color GetColor(int colorChoice)
    {
        switch (colorChoice)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
            case 4: return Color.cyan;
            case 5: return Color.grey;
            case 6: return Color.magenta;
            case 7: return Color.white;
        }

        return Color.black;
    }
}
namespace Photon.Pun.UtilityScripts
{
    public class GameMM : MonoBehaviourPunCallbacks
    {
        public static GameMM Instance = null;

        public Text InfoText;

        //public GameObject[] AsteroidPrefabs;

        #region UNITY
        [SerializeField] private UnityEngine.UI.Image HudTargetWindow;
        [SerializeField] HUD_Text_Controller healthTexts, manaTexts, expTexts;
        private void Awake()
        {
            Instance = this;

        }
        public bool setIsOnline()
        {
            if (FindObjectOfType<new_offline_mode>() != null) { return true; }
            return false;
        }
        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
            if (FindObjectOfType<new_offline_mode>() != null)
            {
                offlineStartGame();
                return;
            }
        }

        private void Start()
        {
            Hashtable props = new Hashtable
            {
                {Const1.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            StartGame();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        #endregion

        #region COROUTINES

        private IEnumerator SpawnAsteroid()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(Const1.ASTEROIDS_MIN_SPAWN_TIME, Const1.ASTEROIDS_MAX_SPAWN_TIME));

                Vector2 direction = Random.insideUnitCircle;
                Vector3 position = Vector3.zero;

                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    // Make it appear on the left/right side
                    position = new Vector3(Mathf.Sign(direction.x) * Camera.main.orthographicSize * Camera.main.aspect, 0, direction.y * Camera.main.orthographicSize);
                }
                else
                {
                    // Make it appear on the top/bottom
                    position = new Vector3(direction.x * Camera.main.orthographicSize * Camera.main.aspect, 0, Mathf.Sign(direction.y) * Camera.main.orthographicSize);
                }

                // Offset slightly so we are not out of screen at creation time (as it would destroy the asteroid right away)
                position -= position.normalized * 0.1f;


                Vector3 force = -position.normalized * 1000.0f;
                Vector3 torque = Random.insideUnitSphere * Random.Range(500.0f, 1500.0f);
                object[] instantiationData = { force, torque, true };

                PhotonNetwork.InstantiateRoomObject("BigAsteroid", position, Quaternion.Euler(Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f), 0, instantiationData);
            }
        }

        private IEnumerator EndOfGame(string winner, int score)
        {
            float timer = 5.0f;

            while (timer > 0.0f)
            {
                InfoText.text = string.Format("Player2 {0} won with {1} points.\n\n\nReturning to login screen in {2} seconds.", winner, score, timer.ToString("n2"));

                yield return new WaitForEndOfFrame();

                timer -= Time.deltaTime;
            }

            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("sampleScreen");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnMasterClientSwitched(Player2 newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartCoroutine(SpawnAsteroid());
            }
        }

        public override void OnPlayerLeftRoom(Player2 otherPlayer)
        {
            CheckEndOfGame();
        }

        public override void OnPlayerPropertiesUpdate(Player2 targetPlayer, Hashtable changedProps)
        {
            if (FindObjectOfType<new_offline_mode>() != null)
            {
                print("return");
                return;
            }
            if (changedProps.ContainsKey(Const1.PLAYER_LIVES))
            {
                CheckEndOfGame();
                return;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }


            // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
            int startTimestamp;
            bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

            if (changedProps.ContainsKey(Const1.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    if (!startTimeIsSet)
                    {
                        CountdownTimer.SetStartTime();
                    }
                }
                else
                {
                    // not all players loaded yet. wait:
                    Debug.Log("setting text waiting for players! ", this.InfoText);
                    InfoText.text = "Waiting for other players...";
                }
            }

        }

        #endregion

        int playerCount = 0;
        [SerializeField] private Vector3 spawnPos;
        public void offlineStartGame()
        {
            GameObject player = MClass.Instance.GetPlayerPrefab;
            Vector3 position = spawnPos;
            var pgo = Instantiate(player, position, Quaternion.identity);
            //myplayer
            pgo.GetComponentInChildren<HeroClass>().setLocalId = FindObjectOfType<InputTargeting>().isLocalUserId;
            FindObjectOfType<HealthSlider2D_Script>().SetHero(pgo.GetComponentInChildren<HeroClass>(), pgo.GetComponent<PlayerView>().slider2);
            FindObjectOfType<ManaSlider2D_Script>().SetHero(pgo.GetComponentInChildren<HeroClass>(), pgo.GetComponent<PlayerView>().slider1);
            FindObjectOfType<ExpSlider2D_Script>().SetHero(pgo.GetComponentInChildren<HeroClass>());
            //FindObjectOfType<HUD_Text_Controller>().SetHero(pgo.GetComponentInChildren<HeroClass>());
            healthTexts.SetHero(pgo.GetComponentInChildren<HeroClass>());
            manaTexts.SetHero(pgo.GetComponentInChildren<HeroClass>());
            expTexts.SetHero(pgo.GetComponentInChildren<HeroClass>());

            FindObjectOfType<TrackEnemyInfo_HUDWindow>().SetHero(pgo.GetComponentInChildren<HeroClass>());
            FindObjectOfType<HUD_HoverAbilityToolTip>().SetHero(pgo.GetComponentInChildren<HeroClass>());
            //
            //FindObjectOfType<InputTargeting>().SetHero(pgo.GetComponentInChildren<HeroClass>().gameObject);
            pgo.GetComponent<InputTargeting>().SetHero(HudTargetWindow);
            FindObjectOfType<CameraFollow_Script>().SetHero(pgo.GetComponentInChildren<HeroClass>().gameObject);
            //playerCount = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        }
        // called by OnCountdownTimerIsExpired() when the timer ended
        private void StartGame()
        {
            if (FindObjectOfType<new_offline_mode>() != null)
            {
                offlineStartGame();
                return;
            }
            Debug.Log("StartGame!");

            // on rejoin, we have to figure out if the spaceship exists or not
            // if this is a rejoin (the ship is already network instantiated and will be setup via event) we don't need to call PN.Instantiate


            float angularStart = (360.0f / PhotonNetwork.CurrentRoom.PlayerCount) * PhotonNetwork.LocalPlayer.GetPlayerNumber();
            //float x = 20.0f * Mathf.Sin(angularStart * Mathf.Deg2Rad);
            int x = Random.Range(-20, 20);
            float z = 20.0f * Mathf.Cos(angularStart * Mathf.Deg2Rad);
            Vector3 position = new Vector3(24, 0.0f, 24);
            Quaternion rotation = Quaternion.Euler(0.0f, angularStart, 0.0f);
            //Debug.Log("[Launcher] JoinRoom Photon");
            GameObject player = MClass.Instance.GetPlayerPrefab;

            if (PhotonNetwork.LocalPlayer.GetPlayerNumber() > -1)
                spawnPos = -spawnPos;

            var pgo = PhotonNetwork.Instantiate(player.name, position, Quaternion.identity, 0);
            pgo.GetComponentInChildren<HeroClass>().setLocalId = FindObjectOfType<InputTargeting>().isLocalUserId;
            FindObjectOfType<HealthSlider2D_Script>().SetHero(pgo.GetComponentInChildren<HeroClass>(), pgo.GetComponent<PlayerView>().slider2);
            FindObjectOfType<ManaSlider2D_Script>().SetHero(pgo.GetComponentInChildren<HeroClass>(), pgo.GetComponent<PlayerView>().slider1);
            FindObjectOfType<ExpSlider2D_Script>().SetHero(pgo.GetComponentInChildren<HeroClass>());
            //FindObjectOfType<HUD_Text_Controller>().SetHero(pgo.GetComponentInChildren<HeroClass>());
            healthTexts.SetHero(pgo.GetComponentInChildren<HeroClass>());
            manaTexts.SetHero(pgo.GetComponentInChildren<HeroClass>());
            expTexts.SetHero(pgo.GetComponentInChildren<HeroClass>());

            FindObjectOfType<TrackEnemyInfo_HUDWindow>().SetHero(pgo.GetComponentInChildren<HeroClass>());
            FindObjectOfType<HUD_HoverAbilityToolTip>().SetHero(pgo.GetComponentInChildren<HeroClass>());
            //
            //FindObjectOfType<InputTargeting>().SetHero(pgo.GetComponentInChildren<HeroClass>().gameObject);
            pgo.GetComponent<InputTargeting>().SetHero(HudTargetWindow);
            FindObjectOfType<CameraFollow_Script>().SetHero(pgo.GetComponentInChildren<HeroClass>().gameObject);
            playerCount = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            //print(PhotonNetwork.LocalPlayer.GetPlayerNumber());
            if (PhotonNetwork.IsMasterClient)
            {
                //StartCoroutine(SpawnAsteroid());
            }
        }

        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player2 p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(Const1.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool)playerLoadedLevel)
                    {
                        continue;
                    }
                }


                return false;
            }

            return true;
        }

        private void CheckEndOfGame()
        {
            bool allDestroyed = true;

            foreach (Player2 p in PhotonNetwork.PlayerList)
            {
                object lives;
                if (p.CustomProperties.TryGetValue(Const1.PLAYER_LIVES, out lives))
                {
                    if ((int)lives > 0)
                    {
                        allDestroyed = false;
                        break;
                    }
                }
            }

            if (allDestroyed)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StopAllCoroutines();
                }

                string winner = "";
                int score = -1;

                foreach (Player2 p in PhotonNetwork.PlayerList)
                {
                    if (p.GetScore() > score)
                    {
                        winner = p.NickName;
                        score = p.GetScore();
                    }
                }

                StartCoroutine(EndOfGame(winner, score));
            }
        }

        private void OnCountdownTimerIsExpired()
        {
            StartGame();
        }
    }
}