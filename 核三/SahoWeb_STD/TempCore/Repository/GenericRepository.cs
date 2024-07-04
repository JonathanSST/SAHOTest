using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MyDataObjectLib;
using DataAnnotationMapper;
using System.Reflection;

namespace TempCore.Repository
{
    public class GenericRepository<TEntity>  where TEntity : class
    {
        public bool isSuccess = true;
        public string Excetion = string.Empty;   //例外資訊                
        public SimpleDataObject _context
        {
            get;
            set;
        }
        public GenericRepository(string kind,string connection_string)
            : this(new SimpleDataObject(kind,connection_string))
        {
        }

        public GenericRepository(SimpleDataObject context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this._context = context;
        }

        /// <summary>將查詢用映射方式轉為物件</summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<TEntity> GetList<TEntity>(string cmd,Dictionary<string,object> parameters=null) where TEntity: new()
        {
            if (parameters != null)
            {
                this._context.SetParameter(parameters);
            }                
            DataTable dt = this._context.GetDataTableBySql(cmd);
            List<TEntity> result = ColumnInfo.GetEntities<TEntity>(dt);
            return result;
        }        


        public void Create(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {                
                StringBuilder builders = new StringBuilder();
                var pd = TableInfo.FromPoco(instance.GetType());
                var infos = ColumnInfo.GetColumnInfos<TEntity>(instance);
                Dictionary<string, object> dic_data = new Dictionary<string, object>();
                string insert_field = "";
                string insert_data = "";
                foreach (var ci in infos.Where(i=>i.IsDataColumn))
                {
                    if (!pd.PrimaryKey.Contains(ci.ColumnName)&&!ci.DataKey&&!ci.ResultColumn)
                    {
                        insert_field += "," + ci.ColumnName;
                        insert_data += ",@" + ci.ColumnName;
                        dic_data.Add("@" + ci.ColumnName, ci.DataValue);
                    }
                }
                builders.AppendLine("INSERT INTO " + pd.TableName);
                builders.Append("(");
                builders.Append(insert_field.TrimStart(','));
                builders.Append(")");
                builders.AppendLine(" VALUES ");
                builders.Append("(");
                builders.Append(insert_data.TrimStart(','));
                builders.Append(")");
                try
                {
                    this._context.BeginTransaction();
                    this._context.SetParameter(dic_data);                    
                    this._context.ExecuteNonQuery(builders.ToString());
                    this.isSuccess = true;
                    this._context.Commit();
                }
                catch (Exception ex)
                {
                    this.isSuccess = false;
                    this.Excetion = ex.Message;
                }                
            }
        }


        public void DoTransaction(string cmd_str, TEntity instance)
        {
            var infos = ColumnInfo.GetColumnInfos<TEntity>(instance);
            Dictionary<string, object> dic_data = new Dictionary<string, object>();
            string update_field = "";
            string update_data = "";
            foreach (var ci in infos.Where(i => i.IsDataColumn))
            {
                dic_data.Add("@" + ci.ColumnName, ci.DataValue);
            }
            try
            {
                this._context.BeginTransaction();
                this._context.SetParameter(dic_data);
                this._context.ExecuteNonQuery(cmd_str);
                this._context.Commit();
                this.isSuccess = true;
            }
            catch (Exception ex)
            {
                this.Excetion = ex.Message;
                this.isSuccess = false;
            }
        }

        public void Update(TEntity instance)
        {
            var pd = TableInfo.FromPoco(instance.GetType());
            var infos = ColumnInfo.GetColumnInfos<TEntity>(instance);
            Dictionary<string, object> dic_data = new Dictionary<string, object>();
            string update_field = "";
            string update_data = "";
            foreach (var ci in infos.Where(i=>i.IsDataColumn))
            {
                if (!pd.PrimaryKey.Contains(ci.ColumnName) && !ci.DataKey&&!ci.ResultColumn)
                {
                    update_field += "," + ci.ColumnName + "=@" + ci.ColumnName;
                    dic_data.Add("@" + ci.ColumnName, ci.DataValue);
                }
                if (pd.PrimaryKey.Contains(ci.ColumnName))
                {
                    dic_data.Add("@" + ci.ColumnName, ci.DataValue);
                }
            }
            update_field = update_field.TrimStart(',');
            var cmd = " UPDATE " + pd.TableName + " SET " + update_field + " WHERE "+pd.PrimaryKey.FirstOrDefault()+"=@"+pd.PrimaryKey.FirstOrDefault();
            try
            {                
                this._context.BeginTransaction();
                this._context.SetParameter(dic_data);
                this._context.ExecuteNonQuery(cmd);
                this._context.Commit();
                this.isSuccess = true;
            }
            catch (Exception ex)
            {
                this.Excetion = ex.Message;
                this.isSuccess = false;
            }
        }

    }
}
