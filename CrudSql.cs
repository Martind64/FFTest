using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFTest.Model;

namespace FFTest.DataAccess
{
    public static class CrudSql
    {
        public static Member GetMember(int memberId)
        {
            string sql = @"SELECT MemberId, FirstName, LastName FROM [Member]
                           WHERE UserId = @UserId";
            var parms = new Dictionary<string, object>();
            parms.Add("@UserId", userId);
            return AdoDataAccess.Read(sql, MakeDataObject, parms);
        }
        public static ObservableCollection<Member> GetMembers()
        {
            string sql = @"SELECT MemberId, FirstName, LastName FROM [Member]";
            return AdoDataAccess.ReadList(sql, MakeDataObject);
        }

        public static void InsertMember(Member member)
        {
            string sql = @"INSERT INTO [User] (FirstName, LastName) VALUES (@FirstName, @LastName)";
            var parms = CreateParameters(member);
            member.MemberId = AdoDataAccess.Insert(sql, parms);

        }
        public static int UpdateMember(Member member)
        {
            string sql = @"UPDATE [User] SET FirstName = @FirstName, LastName = @LastName
                           WHERE UserId = @UserId";

            return AdoDataAccess.Update(sql, CreateParameters(member));
        }
        private static Func<IDataReader, Member> MakeDataObject = reader =>
            new Member
            {
                MemberId = Convert.ToInt32(reader["MemberId"]),
                FirstName = reader["FirstName"].ToString().Trim(),
                LastName = reader["LastName"].ToString().Trim(),
            };
        private static Dictionary<string, object> CreateParameters(Member member)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@MemberId", member.MemberId);
            parameters.Add("@FirstName", member.FirstName);
            parameters.Add("@LastName", member.LastName);
            return parameters;
        }
        public static class Extensions
        {
            public static string AsBase64String(this object item)
            {
                if (item == null) return null;
                return Convert.ToBase64String((byte[])item);
            }
            public static byte[] AsByteArray(this string s)
            {
                if (string.IsNullOrEmpty(s)) return null;
                return Convert.FromBase64String(s);
            }
        }
    }
}
