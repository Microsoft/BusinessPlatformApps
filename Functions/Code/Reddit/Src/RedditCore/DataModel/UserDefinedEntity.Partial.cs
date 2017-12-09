using System;
using System.Reflection;
using System.Text;

namespace RedditCore.DataModel
{
    public partial class UserDefinedEntity : IEquatable<UserDefinedEntity>
    {
        public override string ToString()
        {
            var flags = BindingFlags.Instance | BindingFlags.Public |
                        BindingFlags.FlattenHierarchy;
            var infos = GetType().GetProperties(flags);

            var sb = new StringBuilder();

            var typeName = GetType().Name;
            sb.Append(typeName);

            sb.Append("[");
            foreach (var info in infos)
            {
                var value = info.GetValue(this, null);
                sb.AppendFormat("{0}: {1},", info.Name, value != null ? value : "null");
            }
            sb.Append("]");

            return sb.ToString();
        }

        public bool Equals(UserDefinedEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DocumentId == other.DocumentId 
                && string.Equals(Entity, other.Entity, StringComparison.OrdinalIgnoreCase) 
                && string.Equals(EntityType, other.EntityType, StringComparison.OrdinalIgnoreCase) 
                && EntityOffset == other.EntityOffset 
                && EntityLength == other.EntityLength 
                && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserDefinedEntity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DocumentId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Entity != null ? Entity.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EntityType != null ? EntityType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ EntityOffset.GetHashCode();
                hashCode = (hashCode * 397) ^ EntityLength.GetHashCode();
                hashCode = (hashCode * 397) ^ Id.GetHashCode();
                return hashCode;
            }
        }
    }
}
