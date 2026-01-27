using System;
using System.Collections.Generic;

namespace testmaker.Domain.Entities;

public partial class QuestionDetail
{
    public Guid Id { get; set; }

    public int QuestionTypeId { get; set; }

    public Guid SubjectId { get; set; }

    public Guid Difficulty { get; set; }

    public int Marks { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public string? Mcq { get; set; }

    public string? MatchA { get; set; }

    public string? MatchB { get; set; }

    public string? FibWords { get; set; }

    public string? Reason { get; set; }

    public string? Assertion { get; set; }

    public virtual QuestionDifficulty DifficultyNavigation { get; set; } = null!;

    public virtual ICollection<QuestionImage> QuestionImages { get; set; } = new List<QuestionImage>();

    public virtual ICollection<QuestionSubquestionMap> QuestionSubquestionMapQuestions { get; set; } = new List<QuestionSubquestionMap>();

    public virtual ICollection<QuestionSubquestionMap> QuestionSubquestionMapSubquestions { get; set; } = new List<QuestionSubquestionMap>();

    public virtual QuestionType QuestionType { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
