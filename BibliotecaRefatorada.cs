using System;
using System.Collections.Generic;
using System.Linq;

namespace BibliotecaApp
{
    // Entidades
    public class Livro
    {
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string ISBN { get; set; }
        public bool Disponivel { get; set; } = true;
    }

    public class Usuario
    {
        public string Nome { get; set; }
        public int ID { get; set; }
    }

    public class Emprestimo
    {
        public Livro Livro { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataDevolucaoPrevista { get; set; }
        public DateTime? DataDevolucaoEfetiva { get; set; }
    }

    // Serviços
    public interface INotificador
    {
        void Notificar(string destinatario, string assunto, string mensagem);
    }

    public class EmailNotificador : INotificador
    {
        public void Notificar(string destinatario, string assunto, string mensagem)
        {
            Console.WriteLine($"E-mail para {destinatario} | Assunto: {assunto} | Mensagem: {mensagem}");
        }
    }

    public class SmsNotificador : INotificador
    {
        public void Notificar(string destinatario, string assunto, string mensagem)
        {
            Console.WriteLine($"SMS para {destinatario} | Mensagem: {mensagem}");
        }
    }

    public interface IMultaService
    {
        double CalcularMulta(DateTime dataPrevista, DateTime dataEfetiva);
    }

    public class MultaService : IMultaService
    {
        private const double ValorMultaPorDia = 1.0;

        public double CalcularMulta(DateTime dataPrevista, DateTime dataEfetiva)
        {
            if (dataEfetiva > dataPrevista)
            {
                return (dataEfetiva - dataPrevista).Days * ValorMultaPorDia;
            }
            return 0;
        }
    }

    public class BibliotecaService
    {
        private List<Livro> livros = new();
        private List<Usuario> usuarios = new();
        private List<Emprestimo> emprestimos = new();
        private readonly List<INotificador> notificadores;
        private readonly IMultaService multaService;

        public BibliotecaService(List<INotificador> notificadores, IMultaService multaService)
        {
            this.notificadores = notificadores;
            this.multaService = multaService;
        }

        public void AdicionarLivro(string titulo, string autor, string isbn)
        {
            livros.Add(new Livro { Titulo = titulo, Autor = autor, ISBN = isbn });
        }

        public void AdicionarUsuario(string nome, int id)
        {
            var usuario = new Usuario { Nome = nome, ID = id };
            usuarios.Add(usuario);
            NotificarTodos(usuario.Nome, "Bem-vindo à Biblioteca", "Você foi cadastrado com sucesso.");
        }

        public bool RealizarEmprestimo(int usuarioId, string isbn, int diasEmprestimo)
        {
            var livro = livros.FirstOrDefault(l => l.ISBN == isbn && l.Disponivel);
            var usuario = usuarios.FirstOrDefault(u => u.ID == usuarioId);

            if (livro == null || usuario == null) return false;

            livro.Disponivel = false;

            var emprestimo = new Emprestimo
            {
                Livro = livro,
                Usuario = usuario,
                DataEmprestimo = DateTime.Now,
                DataDevolucaoPrevista = DateTime.Now.AddDays(diasEmprestimo)
            };

            emprestimos.Add(emprestimo);

            NotificarTodos(usuario.Nome, "Empréstimo Realizado", $"Você pegou o livro '{livro.Titulo}'.");

            return true;
        }

        public double RealizarDevolucao(int usuarioId, string isbn)
        {
            var emprestimo = emprestimos.FirstOrDefault(e =>
                e.Usuario.ID == usuarioId &&
                e.Livro.ISBN == isbn &&
                e.DataDevolucaoEfetiva == null);

            if (emprestimo == null) return -1;

            emprestimo.DataDevolucaoEfetiva = DateTime.Now;
            emprestimo.Livro.Disponivel = true;

            double multa = multaService.CalcularMulta(
                emprestimo.DataDevolucaoPrevista,
                emprestimo.DataDevolucaoEfetiva.Value
            );

            if (multa > 0)
            {
                NotificarTodos(emprestimo.Usuario.Nome, "Multa por Atraso", $"Você tem uma multa de R$ {multa}");
            }

            return multa;
        }

        private void NotificarTodos(string destinatario, string assunto, string mensagem)
        {
            foreach (var notificador in notificadores)
            {
                notificador.Notificar(destinatario, assunto, mensagem);
            }
        }

        // Métodos auxiliares
        public List<Livro> BuscarLivros() => livros;
        public List<Usuario> BuscarUsuarios() => usuarios;
        public List<Emprestimo> BuscarEmprestimos() => emprestimos;
    }

    // Programa principal
    class Program
    {
        static void Main(string[] args)
        {
            var notificadores = new List<INotificador>
            {
                new EmailNotificador(),
                new SmsNotificador()
            };

            var biblioteca = new BibliotecaService(notificadores, new MultaService());

            biblioteca.AdicionarLivro("Clean Code", "Robert C. Martin", "978-0132350884");
            biblioteca.AdicionarUsuario("João Silva", 1);

            biblioteca.RealizarEmprestimo(1, "978-0132350884", 7);

            // Simula devolução com atraso
            System.Threading.Thread.Sleep(2000); // Apenas para simular tempo passando
            double multa = biblioteca.RealizarDevolucao(1, "978-0132350884");
            Console.WriteLine($"Multa: R$ {multa}");

            Console.ReadLine();
        }
    }
}
