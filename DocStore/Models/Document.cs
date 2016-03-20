namespace DocStore.Models
{
    /// <summary>
    /// Класс, описывающий сущность документа в БД
    /// </summary>
    public class Document
    {
        public virtual int Id { get; set; }
        public virtual User User { get; set; }
        public virtual byte[] Source { get; set; }
    }
}