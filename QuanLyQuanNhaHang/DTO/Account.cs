using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCaPhe.DTO
{
    public class Account
    {
        public Account(string userName, string displayName, int type, string password = null)
        {
            this.Username = userName;
            this.Displayname = displayName;
            this.Type = type;
            this.Password = password;
        }

        public Account(DataRow row)
        {
            this.Username = row["UserName"].ToString();
            this.Displayname = row["DisplayName"].ToString();
            this.Type = (int)row["Type"];
            this.Password = row["Password1"].ToString();
        }

        private string userName;
        public string Username
        {
            get { return userName; }
            set { userName = value; }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string displayName;
        public string Displayname
        {
            get { return displayName; }
            set { displayName = value; }
        }

        private int type;
        public int Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}
