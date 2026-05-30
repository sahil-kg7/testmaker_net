namespace testmaker.Application.Features.Schools.Queries.GetAllSchools;

public record SchoolDto(Guid Id, string Name, string? LogoFilename, DateTime CreatedOn, DateTime UpdatedOn);
