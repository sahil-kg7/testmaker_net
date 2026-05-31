namespace testmaker.Application.Features.Questions.Common;

/// <summary>
/// Enum representing the different kinds of question types.
/// Used to determine which fields are valid for a given question type.
/// </summary>
internal enum QuestionTypeKind
{
    Generic,
    Mcq,
    MatchTheFollowing,
    FillInTheBlank,
    AssertionReason
}
