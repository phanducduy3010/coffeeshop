using QuanLyQuanCaPhe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCaPhe.DAO
{
    public class CategoryDAO
    {
        public static CategoryDAO instance;
        public static CategoryDAO Instance
        {
            get { if (instance == null) instance = new CategoryDAO(); return CategoryDAO.instance; }
            private set { CategoryDAO.instance = value; }
        }

        private CategoryDAO() { }

        public List<Category> GetListCategory()
        {
            List<Category> list = new List<Category>();
            string query = "select * from FoodCategory";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach(DataRow item in data.Rows)
            {
                Category category = new Category(item);
                list.Add(category);
            }
            return list;
        }

        public Category GetCategoryById (int id)
        {
            Category c = null;
            string query = "select * from FoodCategory where id = " +id;
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                c = new Category(item);
                return c;
            }

            return c;
        }

        public DataTable GetListCategory1()
        {
            return DataProvider.Instance.ExecuteQuery("Select id, name from FoodCategory");
        }

        public bool InsertCategory(string name)
        {
            string query = string.Format("insert FoodCategory (name) values (N'{0}')", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { name });
            return result > 0;
        }

        public bool UpdateCategory(string name, int id)
        {
            string query = string.Format("update FoodCategory set name = N'{0}' where id = {1}", name, id);
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { name, id });
            return result > 0;
        }

        public bool DeleteCategory(int id)
        { 
            FoodDAO.Instance.DeleteFoodByFoodCategory(id);
            string query = string.Format("delete FoodCategory where id = {0}", id);
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { id });
            return result > 0;
        }
    }
}
