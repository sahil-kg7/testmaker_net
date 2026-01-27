using testmaker.Domain.Common;

namespace testmaker.Domain.Entities;

public class School : IEntity
{
    Guid Id { get; set; }
    DateTime IEntity.CreatedAt { get; set; }
    DateTime IEntity.UpdatedAt { get; set; }
}