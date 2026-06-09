# ARIS Mobile

O **ARIS** e um projeto da Global Solution 2026/1 que une **agricultura inteligente**, **IoT** e **monitoramento em tempo real** em uma proposta inspirada em cenarios espaciais. A ideia e mostrar como tecnologias usadas em ambientes extremos podem ser adaptadas para melhorar o controle de estufas, reduzir desperdicios e apoiar a producao agricola com mais eficiencia.

## O que o app faz

- mostra um dashboard com informacoes do sistema;
- permite login e cadastro de usuarios;
- gerencia estufas;
- gerencia culturas;
- acompanha alertas e dados operacionais;
- consome a API em .NET do backend;
- conversa com sensores simulados no Wokwi para demonstracao do fluxo IoT.

## Proposta do projeto

O ARIS foi pensado para representar uma solucao de agricultura conectada inspirada em cenarios como Marte e outras superficies extremas, onde cada recurso precisa ser monitorado com muito cuidado. A logica e simples: se tecnologias de missao espacial conseguem controlar ambientes hostis com sensores, telemetria e automacao, a mesma ideia pode ajudar a agricultura real na Terra.

No dia a dia, isso faz diferenca porque o projeto permite:

- acompanhar condicoes de cultivo em tempo real;
- definir limites de temperatura e umidade para cada cultura;
- visualizar dados vindos de sensores sem depender de verificacao manual;
- reduzir desperdicio de agua e falhas de monitoramento;
- apoiar tomadas de decisao com mais previsibilidade;
- conectar o mobile a uma API e a um ambiente de simulacao de sensores.

Em termos simples: o app ajuda a controlar o cultivo de forma mais inteligente, com foco em sustentabilidade, automacao e monitoramento continuo.

## Como isso ajuda na pratica

A agricultura sofre com problemas como calor excessivo, baixa umidade, irrigacao fora de hora e falta de visibilidade sobre o que esta acontecendo no cultivo. O ARIS entra justamente para diminuir essas falhas.

Quando a cultura e salva com temperatura e umidade minima/maxima:

- o sistema registra os parametros corretos do cultivo;
- a tela consegue mostrar esses valores depois do salvamento;
- o usuario passa a enxergar o que cada cultura precisa;
- o monitoramento fica mais util para simulacao e demonstracao;
- o projeto ganha valor tecnico porque fecha o ciclo entre cadastro, leitura e acompanhamento.

Isso tambem ajuda a explicar a proposta de Marte: em um ambiente extremo, nao da para agir no improviso. Tudo precisa ser medido, comparado com limites definidos e exibido de forma clara. O ARIS traz essa mesma mentalidade para a agricultura.

## Estrutura do repositorio

```text
ARIS-mobile/
  ARIS-backend/
    Aris.api/
    Aris.Application/
    Aris.Domain/
    Aris.Infrastructure/
    Aris.Tests/
  ArisMobile/
    App.js
    index.js
    src/
      assets/
      components/
      context/
      routes/
      screens/
      services/
      styles/
```

## Como abrir cada pasta

### 1. Backend

Abra a pasta `ARIS-backend` no **Rider** ou no **Visual Studio**.

Se preferir terminal:

```powershell
cd .\ARIS-backend
dotnet run --project .\Aris.api\Aris.api.csproj
```

O backend sobe por padrao em:

- `http://localhost:5070`

Swagger:

- `http://localhost:5070/swagger`

### 2. Mobile

Abra a pasta `ArisMobile` no **VS Code**.

No terminal da pasta do app:

```cmd
cd .\ArisMobile
npm i
npx expo start
```

Depois abra em:

- **Expo Go** no celular;
- emulador Android;
- simulador compativel com Expo.

### 3. Wokwi

O projeto de sensores pode ser aberto aqui:

- https://wokwi.com/projects/466024476249026561

Use o Wokwi para simular leitura de sensores e testar o fluxo de telemetria ligado ao projeto.

## Como o projeto se organiza

### Backend

O backend esta na pasta `ARIS-backend` e foi dividido em camadas:

- `Aris.api`: controllers, middleware e configuracao da aplicacao;
- `Aris.Application`: servicos, DTOs e contratos;
- `Aris.Domain`: entidades e regras de dominio;
- `Aris.Infrastructure`: repositorios, persistencia, migrations e integracoes.

Essa separacao facilita manutencao, testes e evolucao do sistema.

### Mobile

O app mobile esta em `ArisMobile` e usa Expo para acelerar o desenvolvimento. As telas principais ficam em `src/screens`, enquanto as chamadas para a API ficam em `src/services`.

## Configuracao da API no mobile

O app usa a URL definida em `src/services/api.js`.

Por padrao:

- Android emulator: `http://10.0.2.2:5070/api`
- demais casos locais: `http://localhost:5070/api`

Se precisar apontar para outra maquina ou ambiente, ajuste a variavel:

```powershell
EXPO_PUBLIC_API_URL
```

## Funcionalidades principais

- autenticacao de usuario;
- cadastro e edicao de estufas;
- cadastro e edicao de culturas com temperatura e umidade minima/maxima;
- visualizacao de alertas;
- integracao com telemetria e sensores;
- base pronta para demonstracao academica e evolucao futura.

## Observacao sobre sensores

Para demonstracao com sensores, mantenha o backend rodando e use o projeto no Wokwi como fonte de simulacao. Assim voce consegue mostrar o fluxo completo:

1. sensor gera dado;
2. backend recebe e processa;
3. mobile consulta a API;
4. usuario visualiza a informacao na interface.

Esse fluxo e importante porque mostra que o app nao e so uma tela bonita: ele conecta cadastro, telemetria e exibicao dos dados para dar sentido ao monitoramento da cultura.

## Integrantes

- Amandha Yumi Toyota Artulino - RM: 563549
- Giovanna Bardella Gomes - RM: 561439
- Erick Takeshi Nakajune - RM: 566059

## Links

- Youtube: https://youtube.com/shorts/BuxT520oGQ4?si=I233ZZWJNBjFX2jW
- Repositorio: https://github.com/ARIS-GlobalSolution/ARIS-Mobile.git
- Wokwi: https://wokwi.com/projects/466024476249026561

## Dica rapida de execucao

Se quiser testar tudo de uma vez, a ordem recomendada e:

1. abrir `ARIS-backend` no Rider;
2. rodar a API em `http://localhost:5070`;
3. abrir `ArisMobile` no VS Code;
4. executar `npm install`;
5. executar `npx expo start`;
6. abrir o projeto do Wokwi para simular os sensores.
