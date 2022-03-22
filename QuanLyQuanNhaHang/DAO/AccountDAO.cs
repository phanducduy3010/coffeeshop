using System;
using QuanLyQuanCaPhe.DTO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCaPhe.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance 
        { 
            get
            {
                if (instance == null) instance = new AccountDAO();
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        private AccountDAO() { }

        public bool Login1(string userName, string passWord)
        {
            string query = " SELECT * FROM Account WHERE UserName = N'" + userName + "' AND PassWord1 = N'" + passWord + "' ";

            DataTable result = DataProvider.Instance.ExecuteQuery(query);
            
            return result.Rows.Count > 0;
        }

        public bool UpdateAccount(string userName, string displayName, string pass, string newpass)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateAccount @userName , @displayName , @password , @newPassword", new object[] { userName, displayName, pass, newpass });
            return result > 0;
        }

        public bool InsertAccount(string name, string displayname, int type)
        {
            string query = string.Format("INSERT Account ( UserName, DisplayName, Type )VALUES  ( N'{0}', N'{1}', {2})", name, displayname, type);
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { name, displayname, type });

            return result > 0;
        }

        public bool UpdateAccount1(string name, string displayname, int type)
        {
            string query = string.Format("UPDATE Account SET DisplayName = N'{1}', Type = {2} WHERE UserName = N'{0}'", name, displayname, type);
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] {name, displayname, type });

            return result > 0;
        }

        public bool DeleteAccount(string name)
        {
            string query = string.Format("Delete Account where UserName = N'{0}'", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { name });

            return result > 0;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("Select UserName, DisplayName, Type from Account");
        }

        public bool ResetPassword(string name)
        {
            string query = string.Format("update Account set Password1 = N'111111' where UserName = N'{0}'", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { name });

            return result > 0;
        }

        public Account GetAccountByUserName(string userName)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("select * from Account where UserName = N'" + userName + "'");

            foreach(DataRow item in data.Rows)
            {
                return new Account(item);
            }
            return null;
        }
    }
}
