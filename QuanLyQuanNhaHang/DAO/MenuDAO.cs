using QuanLyQuanCaPhe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCaPhe.DAO
{
    class MenuDAO
    {

        private static MenuDAO instance;
        public static MenuDAO Instance
        {
            get { if (instance == null) instance = new MenuDAO(); return MenuDAO.instance; }
            private set { MenuDAO.instance = value; }
        }

        private MenuDAO() { }

        public List<Menu1> GetListMenuByTable(int id)
        {
            List<Menu1> listMenu = new List<Menu1>();

            string query = "select f.name, b1.count, f.price, f.price*b1.count as totalPrice from BillInfo as b1, Bill as b2, Food as f where b1.idBill = b2.id and status = 0 and b1.idFood = f.id and b2.idTable = " +id ;
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Menu1 menu = new DTO.Menu1(item);
                listMenu.Add(menu);
            }

            return listMenu;
        }
    }
}
