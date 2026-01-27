using System;
using System.Collections.Generic;
using testmaker.Domain.Entities;

namespace testmaker.Domain.Entities;

public partial class Test
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = null!;

    public Guid? SchoolId { get; set; }

    public int? ClassNumber { get; set; }

    public Guid? SubjectId { get; set; }

    public Guid? TestTypeId { get; set; }

    public int? SectionCount { get; set; }

    public int TimeDuration { get; set; }

    public int MaximumMarks { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public virtual Class? ClassNumberNavigation { get; set; }

    public virtual ICollection<QuestionSubquestionMap> QuestionSubquestionMaps { get; set; } = new List<QuestionSubquestionMap>();

    public virtual School? School { get; set; }

    public virtual Subject? Subject { get; set; }

    public virtual TestType? TestType { get; set; }
}
