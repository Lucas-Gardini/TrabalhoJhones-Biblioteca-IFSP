namespace BibliotecaLD.SqLite {
    /// <summary>
    /// Classe/Objeto para facilitar o envio de dados que serão atualizados de um método/classe a outro(a)
    /// </summary>
    public class Update {
        public string Id { get; set; }
        public string Title { get; set; } = null;
        public string Author { get; set; } = null;
        public string Year { get; set; } = null;
    }
}
