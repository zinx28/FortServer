namespace FortBackend.src.App.Utilities.MongoDB.Helpers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BsonCollectionNameAttribute : Attribute
    {
        public string CollectionName { get; }

        public BsonCollectionNameAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
