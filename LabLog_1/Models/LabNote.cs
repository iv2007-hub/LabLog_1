
using Google.Cloud.Firestore;
namespace LabLog_1.Models;

// Indica que esta clase puede almacenarse como documento en Firestore
[FirestoreData]
public class LabNote
{
    
        [FirestoreProperty] public string Id { get; set; } = string.Empty;

        [FirestoreProperty] public string Title { get; set; } = string.Empty;

        [FirestoreProperty] public string Observation { get; set; } = string.Empty;

        [FirestoreProperty] public string Category { get; set; } = string.Empty;

        [FirestoreProperty] public int Priority { get; set; }

        [FirestoreProperty] public bool IsPublic { get; set; }

        [FirestoreProperty] public string Tags { get; set; } = string.Empty;

        [FirestoreProperty] public DateTime CreatedAt { get; set; }

        [FirestoreProperty] public string UserId { get; set; } = string.Empty;

    }
