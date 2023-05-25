using BibliotecaLD.SqLite;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BibliotecaLD {
    /// <summary>
    /// Classe responsável pela criação do banco caso não exista, além de exportar uma outra classe customizada, para geração de comandos SQL
    /// </summary>
    public class Database {
        public bool Ok { get; set; }
        private static SQLiteConnection _sQLiteConnection;

        /// <summary>
        /// Gera o arquivo de banco necessário caso não exista, além de iniciar a conexão (mas não abri-la) e chamar a criação das tabelas
        /// </summary>
        public Database(string connectionPath) {
            if (!File.Exists(connectionPath)) {
                // Verificando se o banco existe
                var confirm = MessageBox.Show("Deseja criar o arquivo?", "Banco de dados não encontrado", MessageBoxButtons.YesNo);

                if (confirm == DialogResult.No) {
                    Ok = false;
                    Application.Exit();
                    return;
                }

                // Obtendo todo o caminho, até onde o arquivo do banco ficará
                List<string> pathSplitted = connectionPath.Split('\\').ToList();
                pathSplitted.RemoveAt(pathSplitted.Count - 1);
                string folder = string.Join("\\", pathSplitted);

                // Verificando e criando a pasta do banco caso não exista
                if (!Directory.Exists(folder)) {
                    Directory.CreateDirectory(folder);
                }

                // Criando o arquivo de banco de dados
                SQLiteConnection.CreateFile(connectionPath);
                MessageBox.Show($"Arquivo criado em {connectionPath}");
            }

            if (File.Exists(connectionPath)) {

                // Criando a conexão
                string connectionString = $"Data Source={connectionPath}; Version=3;";
                _sQLiteConnection = new SQLiteConnection(connectionString);

                // Criando as tabelas do banco e fechando a aplicação em caso de erro
                bool success = Models.CreateTables(_sQLiteConnection);
                if (!success) Application.Exit();

                Ok = true;
            }
        }

        /// <summary>
        /// Função responsável por exportar a classe "Queries", que por sua vez é responsável por realizar os comandos SQL
        /// </summary>
        public Queries From(string table) {
            return new Queries(_sQLiteConnection, table);
        }
    }
}
