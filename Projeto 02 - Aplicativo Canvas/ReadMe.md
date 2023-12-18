# Criando um Aplicativo Tipo Canvas com a Power Platform

Solução do *Desafio de Projeto: "Criando um Aplicativo Tipo Canvas com a Power Platform"* do Bootcamp Avanade - Programação C# com CRM Dynamics.  
Este desafio propõe a criação de um sistema de cadastro de cursos, instrutores e alunos, além da criação de um aplicativo Model Driven para acessar os formulários e exibições, um aplicativo Canvas para cadastro de instrutores, que será disponibilizado pelo formulário da entidade de Cursos, e um fluxo do Power Automate para enviar um e-mail ao instrutor que for adicionado a um curso.

O sistema deve conter:
- 5 Tabelas.
	- Cursos: Os cursos disponibilizados para estudo
	- Instrutores: As pessoas repsonsáveis por ensinar no curso
	- Alunos: Os alunos que poderão se inscrever nos cursos
	- Cursos x Instrutores: Faz a relação de instrutores com cursos
	- Cursos x Alunos: Faz a relação de alunos com cursos
- Um Aplicativo Model Driven, usado para poder acessar os formulários e visualizações dessas tabelas.
- Um Aplicativo Canvas, que será usado para cadastrar/remover os instrutores dos cursos. O aplicativo deve ser acessado a partir do formulário de Cursos, e permitir adicionar ou remover instrutores do curso atual.
- Um Fluxo do Power Automate, que deve enviar um e-mail para o instrutor quando ele é adicionado a um curso.

O repositório contem a solução com todos os componentes criados para o desafio.

Esta solução do desafio também conta com algumas melhorias extras:
- Um script em JavaScript para validação do CPF dos alunos.
- Implementação completa de formulários e exibições para todas as tabelas, incluindo as tabelas de Alunos e da relação Curso x Aluno.
- Regra de Negócio para validar o campo de valor do curso baseado no tipo do curso.
- Validação dentro do aplicativo Canvas para que o Instrutor não seja nulo.