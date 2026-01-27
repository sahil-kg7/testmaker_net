using System;
using System.Collections.Generic;

namespace testmaker.Domain.Entities;

public partial class QuestionSubquestionMap
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    public Guid QuestionId { get; set; }

    public Guid SubquestionId { get; set; }

    public int SubquestionNumber { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public virtual QuestionDetail Question { get; set; } = null!;

    public virtual QuestionDetail Subquestion { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
