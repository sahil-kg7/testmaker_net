using System;
using System.Collections.Generic;

namespace testmaker.Domain.Entities;

public partial class TestQuestionMap
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    public Guid QuestionId { get; set; }

    public int QuestionPosition { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}
