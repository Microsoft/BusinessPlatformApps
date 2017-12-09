using System.Reflection;
using System.Text;

namespace RedditCore.DataModel
{
    public partial class Comment : IDocument
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
    }
}
