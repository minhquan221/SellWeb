using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using Oracle.DataAccess;
using Oracle.DataAccess.Client;

namespace Lib.Data.DataConnect
{
    internal class OracleConnect : Data, IData
    {
        // Fields
        private OracleConnection objConnection;
        private OracleCommand objCommand;
        private OracleTransaction objTransaction;

        // Methods
        public OracleConnect()
        {
            this.objConnection = null;
            this.objCommand = null;
            this.objTransaction = null;
        }

        public OracleConnect(string strConnect)
        {
            this.objConnection = null;
            this.objCommand = null;
            this.objTransaction = null;
            base.strConnect = strConnect;
        }

        public void AddParameter(params object[] objArrParam)
        {
            for (int i = 0; i < (objArrParam.Length / 2); i++)
            {
                this.AddParameter(objArrParam[i * 2].ToString().Trim(), objArrParam[(i * 2) + 1]);
            }
        }

        public void AddParameter(Hashtable hstParameter)
        {
            IDictionaryEnumerator enumerator = hstParameter.GetEnumerator();
            while (enumerator.MoveNext())
            {
                this.AddParameter(enumerator.Key.ToString(), enumerator.Value);
            }
        }

        public void AddParameter(string strParameterName, object objValue)
        {
            if ((objValue != null) && objValue.ToString().Equals("True", StringComparison.OrdinalIgnoreCase))
            {
                objValue = 1;
            }
            else if ((objValue != null) && objValue.ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
            {
                objValue = 0;
            }
            this.objCommand.Parameters.Add(strParameterName.Replace("@", "v_"), objValue);
        }

        public void AddParameter(string strParameterName, object objValue, Globals.DATATYPE enDataType)
        {
            OracleParameter parameter = new OracleParameter(strParameterName.Replace("@", "v_"), this.GetOracleDataType(enDataType))
            {
                Value = objValue
            };
            this.objCommand.Parameters.Add(parameter);
        }

        public void BeginTransaction()
        {
            if (!this.IsConnected())
            {
                this.Connect();
            }
            this.objTransaction = this.objConnection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (this.objTransaction != null)
            {
                this.objTransaction.Commit();
            }
        }

        public bool Connect()
        {
            if (!this.IsConnected())
            {
                if (this.objConnection == null)
                {
                    string str = Data.Decrypt(base.strConnect, ConfigurationManager.AppSettings["DataEncryptKey"]).Replace(";Unicode=True", string.Empty);
                    this.objConnection = new OracleConnection(str);
                }
                this.objConnection.Open();
            }
            return true;
        }

        public void CreateNewSqlText(string strSQL)
        {
            this.objCommand = this.SetCommand(strSQL);
            this.objCommand.CommandType = CommandType.Text;
        }

        public void CreateNewStoredProcedure(string strStoreProName)
        {
            strStoreProName = this.RightSchema(strStoreProName);
            this.objCommand = this.SetCommand(strStoreProName);
            this.objCommand.CommandType = CommandType.StoredProcedure;
        }

        public void CreateNewStoredProcedure(string strStoreProName, int intTimeOut)
        {
            strStoreProName = this.RightSchema(strStoreProName);
            this.objCommand = this.SetCommand(strStoreProName);
            this.objCommand.CommandTimeout = intTimeOut;
            this.objCommand.CommandType = CommandType.StoredProcedure;
        }

        public bool Disconnect()
        {
            try
            {
                if (this.objCommand != null)
                {
                    this.objCommand.Dispose();
                }
                this.objConnection.Close();
                OracleConnection.ClearPool(this.objConnection);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public int ExecNonQuery() =>
            this.objCommand.ExecuteNonQuery();

        public ArrayList ExecQueryToArrayList(string strSQL)
        {
            IDataReader drReader = this.ExecQueryToDataReader(strSQL);
            ArrayList list = Globals.ConvertArrayList(drReader);
            drReader.Close();
            return list;
        }

        public byte[] ExecQueryToBinary(string strSQL) =>
            ((byte[])this.SetCommand(strSQL).ExecuteScalar());

        public IDataAdapter ExecQueryToDataAdapter(string strSQL) =>
            this.SetDataAdapter(strSQL);

        public IDataReader ExecQueryToDataReader(string strSQL) =>
            this.SetCommand(strSQL).ExecuteReader();

        public DataSet ExecQueryToDataSet(string strSQL)
        {
            DataSet dataSet = new DataSet();
            this.SetDataAdapter(strSQL).Fill(dataSet);
            return dataSet;
        }

        public DataTable ExecQueryToDataTable(string strSQL)
        {
            DataSet dataSet = new DataSet();
            this.SetDataAdapter(strSQL).Fill(dataSet);
            return dataSet.Tables[0];
        }

        public Hashtable ExecQueryToHashtable(string strSQL)
        {
            IDataReader drReader = this.ExecQueryToDataReader(strSQL);
            Hashtable hashtable = Globals.ConvertHashTable(drReader);
            drReader.Close();
            return hashtable;
        }

        public string ExecQueryToString(string strSQL)
        {
            object obj2 = this.SetCommand(strSQL).ExecuteScalar();
            return obj2?.ToString().Trim();
        }

        public ArrayList ExecStoreToArrayList() =>
            this.ExecStoreToArrayList(string.Empty);

        public ArrayList ExecStoreToArrayList(string strOutParameter)
        {
            if (strOutParameter.Trim().Length == 0)
            {
                strOutParameter = "v_Out";
            }
            this.objCommand.Parameters.Add(strOutParameter, 0x79).Direction = ParameterDirection.Output;
            return Globals.ConvertArrayList(this.objCommand.ExecuteReader());
        }

        public byte[] ExecStoreToBinary() =>
            this.ExecStoreToBinary(string.Empty);

        public byte[] ExecStoreToBinary(string strOutParameter)
        {
            if (strOutParameter.Length == 0)
            {
                strOutParameter = "v_Out";
            }
            this.objCommand.Parameters.Add(strOutParameter, (OracleDbType)0x84, 0x7d0).Direction = ParameterDirection.Output;
            return (byte[])this.objCommand.ExecuteScalar();
        }

        public IDataAdapter ExecStoreToDataAdapter() =>
            this.ExecStoreToDataAdapter(string.Empty);

        public IDataAdapter ExecStoreToDataAdapter(string strOutParameter)
        {
            if (strOutParameter.Trim().Length == 0)
            {
                strOutParameter = "v_Out";
            }
            this.objCommand.Parameters.Add(strOutParameter, 0x79).Direction = ParameterDirection.Output;
            return this.SetDataAdapter(this.objCommand);
        }

        public IDataReader ExecStoreToDataReader() =>
            this.ExecStoreToDataReader(string.Empty);

        public IDataReader ExecStoreToDataReader(string strOutParameter)
        {
            if (strOutParameter.Trim().Length == 0)
            {
                strOutParameter = "v_Out";
            }
            this.objCommand.Parameters.Add(strOutParameter, 0x79).Direction = ParameterDirection.Output;
            return this.objCommand.ExecuteReader();
        }

        public DataSet ExecStoreToDataSet() =>
            this.ExecStoreToDataSet(new string[] { string.Empty });

        public DataSet ExecStoreToDataSet(params string[] strOutParameter)
        {
            int num;
            for (num = 0; num < strOutParameter.Length; num++)
            {
                if (strOutParameter[num].Trim().Length == 0)
                {
                    strOutParameter[num] = "v_Out";
                }
            }
            for (num = 0; num < strOutParameter.Length; num++)
            {
                this.objCommand.Parameters.Add(strOutParameter[num], 0x79).Direction = ParameterDirection.Output;
            }
            DataSet dataSet = new DataSet();
            this.SetDataAdapter(this.objCommand).Fill(dataSet);
            return dataSet;
        }

        public DataTable ExecStoreToDataTable() =>
            this.ExecStoreToDataTable(string.Empty);

        public DataTable ExecStoreToDataTable(string strOutParameter)
        {
            if (strOutParameter.Trim().Length == 0)
            {
                strOutParameter = "v_Out";
            }
            this.objCommand.Parameters.Add(strOutParameter, 0x79).Direction = ParameterDirection.Output;
            DataSet dataSet = new DataSet();
            this.SetDataAdapter(this.objCommand).Fill(dataSet);
            return dataSet.Tables[0];
        }

        public Hashtable ExecStoreToHashtable() =>
            this.ExecStoreToHashtable(string.Empty);

        public Hashtable ExecStoreToHashtable(string strOutParameter)
        {
            if (strOutParameter.Trim().Length == 0)
            {
                strOutParameter = "v_Out";
            }
            this.objCommand.Parameters.Add(strOutParameter, 0x79).Direction = ParameterDirection.Output;
            return Globals.ConvertHashTable(this.objCommand.ExecuteReader());
        }

        public string ExecStoreToString() =>
            this.ExecStoreToString(string.Empty);

        public string ExecStoreToString(string strOutParameter)
        {
            if (strOutParameter.Length == 0)
            {
                strOutParameter = "v_Out";
            }
            this.objCommand.Parameters.Add(strOutParameter, (OracleDbType)0x77, 0xfa0).Direction = ParameterDirection.Output;
            this.objCommand.ExecuteScalar();
            object obj2 = this.objCommand.Parameters[strOutParameter].Value;
            if (Convert.IsDBNull(obj2) || obj2.ToString().Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }
            return obj2.ToString().Trim();
        }

        public void ExecUpdate(string strSQL)
        {
            this.SetCommand(strSQL).ExecuteNonQuery();
        }

        public void ExecUpdate(string strSQL, params IDataParameter[] objParameters)
        {
            this.SetCommand(strSQL);
            foreach (IDataParameter parameter in objParameters)
            {
                this.objCommand.Parameters.Add(parameter);
            }
            this.objCommand.ExecuteNonQuery();
        }

        public void ExecUpdate(string strSQL, ArrayList arrParameters)
        {
            this.SetCommand(strSQL);
            foreach (IDataParameter parameter in arrParameters)
            {
                this.objCommand.Parameters.Add(parameter);
            }
            this.objCommand.ExecuteNonQuery();
        }

        ~OracleConnect()
        {
            if (this.IsConnected())
            {
                this.Disconnect();
            }
        }

        private OracleDbType GetOracleDataType(Globals.DATATYPE enDataType)
        {
            switch (enDataType)
            {
                case Globals.DATATYPE.NUMBER:
                    return (OracleDbType)0x70;

                case Globals.DATATYPE.CHAR:
                    return (OracleDbType)0x68;

                case Globals.DATATYPE.VARCHAR:
                    return (OracleDbType)0x7e;

                case Globals.DATATYPE.NVARCHAR:
                    return (OracleDbType)0x77;

                case Globals.DATATYPE.NTEXT:
                    return (OracleDbType)0x74;

                case Globals.DATATYPE.BINARY:
                    return (OracleDbType)0x84;

                case Globals.DATATYPE.BLOB:
                    return (OracleDbType)0x66;

                case Globals.DATATYPE.CLOB:
                    return (OracleDbType)0x69;

                case Globals.DATATYPE.NCLOB:
                    return (OracleDbType)0x74;
            }
            return (OracleDbType)0x70;
        }

        public bool IsConnected()
        {
            if ((this.objConnection == null) || (this.objConnection.State != ConnectionState.Open))
            {
                return false;
            }
            return true;
        }

        private string RightSchema(string strStoreProName)
        {
            if ((strStoreProName.Split(new char[] { '.' }).Length <= 1) && (ConfigurationManager.AppSettings["DBSchema"] != null))
            {
                return (ConfigurationManager.AppSettings["DBSchema"].ToString().Trim() + "." + strStoreProName);
            }
            return strStoreProName;
        }

        public void RollBackTransaction()
        {
            if (this.objTransaction != null)
            {
                this.objTransaction.Rollback();
                this.objTransaction = null;
            }
        }

        private OracleCommand SetCommand(string strSQL)
        {
            this.objCommand = new OracleCommand(strSQL, this.objConnection);
            this.objCommand.BindByName = true;
            if (this.objTransaction != null)
            {
                this.objCommand.Transaction = this.objTransaction;
            }
            return this.objCommand;
        }

        private OracleDataAdapter SetDataAdapter(OracleCommand objCommand) =>
            new OracleDataAdapter(objCommand);

        private OracleDataAdapter SetDataAdapter(string strSQL) =>
            new OracleDataAdapter(strSQL, this.objConnection);

        // Properties
        IDbConnection IData.IConnection
        {
            get
            {
                return this.objConnection;
            }
            set
            {
                this.objConnection = (OracleConnection)value;
            }
        }

        IDbTransaction IData.ITransaction
        {
            get
            {
                return this.objTransaction;
            }
            set
            {
                this.objTransaction = (OracleTransaction)value;
            }
        }

        IDbCommand IData.ICommand
        {
            get
            {
                return this.objCommand;
            }
            set
            {
                this.objCommand = (OracleCommand)value;
            }
        }
    }
}