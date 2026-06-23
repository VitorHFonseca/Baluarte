# Proximos passos

## Objetivo imediato

Transformar o prototipo de sistemas em uma primeira cena jogavel curta:

1. O jogador inicia uma sessao de captura.
2. Clica para capturar criaturas por 60 a 90 segundos.
3. Ve uma tela simples de triagem.
4. Seleciona ate algumas criaturas para defesa.
5. Entra em uma cena de tower defense com uma onda basica.

## Passo 1: abrir e compilar no Unity

- Abrir a pasta raiz `Baluarte` pelo Unity Hub.
- Deixar o Unity importar os pacotes em `Packages/manifest.json`.
- Conferir o Console e resolver qualquer erro de compilacao restante antes de criar prefabs ou UI.

## Passo 2: criar a cena vertical slice

Criar uma cena chamada `VerticalSlice.unity` com estes objetos:

- `GameManager`
- `CaptureSession`
- `AbilitySystem`
- `EnemyManager`
- `TowerDefenseEngine`
- Canvas simples para botao de captura, timer, energia e lista de capturados.

## Passo 3: criar dados de teste

Em `Assets/_Project/ScriptableObjects`, criar:

- 1 criatura de fogo.
- 1 habilidade ativa de fogo.
- 1 habilidade passiva de fogo.
- 1 configuracao simples de tower defense.

## Passo 4: fechar o loop jogavel

O primeiro marco bom e pequeno e:

- Capturar pelo menos 1 criatura.
- Marcar ela para defesa.
- Carregar a defesa.
- Instanciar inimigos.
- Ver uma torre causar dano.

Quando esse ciclo funcionar, o jogo deixa de ser apenas arquitetura e vira uma base jogavel.
