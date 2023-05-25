using BibliotecaLD.SqLite;
using System;
using System.Windows.Forms;

namespace BibliotecaLD {
    public partial class PopupForm : Form {
        private int _idToDelete = -1;
        private Database _database;

        public PopupForm(Database database, Update toUpdate = null) {
            _database = database;
            InitializeComponent();

            // Se o objeto for nulo, significa que o usuário está adicionando um novo livro, caso contrário, preenche os campos para edição
            if (toUpdate != null) {
                _idToDelete = int.Parse(toUpdate.Id);
                TxtTitulo.Text = toUpdate.Title;
                TxtAutor.Text = toUpdate.Author;
                TxtAno.Text = toUpdate.Year;
            }
        }

        /// <summary>
        /// Função que altera o título do formulário dependendo da ação (adição/exclusão)
        /// </summary>
        private void PopupForm_Load(object sender, EventArgs e) {
            if (_idToDelete != -1) {
                this.Text = TxtTitulo.Text;
                LabelTitle.Text = "Editar livro";
            } else {
                this.Text = "Adicionar livro";
                LabelTitle.Text = "Adicionar livro";

            }
        }

        /// <summary>
        /// Limpa os campos
        /// </summary>
        private void BtnClear_Click(object sender, EventArgs e) {
            TxtTitulo.Text = "";
            TxtAutor.Text = "";
            TxtAno.Text = "";
        }

        /// <summary>
        /// Fecha o formulário, pergunta se o usuário deseja sair sem salvar caso existam campos preenchidos
        /// </summary>
        private void BtnClose_Click(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(TxtAno.Text) || !string.IsNullOrEmpty(TxtAutor.Text) || !string.IsNullOrEmpty(TxtTitulo.Text)) {
                var close = MessageBox.Show("Sair sem salvar?", "Existem itens sem salvar", MessageBoxButtons.YesNo);
                if (close == DialogResult.No) return;
            }
            this.Close();
        }

        /// <summary>
        /// Salva os dados no banco
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e) {
            // Verificando se os campos estão preenchidos
            if (string.IsNullOrEmpty(TxtAno.Text) || string.IsNullOrEmpty(TxtAutor.Text) || string.IsNullOrEmpty(TxtTitulo.Text)) {
                MessageBox.Show("Preencha todas as informações!");
                return;
            }

            // Se o id for -1, significa que o usuário está adicionando um novo livro, caso contrário, atualiza o livro
            if (_idToDelete == -1) {
                var result = _database.From("livros").Insert(
                    "title, author, year",
                    $"'{TxtTitulo.Text}', '{TxtAutor.Text}', {TxtAno.Text}"
                );

                // Se o resultado for true, significa que o livro foi adicionado com sucesso
                if (result.Equals(true)) {
                    TxtTitulo.Text = "";
                    TxtAutor.Text = "";
                    TxtAno.Text = "";

                    MessageBox.Show("Livro criado com sucesso!");
                }

                // obs: Caso a query dê errado, a própria classe do Database já exibirá o erro a partir de uma messagebox
            } else {
                var result = _database.From("livros").Update(
                    $"title = '{TxtTitulo.Text}', author = '{TxtAutor.Text}', year = {TxtAno.Text}",
                    $"id = {_idToDelete}"
                );

                // Se o resultado for true, significa que o livro foi atualizado com sucesso
                if (result.Equals(true)) {
                    TxtTitulo.Text = "";
                    TxtAutor.Text = "";
                    TxtAno.Text = "";

                    MessageBox.Show("Livro atualizado com sucesso!");

                    this.Close();
                }

                // obs: Caso a query dê errado, a própria classe do Database já exibirá o erro a partir de uma messagebox
            }
        }
    }
}