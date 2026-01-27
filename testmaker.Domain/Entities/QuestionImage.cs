using System;
using System.Collections.Generic;

namespace testmaker.Domain.Entities;

public partial class QuestionImage
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public int ImagePosition { get; set; }

    public string ImageName { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public virtual QuestionDetail Question { get; set; } = null!;
}
