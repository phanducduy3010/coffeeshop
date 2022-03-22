using QuanLyQuanCaPhe.DAO;
using QuanLyQuanCaPhe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCaPhe
{
    public partial class fAdmin : Form
    {
        BindingSource foodList = new BindingSource();
        BindingSource accountList = new BindingSource();
        BindingSource categoryList = new BindingSource();

        public Account loginAccount;
        public fAdmin()
        {
            
            InitializeComponent();
            LoadDateTimePickerBill();
            LoadListBillByDate(dtpFromDate.Value, dtpcToDay.Value);
            LoadListFood();
            LoadListAccount();
            LoadListCategory();
            AddFoodBinding();
            AddAccountBinding();
            AddCategoryBinding();
            LoadCategory(cbFoodCatergories);
        }


        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood  = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }
        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpcToDay.Value = dtpFromDate.Value.AddMonths(1).AddDays(-1);
        }
        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListDate(checkIn, checkOut);
        }

        void LoadListFood()
        {

            foodList.DataSource = FoodDAO.Instance.GetListFood();
            dgvFood.DataSource = foodList;
        }

        void LoadListAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
            dtgvAccount.DataSource = accountList;
        }
       
        void LoadListCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetListCategory1();
            dtgvCategory.DataSource = categoryList;
        }
    

        void AddFoodBinding()
        {
            tbFoodName.DataBindings.Add(new Binding("Text", dgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txtID.DataBindings.Add(new Binding("Text", dgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            nmPrice.DataBindings.Add(new Binding("Value", dgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));
        }

        void AddAccountBinding()
        {
            txbUsername.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never ));
            txbDisplayname.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            nmType.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));
        }

        void AddCategoryBinding()
        {
            txbCaetegoryID.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "id", true, DataSourceUpdateMode.Never));
            txbCatergoryName.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "name", true, DataSourceUpdateMode.Never));
        }
        void LoadCategory(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";       
        }

        void AddAccount(string userName, string displayname, int type)
        {
            if(AccountDAO.Instance.InsertAccount(userName, displayname, type))
            {
                MessageBox.Show("Thêm tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Thêm thài khoản thất bại");
            }
            LoadListAccount();
        }

        void EditAccount(string userName, string displayname, int type)
        {
            if (AccountDAO.Instance.UpdateAccount1(userName, displayname, type))
            {
                MessageBox.Show("Cập nhật tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại");
            }
            LoadListAccount();
        }

        void DeleteAccount(string userName)
        {
            if(loginAccount.Username.Equals(userName))
            {
                MessageBox.Show("Bạn không thể xóa chính bạn!");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Xóa tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Xóa thài khoản thất bại");
            }
            LoadListAccount();
        }
        private void btn_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpFromDate.Value, dtpcToDay.Value);
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }
        private void txtID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dgvFood.SelectedCells[0].OwningRow.Cells["IDCategory"].Value;

                    Category c = CategoryDAO.Instance.GetCategoryById(id);
                    cbFoodCatergories.SelectedItem = c;
                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbFoodCatergories.Items)
                    {
                        if (item.ID == c.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }
                    cbFoodCatergories.SelectedIndex = index;
                }
            }
            catch
            {

            }
            
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = tbFoodName.Text;
            int categoryID = (cbFoodCatergories.SelectedItem as Category).ID;

            float price = (float)nmPrice.Value;

            if (FoodDAO.Instance.InsertFood(name, categoryID, price))
            {
                MessageBox.Show("Thêm món thành công");
                LoadListFood();
                if (insertFood != null) insertFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Thêm món thất bại");
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            string name = tbFoodName.Text;
            int categoryID = (cbFoodCatergories.SelectedItem as Category).ID;

            float price = (float)nmPrice.Value;
            int id = Convert.ToInt32(txtID.Text);

            if (FoodDAO.Instance.UpdateFood(id, name, categoryID, price))
            {
                MessageBox.Show("Sửa món thành công");
                LoadListFood();
                if (updateFood != null) updateFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Sửa món thất bại");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtID.Text);

            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Xóa món thành công");
                LoadListFood();
                if (deleteFood != null) deleteFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Xóa món thất bại");
            }
        }
        
        void ResetPassword(string userName)
        {
            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show("Đặt lại mật khẩu thành công");
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu thất bại");
            }
            LoadListAccount();
        }



        private void btnSearchFood_Click(object sender, EventArgs e)
        {
             foodList.DataSource = SearchFoodByName(txbSeacrhFood.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;
            string displayName = txbDisplayname.Text;
            int type = (int)nmType.Value;
            AddAccount(userName, displayName, type);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;
            DeleteAccount(userName);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;
            string displayName = txbDisplayname.Text;
            int type = (int)nmType.Value;
            EditAccount(userName, displayName, type);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;
            ResetPassword(userName);
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            txbNum.Text = "1";
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            int sumRecord = BillDAO.Instance.GetBillNumDate(dtpFromDate.Value, dtpcToDay.Value);
            int lastPage = sumRecord / 10;
            if (sumRecord % 10 != 0) lastPage++;
            txbNum.Text = lastPage.ToString();
        }

        private void txbNum_TextChanged(object sender, EventArgs e)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDateAndPage(dtpFromDate.Value, dtpcToDay.Value, Convert.ToInt32(txbNum.Text));

        }

        private void btnPre_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbNum.Text);
            if(page > 1)
                page--;
   
            txbNum.Text = page.ToString();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbNum.Text);
            int sumRecord = BillDAO.Instance.GetBillNumDate(dtpFromDate.Value, dtpcToDay.Value);
            if(page < sumRecord)          
                page++;
         
            txbNum.Text = page.ToString();
        }

        
        private void addCategory_Click(object sender, EventArgs e)
        {
            string name = txbCatergoryName.Text;

            if(CategoryDAO.Instance.InsertCategory(name))
            {
                MessageBox.Show("Thêm thành công");
                LoadListCategory();
                LoadListFood();
            } else
            {
                MessageBox.Show("Thêm thất bại");
            }
        }

        private void btnUpdateFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbCaetegoryID.Text);
            string name = txbCatergoryName.Text;

            if (CategoryDAO.Instance.UpdateCategory(name, id))
            {
                MessageBox.Show("Sửa thành công");
                LoadListCategory();
                LoadListFood();
            }
            else
            {
                MessageBox.Show("Sửa thất bại");
            }
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbCaetegoryID.Text);
            

            if (CategoryDAO.Instance.DeleteCategory(id))
            {
                MessageBox.Show("Xóa thành công");
                LoadListCategory();
                LoadListFood();
            }
            else
            {
                MessageBox.Show("Xóa thất bại");
            }
        }
    }
}
