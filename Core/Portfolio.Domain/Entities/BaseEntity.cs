using System;

namespace Portfolio.Domain.Entities;

public abstract class BaseEntity
{
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
        }
}
