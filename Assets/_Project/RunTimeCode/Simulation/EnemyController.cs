using UnityEngine;
using GameCore.Data;

namespace GameCore.Simulation
{
    public class EnemyController : MonoBehaviour
    {
        public float vidaAtual = 100f;
        public bool estaMorto = false;

        public void ReceberDano(float dano, ElementType elementoDoAtaque)
        {
            if (estaMorto) return;
            vidaAtual -= dano;
            if (vidaAtual <= 0)
            {
                estaMorto = true;
                EnemyManager.Instance.RemoverInimigo(this);
                Destroy(gameObject);
            }
        }
    }
}
