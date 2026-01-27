using System;
using System.Collections.Generic;

namespace testmaker.Domain.Entities;

public partial class QuestionType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public virtual ICollection<QuestionDetail> QuestionDetails { get; set; } = new List<QuestionDetail>();
}
