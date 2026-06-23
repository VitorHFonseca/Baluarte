using System;
using System.IO;
using GameCore.Data;
using UnityEngine;

namespace GameCore.Systems
{
    public class PlayerProfileService : MonoBehaviour
    {
        public static PlayerProfileService Instance { get; private set; }

        public PlayerProfile CurrentProfile { get; private set; }
        public bool HasProfile => CurrentProfile != null;

        private string SavePath => Path.Combine(Application.persistentDataPath, "player_profile.json");

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadProfile();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public PlayerProfile CreateProfile(string nome, CharacterBodyType corpo)
        {
            CurrentProfile = PlayerProfile.Create(nome, corpo);
            SaveProfile();
            return CurrentProfile;
        }

        public void TouchLastPlayed()
        {
            if (CurrentProfile == null) return;

            CurrentProfile.lastPlayedAtUtc = DateTime.UtcNow.ToString("o");
            SaveProfile();
        }

        public void SaveProfile()
        {
            if (CurrentProfile == null) return;

            string json = JsonUtility.ToJson(CurrentProfile, true);
            File.WriteAllText(SavePath, json);
        }

        public void LoadProfile()
        {
            if (!File.Exists(SavePath))
            {
                CurrentProfile = null;
                return;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                CurrentProfile = JsonUtility.FromJson<PlayerProfile>(json);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Nao foi possivel carregar o perfil do jogador: {ex.Message}");
                CurrentProfile = null;
            }
        }

        public void DeleteProfile()
        {
            CurrentProfile = null;

            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }
        }
    }
}
