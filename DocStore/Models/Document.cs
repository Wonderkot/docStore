namespace DocStore.Models
{
    /// <summary>
    /// Класс, описывающий сущность документа в БД
    /// </summary>
    public class Document
    {
        public int Id { get; set; }
        public User User { get; set; }
        public byte[] Source { get; set; }
    }
}