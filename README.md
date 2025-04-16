# Sistema de Gerenciamento de Biblioteca

Este projeto é uma refatoração de um sistema simples de gerenciamento de biblioteca com o objetivo de aplicar os princípios **SOLID** e boas práticas de **Clean Code**.

---

## ✅ Parte 1: Violações Identificadas no Código Original

### 1. 📛 SRP Violado (Single Responsibility Principle)
- **Classe**: `GerenciadorBiblioteca`
- **Problema**: A classe possui múltiplas responsabilidades (gerenciar dados, controlar fluxo de empréstimos, enviar notificações).
- **Impacto**: Dificulta manutenção, leitura e testes unitários.

---

### 2. 📛 OCP Violado (Open/Closed Principle)
- **Método**: `RealizarEmprestimo`
- **Problema**: Toda nova notificação (ex: Push Notification) exige modificação do método.
- **Impacto**: Aumenta acoplamento e dificulta a extensibilidade.

---

### 3. 📛 DIP Violado (Dependency Inversion Principle)
- **Classe**: `GerenciadorBiblioteca`
- **Problema**: Depende diretamente de implementações concretas de envio de email e SMS.
- **Impacto**: Torna o código rígido e difícil de testar (ex: testes sem envio real de mensagem).

---

### 4. 📛 Clean Code Violado - Métodos longos e acoplados
- **Método**: `RealizarEmprestimo`, `AdicionarUsuario`
- **Problema**: Métodos grandes e com múltiplas tarefas.
- **Impacto**: Código difícil de entender, testar e reutilizar.

---

### 5. 📛 Código duplicado / baixa coesão
- **Método**: `AdicionarUsuario`
- **Problema**: Envio de e-mail dentro da lógica de cadastro.
- **Impacto**: Mistura de responsabilidades e violação da separação de preocupações.

---

## ✅ Parte 2: Refatoração Aplicada

A versão refatorada do sistema aplica:

- **SRP**: Cada classe possui uma única responsabilidade.
- **OCP**: É possível adicionar notificadores sem alterar a lógica principal.
- **DIP**: A lógica depende de abstrações (`INotificador`, `IMultaService`).
- **Clean Code**: Código com nomes claros, bem dividido, sem duplicações e com foco em legibilidade e manutenção.



