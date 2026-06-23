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

Atalho ja preparado:

- No Unity, rode `Baluarte > Cenas > Criar Cena Inicial`.
- Isso cria `Assets/_Project/Scenes/CenaInicial.unity` com os managers principais e a tela inicial.
- Esse atalho tambem cria os dados de teste e conecta as habilidades/configuracao nos managers.
- A tela inicial mostra a versao `0.0.0.3`, as atualizacoes, e leva para criacao de personagem quando nao existir perfil salvo.

## Fluxo inicial do jogador

- Se nao existir personagem salvo, o botao inicial abre a criacao.
- A criacao permite escolher `Homem` ou `Mulher` por enquanto com visual padrao.
- O personagem recebe status iniciais: Vigor, Foco, Empatia Animal, Sorte e Conhecimento Arcano.
- Se ja existir personagem salvo, o botao inicial comeca a jornada diretamente.

## Passo 3: criar dados de teste

Em `Assets/_Project/ScriptableObjects`, criar:

- 1 criatura de fogo.
- 1 habilidade ativa de fogo.
- 1 habilidade passiva de fogo.
- 1 configuracao simples de tower defense.

Atalho ja preparado:

- No Unity, rode `Baluarte > Dados de Teste > Criar ou Atualizar`.
- Isso cria/atualiza os assets de teste, o prefab de projetil, o prefab de inimigo e a config `ConfigTD_VerticalSlice`.
- Depois, no objeto `AbilitySystem`, arraste as habilidades criadas para a lista `Banco De Dados Habilidades`.
- No objeto `TowerDefenseEngine`, arraste `ConfigTD_VerticalSlice` para o campo `Config TD`.

Se voce usar `Baluarte > Cenas > Criar Cena Inicial`, essas referencias ja sao conectadas automaticamente.

## Passo 4: fechar o loop jogavel

O primeiro marco bom e pequeno e:

- Capturar pelo menos 1 criatura.
- Marcar ela para defesa.
- Carregar a defesa.
- Instanciar inimigos.
- Ver uma torre causar dano.

Quando esse ciclo funcionar, o jogo deixa de ser apenas arquitetura e vira uma base jogavel.
