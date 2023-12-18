# Criando Soluções Robustas no Dynamics 365 com a Extensão SDK

Solução do *Desafio de Projeto: "Criando Soluções Robustas no Dynamics 365 com a Extensão SDK"* do Bootcamp Avanade - Programação C# com CRM Dynamics.  
Este desafio propõe o desenvolvimento de sisversas extensões para o sistema do Dynamics 365 utilizando o SDK do Dynamics. O projeto inclui a criação de plugins, actions e workflow assemblies.

O sistema deve conter:
- 4 Plugins
	- Pre-Validation: Valida se o campo Telefone Principal da tabela de Contas foi preenchido.
	- Pre-Operation: Associa o contato principal da Conta ao contato com o mesmo número de telefone da Conta.
	- Post-Operation: Cria uma task baseada no campo Site da Conta. Também valida se o campo Site foi preenchido.
	- Assíncrono: Cria vários contatos vinculados à conta.
- 2 Actions
	- Interação com Dataverse: Cria um registro de Cliente Potencial. Essa action é chamada por um botão do Ribbon Workbench.
	- Integração com API: Acessa a API ViaCEP para preencher os campos de endereço do Cliente Potencial com base no valor do CEP.  
	O desafio utiliza essa action em um Ribbon, mas decidi fazer através do evento OnChange do campo CEP.
- 2 Workflow Assemblies:
	- Validação: Validação que não permite a um Aluno se matricular em mais de 2 cursos ao mesmo tempo.
	- Criação de Registros: Cria registros de Calendário do Aluno para todas as aulas de um curso.
	Para estes Workflows, foi recriada a estrutura de tabelas de Alunos e Cursos utilizada no Desafio de Projeto anterior. 

O repositório contem todos os códigos e scripts utilizados no desenvolvimento das extensões.  
Ele também contém duas soluções do Dynamics 365:
- A solução com todos os componentes utilizados no desafio, incluindo as tabelas de Contas e Clinetes Potenciais, além das tabelas criadas de Aluno, Curso, Curso x Aluno e Calendário do Aluno.  
Ela também possui um aplicativo Model Driven especificamente desenvolvido para permitir acesso a todas as tabelas relevantes usadas no desafio.
- A solução utilizada para a criação do botão no Ribbon Workbench.

Nota: Os componentes utilizados nesse desafio foram refeitos do zero com base nas especificações dos desenvolvimentos necessários. Esta solução não foi criada em conjunto com a solução do Desafio de Projeto 02.