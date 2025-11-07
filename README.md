# Proposta

MottuSense é uma solução inteligente desenvolvida para a Mottu, focada no mapeamento automatizado do pátio e na gestão eficiente das motos da frota.
Utilizando sensores IoT e uma arquitetura em nuvem com APIs .o sistema permite monitoramento em tempo real, controle de entrada e saída dos veículos, e integração com um app mobile para operadores.
Combinando banco de dados relacional e não relacional, DevOps, testes de qualidade e tecnologias modernas de desenvolvimento mobile e backend, o MottuSense garante rastreabilidade, performance e escalabilidade — tudo alinhado com os pilares da Mottu: tecnologia, mobilidade e oportunidade.

🛵 Nome da Solução: MottuSense
🔤 Significado:
"Mottu" (nome da empresa)
"Sense" de sensorial, percepção, inteligência → representa a capacidade da solução de "sentir" e gerenciar o pátio de motos com IoT.

## Diferencial

- Monitoramento em tempo real das motos atraves dos sensores
- localização exata das motos


# MottuSense API

MottuSense API é um sistema de rastreamento de motos desenvolvido em C# com .NET 8. Ele permite gerenciar clientes, motos, pátios e sensores de localização. O sistema é ideal para empresas como a Mottu que precisam monitorar a localização de suas motos em tempo real, garantindo maior controle e segurança.


## Descrição do Projeto
Este projeto é um sistema de rastreamento de motos desenvolvido em C# com .NET 8. Ele permite gerenciar clientes, motos, pátios e sensores de localização.
### Principais Funcionalidades:
- Cadastro e gerenciamento de clientes.
- Registro de motos com informações detalhadas.
- Associação de motos a pátios e clientes.
- Monitoramento de localização das motos por meio de sensores.
### Observações 
- Recomendamos que ao abrir a aplicação no swagger, começe criando primeiro um patio, depois crie o cliente, depois crie a moto pois so é possivel criar uma moto se existe um cliente e patio cadastrado, por ultimo apos criar a moto crie sensores de localização e vincule a essa moto.
- Não é possivel criar motos com o mesmo numero de chassi, pois chassi é um valor unico.
- Não é possivel criar clientes com o mesmo email, pois assim como o chassi, o email é unico.
---

## Rotas da API

### **Clientes**
- **GET /api/clientes**  
  Retorna a lista de todos os clientes cadastrados.

- **GET /api/clientes/{id}**  
  Retorna os detalhes de um cliente específico.

- **POST /api/clientes**  
  Cadastra um novo cliente.  
  **Body:**

- **POST /api/Cliente/login**  
  Realiza o login do cliente.  
  **Body:**  

- **DELETE /api/clientes/{id}**  
  Remove um cliente do sistema.

---

### **Health**
- **GET /api/Health**  
  Verifica o status de saúde da aplicação.  
  Retorna informações básicas indicando se a API está operacional.na documentacao do swagger tem o link da interface grafica.

---

### **Motos**
- **GET /api/motos**  
  Retorna a lista de todas as motos cadastradas.

- **GET /api/motos/{id}**  
  Retorna os detalhes de uma moto específica.

- **POST /api/motos**  
  Cadastra uma nova moto.  
  **Body:**
  
- **PUT /api/motos/{id}**  
  Atualiza os dados de uma moto existente.  
  **Body:**

- **DELETE /api/motos/{id}**  
  Remove uma moto do sistema.

- **POST /api/Moto/predict-status**  
  Realiza a previsão do status de uma moto com base nos dados informados.  
  **Body:**  
---

### **Pátios**
- **GET /api/patios**  
  Retorna a lista de todos os pátios cadastrados.

- **GET /api/patios/{id}**  
  Retorna os detalhes de um pátio específico.

- **POST /api/patios**  
  Cadastra um novo pátio.  
  **Body:**  

- **PUT /api/patios/{id}**  
  Atualiza os dados de um pátio existente.  
  **Body:**  

- **DELETE /api/patios/{id}**  
  Remove um pátio do sistema.

---

### **Sensores de Localização**
- **GET /api/sensores**  
  Retorna a lista de todas as localizações registradas.

- **GET /api/sensores/{id}**  
  Retorna os detalhes de uma localização específica.

- **POST /api/sensores**  
  Registra uma nova localização.  
  **Body:**

- **DELETE /api/sensores/{id}**  
  Remove um registro de localização.

---

## Instalação

### Pré-requisitos
- .NET 8 SDK instalado.
- Banco de dados configurado (ex.: SQL Server).
- Visual Studio 2022 ou outro editor compatível.

### Passos
1. Clone o repositório

2. Configure a string de conexão no arquivo `appsettings.json`

3. Execute as migrações para criar o banco de dados
   
4. Inicie o servidor

5. Acesse o swagger da API em: http://localhost:5001/index.html
   
6. Todos os endpoints tirando o de clientes vao estar bloqueados, crie um cliente ou faça login se voce ja tiver um cliente.

7. ao fazer o login e as credenciais estiverem certas, ira gerar um token, copie ele e cole no cadeado do swagger

8.os outros endpoints estarao liberados apos realizar a etapa anterior. 



## Descrição dos Testes Unitários

Este projeto utiliza o framework xUnit para testes unitários da lógica principal.  
Os testes estão localizados na pasta `Tests`. ao entrar nessa pasta clique em MotoServiceTest e execute os testes de la pelo gerenciador de testes. Os testes unitários do serviço de motos garantem o funcionamento correto das principais operações. Veja abaixo o que cada teste valida(obs: realizamos muitos testes na aplicação onde todos funcionaram, porem esses 3 testes abaixo sao os principais testes para a nossa motoservice):

- **Create_DeveRetornarMoto_QuandoDadosValidos**  
  Verifica se uma moto é criada corretamente quando os dados fornecidos são válidos. Utiliza uma implementação fake do serviço para simular a criação e compara a placa informada.

- **Create_DeveRetornarErro_QuandoNumeroChassiJaExiste**  
  Garante que o serviço retorna um erro ao tentar criar uma moto com número de chassi já existente no banco de dados. O teste utiliza um banco em memória para simular o cenário e espera uma mensagem de erro específica.

- **GetById_DeveRetornarNull_QuandoMotoNaoExiste**  
  Testa se o serviço retorna `null` ao buscar uma moto por um ID inexistente, utilizando banco de dados em memória para garantir o isolamento do teste.

Esses testes cobrem os principais cenários de criação e consulta de motos, assegurando que regras de negócio importantes sejam respeitadas.Alem desses testes de moto service, possuimos testes que testam as services das demais classes. 



## Integrantes

Barbara Dias Santos rm: 556974

Natasha Lopes Rocha Oliveira rm: 554816

Juan Pablo Ruiz de Souza rm: 557727


  
  
