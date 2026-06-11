using UnityEngine;
using GameCore.Data;

namespace GameCore.Simulation
{
    public class Projectile : MonoBehaviour
    {
        private EnemyController _alvo;
        private float _dano;
        private ElementType _elemento;
        private float _velocidade = 12f;

        public void Init(EnemyController alvo, float danoFinal, ElementType el)
        {
            _alvo = alvo;
            _dano = danoFinal;
            _elemento = el;
        }

        private void Update()
        {
            if (_alvo == null)
            {
                Destroy(gameObject); // Retornar ao pool em cenário real
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _alvo.transform.position, _velocidade * Time.deltaTime);

            if ((transform.position - _alvo.transform.position).sqrMagnitude < 0.05f)
            {
                AplicarImpactoFinal();
            }
        }

        private void AplicarImpactoFinal()
        {
            if (_alvo != null)
            {
                _alvo.ReceberDano(_dano, _elemento);
            }
            Destroy(gameObject);
        }
    }
}