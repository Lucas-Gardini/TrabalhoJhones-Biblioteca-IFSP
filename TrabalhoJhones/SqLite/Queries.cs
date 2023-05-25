using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace BibliotecaLD.SqLite {
    /// <summary>
    /// Classe responsável por realizar os comandos SQL, exportando uma maneira mais amigável de se trabalhar com as consultas (sem Join porque ia dar muito trabalho [qualquer coisa só usar entity framework])
    /// </summary>
    public class Queries {
        private readonly string _table;
        private readonly SQLiteConnection _sQLiteConnection;

        public Queries(SQLiteConnection sQLiteConnection, string table) {
            this._sQLiteConnection = sQLiteConnection;
            this._table = table;
        }

        /// <summary>
        /// Método responsável por executar a query (difere as querys de SELECT e as demais, já que a query de SELECT é um pouco diferente na execução)
        /// </summary>
        private object ExecuteQuery(string query) {
            object result = false;

            try {
                _sQLiteConnection.Open();

                // Se a query for do tipo select, ela terá um tratamento diferente
                if (query.Contains("SELECT")) {
                    // Instância do DataTable (contém os dados) e do adapter (executa a query)
                    DataTable dataTable = new DataTable();
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, _sQLiteConnection);

                    // Preenchendo a datatable com os dados
                    adapter.Fill(dataTable);

                    // Retornando os dados em forma de array/lista;
                    result = dataTable;
                } else {
                    // Criando uma instância de comando, direto na conexão
                    SQLiteCommand command = _sQLiteConnection.CreateCommand();

                    // Commando SQL
                    command.CommandText = query;

                    // Executando a query e liberando recursos
                    command.ExecuteNonQuery();
                    command.Dispose();

                    return true;
                }
            } catch (Exception ex) {
                // Exibindo o erro
                MessageBox.Show(ex.Message);

                // Retornando uma lista vazia, pois no caso do select não quebra o "DataGrid"
                result = new List<object>() { };
            } finally { _sQLiteConnection.Close(); }

            return result;
        }

        /// <summary>
        /// Método responsável por realizar a consulta no banco de dados
        /// </summary>
        /// <param name="fields">Campos separados por ',' que serão buscados</param>
        /// <param name="where">Cláusula WHERE em caso de busca específica</param>
        /// <param name="complement">Cláusulas extras a serem adicionadas a query</param>
        public object Select(string fields, string where = null, string complement = "") {
            // Gerando a query de seleção
            string query = $"SELECT {fields} FROM {_table}";

            // Adicionando o where, caso exista
            if (!string.IsNullOrEmpty(where)) query = $"{query} WHERE {where} {complement}";

            // Executando a query
            return this.ExecuteQuery(query);
        }

        /// <summary>
        /// Método responsável por realizar a inserção de dados no banco de dados
        /// </summary>
        /// <param name="fields">Campos separados por ',' que serão inseridos</param>
        /// <param name="values">Valores a serem adicionados, separados por ','</param>
        public object Insert(string fields, string values) {
            // Gerando a query de inserção
            string query = $"INSERT INTO {_table}({fields}) VALUES({values})";

            // Executando a query
            return this.ExecuteQuery(query);
        }

        /// <summary>
        /// Método responsável por atualizar um item no banco de dados
        /// </summary>
        /// <param name="toUpdate">Valores que serão atualizados separados por ','</param>
        /// <param name="condition">Condição WHERE</param>
        public object Update(string toUpdate, string condition) {
            // Gerando a query de atualização
            string query = $"UPDATE {_table} SET {toUpdate} WHERE {condition}";

            // Executando a query
            return this.ExecuteQuery(query);
        }

        /// <summary>
        /// Método responsável por excluir um item no banco de dados
        /// </summary>
        /// <param name="id">ID do item a ser excluído</param>
        public object Delete(string id) {
            // Gerando a query de exclusão
            string query = $"DELETE FROM {_table} WHERE id = {id}";

            return this.ExecuteQuery(query);
        }
    }
}
