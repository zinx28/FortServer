namespace FortMatchmaker.src.App.Utilities.MongoDB.Helpers
{
    // Time changes... this just to make it superrr sexy jk
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
