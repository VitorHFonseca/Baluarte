using System;
using System.Collections.Generic;
using UnityEngine;
using GameCore.Data;

namespace GameCore.Systems
{
    public interface IGameState
    {
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }

    [Serializable]
    public class SessionInventory
    {
        public List<CreatureInstance> animaisCapturados = new List<CreatureInstance>();
        public int MoedasGanhas;
        public string Timestamp;

        public SessionInventory()
        {
            Timestamp = DateTime.UtcNow.ToString("o");
        }
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private IGameState _estadoAtual;
        public SessionInventory inventarioSessaoAtiva;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ChangeState(new MainMenuState());
        }

        private void Update()
        {
            _estadoAtual?.OnUpdate();
        }

        public void ChangeState(IGameState novoEstado)
        {
            _estadoAtual?.OnExit();
            _estadoAtual = novoEstado;
            _estadoAtual?.OnEnter();
            Debug.Log($"Transição de Estado: Inicializado {novoEstado.GetType().Name}");
        }
    }

    // Concrete States para Máquina de Estados Finita (GoF Pattern)
    public class MainMenuState : IGameState
    {
        public void OnEnter() { Debug.Log("Entrou no Menu Principal."); }
        public void OnUpdate() { }
        public void OnExit() { }
    }

    public class CapturingState : IGameState
    {
        public void OnEnter()
        {
            CaptureSession.Instance.OnSessionEnd += TratarFimCaptura;
            CaptureSession.Instance.IniciarSessão();
        }

        public void OnUpdate() { }

        public void OnExit()
        {
            if (CaptureSession.Instance != null)
                CaptureSession.Instance.OnSessionEnd -= TratarFimCaptura;
        }

        private void TratarFimCaptura(SessionInventory inv)
        {
            GameManager.Instance.inventarioSessaoAtiva = inv;
            GameManager.Instance.ChangeState(new TriageState());
        }
    }

    public class TriageState : IGameState
    {
        public void OnEnter() { Debug.Log("Tela de Triagem Pronta para Renderização via UI."); }
        public void OnUpdate() { }
        public void OnExit() { }
    }

    public class DefenseState : IGameState
    {
        private SessionInventory _dadosInjetados;

        public void OnEnter()
        {
            _dadosInjetados = GameManager.Instance.inventarioSessaoAtiva;
            // Configurar e inicializar o mapa e Waves de Tower Defense filtrando por SelecionadoParaDefesa == true
            Simulation.TowerDefenseEngine.Instance.CarregarTorresDisponiveis(_dadosInjetados);
        }

        public void OnUpdate() { }
        public void OnExit() { }
    }

    public class RewardState : IGameState
    {
        public void OnEnter() { }
        public void OnUpdate() { }
        public void OnExit() { }
    }
}