using LabLog_1.DTos;
using LabLog_1.Models;

namespace LabLog_1.Services;

public class LabNoteService
{
    private readonly FirebaseService _firebaseService;

    public LabNoteService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<LabNote> CreateAsync(CreateLabNoteDTo dto, string userId)
    {
        var note = new LabNote
        {
            Id = Guid.NewGuid().ToString(),
            Title = dto.Title,
            Observation = dto.Observation,
            Category = dto.Category,
            Priority = dto.Priority,
            IsPublic = dto.IsPublic,
            Tags = dto.Tags,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        var collection = _firebaseService.GetCollection("labnotes");
        await collection.Document(note.Id).SetAsync(new Dictionary<string, object>
        {
            { "Id", note.Id },
            { "Title", note.Title },
            { "Observation", note.Observation },
            { "Category", note.Category },
            { "Priority", note.Priority },
            { "IsPublic", note.IsPublic },
            { "Tags", note.Tags },
            { "CreatedAt", note.CreatedAt },
            { "UserId", note.UserId }
        });

        return note;
    }

    public async Task<List<LabNote>> GetByUserAsync(string userId)
    {
        var collection = _firebaseService.GetCollection("labnotes");
        var snapshot = await collection
            .WhereEqualTo("UserId", userId)
            .GetSnapshotAsync();

        var notes = new List<LabNote>();
        foreach (var doc in snapshot.Documents)
        {
            var data = doc.ToDictionary();
            notes.Add(new LabNote
            {
                Id = data["Id"].ToString()!,
                Title = data["Title"].ToString()!,
                Observation = data["Observation"].ToString()!,
                Category = data["Category"].ToString()!,
                Priority = Convert.ToInt32(data["Priority"]),
                IsPublic = (bool)data["IsPublic"],
                Tags = data["Tags"].ToString()!,
                CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime(),
                UserId = data["UserId"].ToString()!
            });
        }

        return notes;
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var collection = _firebaseService.GetCollection("labnotes");
        var snapshot = await collection.Document(id).GetSnapshotAsync();

        if (!snapshot.Exists) return false;

        var data = snapshot.ToDictionary();
        if (data["UserId"].ToString() != userId) return false;

        await collection.Document(id).DeleteAsync();
        return true;
    }
}