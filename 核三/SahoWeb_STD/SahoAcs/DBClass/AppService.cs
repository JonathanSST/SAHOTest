using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SahoAcs.DBModel;

namespace SahoAcs.DBClass
{
    public static class AppServiceExtension
    {
        public static IQueryable<T> OrderByField<T>(this IEnumerable<T> q, string SortField, bool Ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, SortField);
            var exp = Expression.Lambda(prop, param);
            string method = Ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.AsQueryable<T>().ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.AsQueryable<T>().Expression, exp);
            return q.AsQueryable<T>().Provider.CreateQuery<T>(mce);
        }

        public static List<T> AddNewObj<T>(this List<T> para, T t)
        {
            para.Add(t);
            return para;
        }

        public static List<EquData> AddObj(this List<EquData> para, string name, string value)
        {
            para.Add(new EquData()
            {
                EquModel = name, EquName = value
            });
            return para;
        }

        public static List<string> GetMcrinfoCols(this DapperDataObjectLib.OrmDataObject odo)
        {
            System.Data.DataTable DataResult = odo.GetDataTableBySql("SELECT TOP 1 * FROM V_MCRInfo");
            //DataResult.Columns.
            List<string> cols = new List<string>();
            foreach (DataColumn dc in DataResult.Columns)
            {
                cols.Add(dc.ColumnName);
            }
            cols.Add("EquEName");
            cols.Add("EquNoList");
            return cols;
        }

        public static List<string> GetDataInfoList(this DapperDataObjectLib.OrmDataObject odo, string TableName)
        {
            System.Data.DataTable DataResult = odo.GetDataTableBySql(string.Format("SELECT TOP 1 * FROM {0} ", TableName));
            //DataResult.Columns.
            List<string> cols = new List<string>();
            foreach (DataColumn dc in DataResult.Columns)
            {
                cols.Add(dc.ColumnName);
            }
            //cols.Add("EquEName");
            return cols;
        }



        public static string GetCardNo(string CardNo)
        {
            Dictionary<string, string> area_code = new Dictionary<string, string>();
            area_code.Add("A", "10");
            area_code.Add("B", "11");
            area_code.Add("C", "12");
            area_code.Add("D", "13");
            area_code.Add("E", "14");
            area_code.Add("F", "15");
            area_code.Add("G", "16");
            area_code.Add("H", "17");
            area_code.Add("I", "34");
            area_code.Add("J", "18");
            area_code.Add("K", "19");

            area_code.Add("M", "21");
            area_code.Add("N", "22");
            area_code.Add("O", "35");
            area_code.Add("P", "23");
            area_code.Add("Q", "24");
            area_code.Add("T", "27");
            area_code.Add("U", "28");
            area_code.Add("V", "29");
            area_code.Add("W", "32");
            area_code.Add("X", "30");
            area_code.Add("Z", "33");

            area_code.Add("S", "26");
            area_code.Add("Y", "31");
            area_code.Add("L", "20");
            area_code.Add("R", "25");
            try
            {
                return area_code[CardNo.Substring(0, 1)] + CardNo.Substring(1, CardNo.Length - 1);
            }
            catch (Exception ex)
            {
                return "0000000000";
            }
        }


        public static string GetFormEndValue(this System.Web.UI.Page page, string KeyName)
        {            
            if (page.Request.Form.AllKeys.Where(i => i.EndsWith(KeyName)).Count() > 0)
            {
                KeyName = page.Request.Form.AllKeys.Where(i => i.EndsWith(KeyName)).First();
                return page.Request.Form[KeyName];
            }
            return "";
        }

        public static string GetFormEqlValue(this System.Web.UI.Page page, string KeyName)
        {            
            if (page.Request.Form.AllKeys.Where(i => i.Equals(KeyName)).Count() > 0)
            {
                KeyName = page.Request.Form.AllKeys.Where(i => i.EndsWith(KeyName)).First();
                return page.Request.Form[KeyName];
            }
            return "";
        }

        public static string GetCheckNumber(this System.Web.UI.Page page, string paramValue)
        {
            double checkValue = 0.0;
            double.TryParse(paramValue, out checkValue);
            return checkValue.ToString();
        }

    }//end class
}//end namespace
