using BibliotecaLD.SqLite;
using System;
using System.Data;
using System.Windows.Forms;

namespace BibliotecaLD {
    public partial class Main : Form {
        private Database _database;

        public Main() {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e) {
            // Instânciando a classe do banco
            this._database = new Database(Application.StartupPath + "\\db\\database.db");

            if (!_database.Ok) {
                Application.Exit();
                return;
            }

            // Carregando os dados da tabela
            RefreshData();
        }

        /// <summary>
        /// Método responsável por atualizar os dados da tabela (ou carregá-los pela primeira vez)
        /// </summary>
        private void RefreshData() {
            // Limpa os dados da tabela
            DataGridBooks.Rows.Clear();

            // Obtém os dados do banco, convertendo o tipo de retorno genérico (object), para DataTable
            var data = (DataTable)this._database.From("livros").Select("*");

            // Loopando pelo resultado e adicionando os itens na row
            foreach (DataRow row in data.Rows) {
                DataGridBooks.Rows.Add(row.ItemArray);
            }

            DataGridBooks.ClearSelection();
        }

        /// <summary>
        /// Método responsável por abrir a edição de uma linha selecionada pelo usuário
        /// </summary>
        private void EditRow() {
            if (DataGridBooks.SelectedCells.Count == 0) return;

            // Obtém o índice da linha selecionada
            var rowIndex = DataGridBooks.SelectedCells[0].RowIndex;

            // Obtém o item selecionado
            var selectedBook = DataGridBooks.Rows[rowIndex].Cells;

            // Instancia uma nova classe/objeto do tipo Update, passando os valores da linha selecionada
            var book = new Update() {
                Id = selectedBook[0].Value.ToString(),
                Title = selectedBook[1].Value.ToString(),
                Author = selectedBook[2].Value.ToString(),
                Year = selectedBook[3].Value.ToString()
            };

            // Abre o popup de edição, passando o objeto criado
            new PopupForm(_database, book).ShowDialog();

            // Atualiza os dados da tabela
            RefreshData();
        }

        /// <summary>
        /// Método responsável por excluir uma linha selecionada pelo usuário
        /// </summary>
        private void DeleteRow() {
            if (DataGridBooks.SelectedCells.Count == 0) return;

            // Obtém o índice da linha selecionada
            var rowIndex = DataGridBooks.SelectedCells[0].RowIndex;

            // Obtém o item selecionado
            var selectedBook = DataGridBooks.Rows[rowIndex].Cells;

            // Deleta o item do banco
            _database.From("livros").Delete(selectedBook[0].Value.ToString());

            // Atualiza os dados da tabela
            RefreshData();
        }

        private void BtnAdd_Click(object sender, EventArgs e) {
            // Abre o popup de adição
            new PopupForm(_database).ShowDialog();

            // Atualiza os dados da tabela
            RefreshData();
        }

        private void BtnBuscar_Click(object sender, EventArgs e) {
            // Limpa os dados da tabela
            DataGridBooks.Rows.Clear();

            // Obtendo coluna a ser filtrada (basicamente convertendo a palavra de portugues para ingles)
            var field = "";
            if (ComboFields.SelectedItem != null) {
                switch (ComboFields.SelectedItem.ToString()) {
                    case "Título":
                        field = "title";
                        break;
                    case "Autor":
                        field = "author";
                        break;
                    case "Ano":
                        field = "year";
                        break;
                }
            }

            // Obtém os dados do banco, convertendo o tipo de retorno genérico (object), para DataTable e filtrando por um campo em específico
            // O ternário, serve para caso seja ano, como no banco é int, o where ser feito sem aspas
            var data = (DataTable)this._database
                .From("livros")
                .Select("*", !string.IsNullOrEmpty(field) ? $"{field}{(field == "year" ? $" = {TextSearch.Text}" : $" LIKE '{TextSearch.Text}%'")}" : "");

            // Loopando pelo resultado e adicionando os itens na row
            foreach (DataRow row in data.Rows) {
                DataGridBooks.Rows.Add(row.ItemArray);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e) {
            // Limpando os campos e tabela
            TextSearch.Clear();
            ComboFields.SelectedIndex = -1;
            DataGridBooks.Rows.Clear();
        }

        private void DataGridBooks_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) {
            EditRow();
        }

        private void BtnEdit_Click(object sender, EventArgs e) {
            EditRow();
        }

        private void BtnDelete_Click(object sender, EventArgs e) {
            DeleteRow();
        }

        private void DataGridBooks_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete) DeleteRow();
        }
    }
}
