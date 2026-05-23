using System;
using System.Collections.Generic;

namespace testmaker.Domain.Entities;

public partial class Class
{
    public Guid Id { get; set; }

    public string ClassName { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
