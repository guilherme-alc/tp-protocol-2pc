# Simulação do Protocolo Two-Phase Commit (2PC)

Este projeto implementa uma simulação do protocolo **Two Phase Commit (2PC)** usando **ASP.NET Core Web API**. Cada participante roda como uma API independente em uma porta diferente e pode atuar como **coordenador** quando inicia uma transação.

## ⚙️ Como funciona

- Cada participante roda em uma porta diferente (`5000`, `5001`, `5002`, `5003`);
- Quando um deles inicia a transação (`/iniciar`), ele se torna o coordenador;
- Ele envia a mensagem da transação para os outros pedindo um voto (`/votar`);
- Se todos responderem **SIM**, ele envia `/commit` para todos;
- Caso contrário (alguém vote NÃO ou não responda a tempo), ele envia `/abort`;
- A mensagem só é gravada no arquivo `participante_{PORTA}.txt` se todos confirmarem.

## 📁 Estrutura do Projeto

Há **4 projetos separados** com nomes:

- `Participante5000`
- `Participante5001`
- `Participante5002`
- `Participante5003`

Cada um representa uma instância do participante.


## Como executar

### 1. Abra 4 terminais

Você precisa abrir **4 terminais diferentes** (um para cada participante).  
Nos terminais, vá até o diretório do projeto.

### 2. Execute os participantes

Execute uma linha do comando abaixo em **cada terminal**:

```bash
dotnet run --project Participante5000
dotnet run --project Participante5001
dotnet run --project Participante5002
dotnet run --project Participante5003
```

## 🧪 Testando no Postman ou Insomnia

1. Crie uma requisição `POST` para:  
   `http://localhost:5000/iniciar` (ou outra porta que você queira testar como coordenador).
2. No body da requisição, selecione `raw`, escolha `JSON`, e envie uma string:

   ```json
   "Essa mensagem está sendo enviada pelo participante da porta 5003"
   ```

## O que acontece?

- O participante da porta 5003 envia o voto para os outros participantes.
- Se todos os votos forem SIM, cada um grava a mensagem em seu arquivo txt.
- Se algum participante votar NÃO ou não responder (timeout), a transação vai ser abortada e ninguém grava a mensagem.


### Arquivos gerados
Cada participante cria um arquivo de texto com seu nome:

- participante_5000.txt
- participante_5001.txt
- participante_5002.txt
- participante_5003.txt

Se a transação der certo (commit), a mensagem vai ser gravada em cada um desses arquivos.


### Para simular falhas

1. Alterando o método Votar em um dos participantes para retornar `false`.
2. Não iniciando um dos participantes (não rodar um dos terminais), simulando assim um timeout.
