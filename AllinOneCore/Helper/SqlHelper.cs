using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AllinOneCore.Helper
{
    public partial class ShopNailEntities
    {
        /// <summary>
        /// Thực thi store trả về List object khi select
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strProcName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<T> ExecuteStoreToList<T>(string strProcName, object[] param)
        {
            return this.Database.SqlQuery<T>("exec " + strProcName, param).ToList<T>();
        }

        public List<T> ExecuteStoreToListParam<T>(string sql, object[] param)
        {
            return this.Database.SqlQuery<T>(sql, param).ToList<T>();
        }

        /// <summary>
        /// Thực thi store trả về List object khi select
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<T> ExecuteStoreToList<T>(string sql, SqlParameter[] param)
        {
            if (param != null && param.Count() > 0)
                return this.Database.SqlQuery<T>(sql, param).ToList<T>();

            return this.Database.SqlQuery<T>(sql).ToList<T>();
        }

        /// <summary>
        /// Thực thi store trả về 1 đối tượng có thể dùng cho hàm Insert khi cần tham số trả về
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strProcName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T ExecuteStoreToObject<T>(string strProcName, params object[] param)
        {
            return this.Database.SqlQuery<T>("exec " + strProcName, param).FirstOrDefault();
        }

        /// <summary>
        /// Thực thi store trả về 1 đối tượng có thể dùng cho hàm Insert khi cần tham số trả về 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T ExecuteStoreToObject<T>(string sql, SqlParameter[] param = null)
        {
            if (param != null && param.Count() > 0)
                return this.Database.SqlQuery<T>(sql, param).FirstOrDefault();

            return this.Database.SqlQuery<T>(sql).FirstOrDefault();
        }

        public bool ExecuteQueryInsert<T>(T objDataObject, ref string ErrorMessage) where T : class
        {
            bool result = false;
            try
            {
                string sql = string.Empty;
                string sqlColumn = string.Empty;
                string sqlValue = string.Empty;
                PropertyInfo primarykey = GetPrimaryKeyInfo<T>();
                sql += "INSERT INTO " + typeof(T).Name + " ";
                PropertyInfo[] objProperties;
                objProperties = typeof(T).GetProperties();
                foreach (var item in objProperties)
                {
                    if (item.Name != primarykey.Name)
                    {
                        sqlColumn += item.Name + ",";
                        sqlValue += item.GetValue(objDataObject, null) == null ? "null," : "'" + item.GetValue(objDataObject, null) + "',";
                    }
                }
                sql += "(" + sqlColumn.Substring(0, sqlColumn.Length - 1) + ") VALUES (" + sqlValue.Substring(0, sqlValue.Length - 1) + ")";
                var rest = this.Database.ExecuteSqlCommand(sql);
                //RunQuery(sql);
                result = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
                throw;
            }
            return result;
        }

        public bool RunQuery(string strSql)
        {
            bool result = false;
            try
            {
                this.Database.SqlQuery<string>(strSql);
                result = true;
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        public PropertyInfo GetPrimaryKeyInfo<T>() where T : class
        {
            ObjectContext objectContext = ((IObjectContextAdapter)this).ObjectContext;
            ObjectSet<T> set = objectContext.CreateObjectSet<T>();
            IEnumerable<string> keyNames = set.EntitySet.ElementType.KeyMembers.Select(k => k.Name);
            PropertyInfo properties = typeof(T).GetProperty(keyNames.First());
            return properties;
        }

        public bool ExecuteQueryUpdate<T>(T objDataObject, ref string ErrorMessage) where T : class
        {
            bool result = false;
            try
            {
                string sql = string.Empty;
                string sqlColumn = string.Empty;
                string sqlValue = string.Empty;
                sql += "UDPDATE " + typeof(T).Name + " SET ";
                PropertyInfo primarykey = GetPrimaryKeyInfo<T>();
                PropertyInfo[] objProperties;
                objProperties = typeof(T).GetProperties();
                foreach (var item in objProperties)
                {
                    if (item.Name != primarykey.Name)
                    {
                        sql += item.Name + " = " + (item.GetValue(objDataObject, null) == null ? "null," : "'" + item.GetValue(objDataObject, null)) + "',";
                    }
                }
                sql = sql.Substring(0, sql.Length - 1);
                sql += " WHERE " + primarykey.Name + " = " + primarykey.GetValue(objDataObject, null);
                var rest = this.Database.ExecuteSqlCommand(sql);
                result = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
                throw;
            }
            return result;
        }

        public List<T> ExecuteQueryToListObject<T>(List<string[]> param = null, List<objJoiningTable> Join = null, string OrderBy = "")
        {
            string sql = string.Empty;
            sql += "SELECT ";
            PropertyInfo[] objProperties;
            objProperties = typeof(T).GetProperties();
            foreach (var item in objProperties)
            {
                sql += item.Name + ",";
            }
            sql = sql.Substring(0, sql.Length - 1);
            sql += " FROM " + typeof(T).Name;
            if (Join != null && Join.Count() > 0)
            {
                for (int i = 0; i < Join.Count; i++)
                {
                    objJoiningTable obj = Join[i];
                    if (i == 0)
                    {
                        sql += " " + obj.TypeJoin + " JOIN " + obj.ObjectData.Name + " = " + obj.ObjectCompare.Name;
                    }
                    else
                    {
                        sql += " " + " AND " + obj.TypeJoin + " JOIN " + obj.ObjectData.Name + " = " + obj.ObjectCompare.Name;
                    }
                }
            }

            if (param != null && param.Count() > 0)
            {
                sql += " WHERE ";
                for (int i = 0; i < param.Count; i++)
                {
                    string[] obj = param[i];
                    if (i == 0)
                    {
                        sql += obj[0] + " = " + obj[1];
                    }
                    else
                    {
                        sql += " AND " + obj[0] + " = " + obj[1];
                    }
                }
            }
            return this.Database.SqlQuery<T>(sql).ToList();
        }

        public T ExecuteQueryToObject<T>(List<string[]> param = null, List<objJoiningTable> Join = null, string OrderBy = "")
        {
            return ExecuteQueryToListObject<T>(param, Join, OrderBy).FirstOrDefault();
        }

        /// <summary>
        /// Thực thi store cho việc Insert, Update, Deleteds
        /// </summary>
        /// <param name="strProcName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool ExecuteStoreCRUD(string strProcName, object[] param)
        {
            int intResult = 0;
            intResult = this.Database.ExecuteSqlCommand(strProcName, param);
            if (intResult > 0) return true;
            return false;
        }

        //public List<T> ExecuteQuerySelectByIdToObject<T>(string strQueryString, string[] paramId, string strTableName)
        //{
        //    int intResult = 0;
        //    strQueryString = string.Empty;
        //    strQueryString += "Select * from " + strTableName + " where ";
        //    for(int i = 0; i < paramId.Length; i ++)
        //    {
        //        if(i == 0)
        //        {
        //            strQueryString += paramId[i][0] + " = " + paramId[i][1];
        //        }
        //        else
        //        {
        //            strQueryString += " AND " + paramId[i][0] + " = " + paramId[i][1];
        //        }
        //    }
        //    var results = this.Database.SqlQuery<T>(strQueryString).ToList();
        //    return results;
        //}

        /// <summary>
        /// Thực thi store cho việc Insert, Update, Deleteds
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public void ExecuteStoreCRUD(string sql, SqlParameter[] param = null)
        {
            //int intResult = 0;
            //intResult = this.Database.ExecuteSqlCommand(sql, param);
            //if (intResult > 0) return true;
            //return false;
            this.Database.ExecuteSqlCommand(sql, param);
        }
    }
}
