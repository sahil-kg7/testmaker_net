using System;
using System.Collections.Generic;

namespace testmaker.Domain.Entities;

public partial class QuestionDifficulty
{
    public Guid Id { get; set; }

    public string Level { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public virtual ICollection<QuestionDetail> QuestionDetails { get; set; } = new List<QuestionDetail>();
}
