using QuanLyQuanCaPhe.DAO;
using QuanLyQuanCaPhe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCaPhe
{
    public partial class fManager : Form
    {
        
        private Account loginAccount;

        public Account LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount.Type); }
        }
        public fManager( Account acc)
        {
            InitializeComponent();
            this.LoginAccount = acc;
            LoadComboboxTable(cbTable);
            LoadTable();
            LoadCategory();
        }

        #region Method
        void ChangeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            thôngTinTàiKhoảnToolStripMenuItem.Text += " (" + loginAccount.Displayname + ")";
        }
        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }

        void LoadFoodList(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }

        void LoadTable()
        {
            pnlTable.Controls.Clear();
            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWitdh, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;// luu luon table
                switch (item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.Aqua;
                        break;
                    default:
                        btn.BackColor = Color.Pink;
                        break;
                }

                pnlTable.Controls.Add(btn);
            }
            LoadComboboxTable(cbTable);
        }
        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<DTO.Menu1> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float totalPrice = 0;
            foreach(DTO.Menu1 item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice; 
                lsvBill.Items.Add(lsvItem);
            }
            CultureInfo culture = new CultureInfo("vi-VN");  

            txbTotalPrice.Text = totalPrice.ToString("c", culture);
            
        }

        void LoadComboboxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableListByStatus();
            cb.DisplayMember = "Name";
        }

        #endregion

        #region Events
        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag; 
            ShowBill(tableID);
        }
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = LoginAccount;
            f.InsertFood  += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.UpdateFood += f_UpdateFood;
            f.ShowDialog();
        }

        private void f_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodList((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null) ShowBill((lsvBill.Tag as Table).ID);
        }

        private void f_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodList((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null) ShowBill((lsvBill.Tag as Table).ID);
            LoadTable();
        }

        private void f_InsertFood(object sender, EventArgs e)
        {
            LoadFoodList((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null) ShowBill((lsvBill.Tag as Table).ID);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null) return;

            Category selected = cb.SelectedItem as Category;
            id = selected.ID;

            LoadFoodList(id);
        }

        private void addFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            if(table == null)
            {
                MessageBox.Show("Hãy chọn bàn");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int foodID = (cbFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;

            if (idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }

            ShowBill(table.ID);
            LoadTable();
        }
        
        private void btnCheckout_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int discount = (int)nmDiscount.Value;
            double totalPrice = Convert.ToDouble(txbTotalPrice.Text.Split(',')[0]);
            double finalTotalPrice = totalPrice - (totalPrice/100)*discount;

            if(idBill != -1)
            {
                if (MessageBox.Show(string.Format("Bạn muốn thanh toán hóa đơn cho bàn {0}\n\nTổng tiền:      {1} 000 (VND)\n\n Giảm giá:         {2}%\n\nThành tiền:      {3} 000 (VND)", table.Name, totalPrice, discount, finalTotalPrice), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill, discount,(float)totalPrice);
                    ShowBill(table.ID);

                    LoadTable();
                }
            } else
            {
                MessageBox.Show("Bàn trống!");
            }
        }

        #endregion

        private void ChangeTable_Click(object sender, EventArgs e)
        {
            int id1 = (lsvBill.Tag as Table).ID;
            int id2 = (cbTable.SelectedItem as Table).ID;

            if (MessageBox.Show(string.Format("Bạn muốn chuyển bàn {0} qua bàn {1}", (lsvBill.Tag as Table).Name, (cbTable.SelectedItem as Table).Name), "Thong bao", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes )
            {
                TableDAO.Instance.SwitchTable(id1, id2);
                LoadTable();
            }

        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(LoginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }

        void f_UpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.Displayname + ")";
        }
    }
}
