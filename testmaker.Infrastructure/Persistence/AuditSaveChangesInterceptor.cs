using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var timestamp = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is null)
                continue;

            if (IsDatabaseManagedAuditEntity(entry.Entity))
                continue;

            SetAuditTimestamps(entry, timestamp);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetAuditTimestamps(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, DateTime timestamp)
    {
        var createdOnProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedOn");
        var updatedOnProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedOn");

        if (createdOnProp is null && updatedOnProp is null)
            return;

        if (entry.State == EntityState.Added)
        {
            if (createdOnProp is not null)
                createdOnProp.CurrentValue = timestamp;
            if (updatedOnProp is not null)
                updatedOnProp.CurrentValue = timestamp;
            return;
        }

        if (entry.State == EntityState.Modified && updatedOnProp is not null)
        {
            updatedOnProp.CurrentValue = timestamp;
        }
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        return result;
    }

    private static bool IsDatabaseManagedAuditEntity(object entity)
    {
        return entity is QuestionDetail
            or QuestionImage
            or Test
            or TestQuestionMap
            or QuestionSubquestionMap;
    }
}
