using System;
using System.Collections.Generic;

namespace testmaker.Domain.Entities;

public partial class Class
{
    public int ClassNumber { get; set; }

    public string ClassRoman { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
