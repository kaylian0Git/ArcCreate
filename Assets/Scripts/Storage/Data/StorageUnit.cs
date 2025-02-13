using System;
using System.Collections.Generic;
using UltraLiteDB;

namespace ArcCreate.Storage.Data
{
    public abstract class StorageUnit<T> : IStorageUnit
        where T : StorageUnit<T>
    {
        [BsonId] public int Id { get; set; }

        public string Identifier { get; set; }

        public int Version { get; set; }

        public List<string> FileReferences { get; set; }

        public DateTime AddedDate { get; set; }

        public abstract string Type { get; }

        public bool IsDefaultAsset { get; set; }

        public void Delete()
        {
            foreach (string refr in FileReferences)
            {
                FileStorage.DeleteReference(string.Join("/", Type, Identifier, refr));
            }

            Database.Current.GetCollection<T>().Delete(Id);
        }

        public IStorageUnit GetConflictingIdentifier()
        {
            return Database.Current.GetCollection<T>().FindOne(Query.EQ("Identifier", Identifier));
        }

        public void Insert()
        {
            Database.Current.GetCollection<T>().Insert(this as T);
        }

        public string GetRealPath(string virtualPath)
        {
            return FileStorage.GetFilePath(string.Join("/", Type, Identifier, virtualPath));
        }

        public virtual bool ValidateSelf(out string reason)
        {
            if (!string.IsNullOrEmpty(Identifier))
            {
                reason = "Identifier is empty";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        public bool Equals(IStorageUnit other)
        {
            return other != null && Type == other.Type && Id == other.Id;
        }

        public override int GetHashCode()
        {
            int hashCode = -31773061;
            hashCode = (hashCode * -1521134295) + Id.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Type);
            return hashCode;
        }
    }
}