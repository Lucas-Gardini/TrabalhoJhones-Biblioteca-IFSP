using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace BibliotecaLD.SqLite {
    /// <summary>
    /// Classe responsável pela criação das tabelas do banco
    /// </summary>
    public class Models {

        /// <summary>
        /// Cria as tabelas no banco, caso não existam
        /// </summary>
        public static bool CreateTables(SQLiteConnection connection) {
            try {
                // Abrindo a conexão com o banco
                connection.Open();

                // Criando uma instância de comando, direto na conexão
                SQLiteCommand livros = connection.CreateCommand();

                // Commando SQL
                livros.CommandText = @"
                CREATE TABLE IF NOT EXISTS livros (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    title VARCHAR(50) NOT NULL,
                    author VARCHAR(50),
                    year INT
                )";

                // Executando a query e liberando recursos
                livros.ExecuteNonQuery();
                livros.Dispose();

                return true;
            } catch (Exception ex) {
                // Em caso de erro, exibe o erro e retorna false
                MessageBox.Show(ex.Message);
                return false;
            } finally { connection.Close(); }
        }
    }
}
