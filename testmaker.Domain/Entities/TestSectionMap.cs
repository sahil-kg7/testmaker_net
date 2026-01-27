using System;
using System.Collections.Generic;

namespace testmaker.Domain.Entities;

public partial class TestSectionMap
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    public int SectionNumber { get; set; }

    public int InitialQuesNumber { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}
