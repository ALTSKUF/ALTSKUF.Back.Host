namespace AltSKUF.Domain.Entity
{
    abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EditAt { get; set; }
    }
}
