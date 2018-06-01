using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Data.OleDb;

namespace Lib.Data.DataConnect
{
    internal class MsAccessConnect : Data, IData
    {
        // Fields
        private OleDbConnection objConnection;
        private OleDbCommand objCommand;
        private OleDbTransaction objTransaction;

        // Methods
        public MsAccessConnect()
        {
            this.objConnection = null;
            this.objCommand = null;
            this.objTransaction = null;
        }

        public MsAccessConnect(string strConnect)
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
        }

        public void AddParameter(string strParameterName, object objValue, Globals.DATATYPE enDataType)
        {
            OleDbParameter parameter = new OleDbParameter(strParameterName.Replace("@", "v_"), this.GetOleDBDataType(enDataType))
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
                    this.objConnection = new OleDbConnection(Data.Decrypt(base.strConnect, "25251325@gmail.com"));
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
            this.objCommand = this.SetCommand(strStoreProName);
            this.objCommand.CommandType = CommandType.StoredProcedure;
        }

        public void CreateNewStoredProcedure(string strStoreProName, int intTimeOut)
        {
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

        public ArrayList ExecStoreToArrayList(string strOutParameter) =>
            Globals.ConvertArrayList(this.objCommand.ExecuteReader());

        public byte[] ExecStoreToBinary() =>
            this.ExecStoreToBinary(string.Empty);

        public byte[] ExecStoreToBinary(string strOutParameter) =>
            ((byte[])this.objCommand.ExecuteScalar());

        public IDataAdapter ExecStoreToDataAdapter() =>
            this.ExecStoreToDataAdapter(string.Empty);

        public IDataAdapter ExecStoreToDataAdapter(string strOutParameter) =>
            this.SetDataAdapter(this.objCommand);

        public IDataReader ExecStoreToDataReader() =>
            this.ExecStoreToDataReader(string.Empty);

        public IDataReader ExecStoreToDataReader(string strOutParameter) =>
            this.objCommand.ExecuteReader();

        public DataSet ExecStoreToDataSet() =>
            this.ExecStoreToDataSet(new string[] { string.Empty });

        public DataSet ExecStoreToDataSet(params string[] strOutParameter)
        {
            DataSet dataSet = new DataSet();
            this.SetDataAdapter(this.objCommand).Fill(dataSet);
            return dataSet;
        }

        public DataTable ExecStoreToDataTable() =>
            this.ExecStoreToDataTable(string.Empty);

        public DataTable ExecStoreToDataTable(string strOutParameter)
        {
            DataSet dataSet = new DataSet();
            this.SetDataAdapter(this.objCommand).Fill(dataSet);
            return dataSet.Tables[0];
        }

        public Hashtable ExecStoreToHashtable() =>
            this.ExecStoreToHashtable(string.Empty);

        public Hashtable ExecStoreToHashtable(string strOutParameter) =>
            Globals.ConvertHashTable(this.objCommand.ExecuteReader());

        public string ExecStoreToString() =>
            this.ExecStoreToString(string.Empty);

        public string ExecStoreToString(string strOutParameter)
        {
            object obj2 = this.objCommand.ExecuteScalar();
            return obj2?.ToString().Trim();
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

        ~MsAccessConnect()
        {
            if (this.IsConnected())
            {
                this.Disconnect();
            }
        }

        private OleDbType GetOleDBDataType(Globals.DATATYPE enDataType)
        {
            OleDbType integer = OleDbType.Integer;
            switch (enDataType)
            {
                case Globals.DATATYPE.NUMBER:
                    return OleDbType.Integer;

                case Globals.DATATYPE.CHAR:
                    return OleDbType.Char;

                case Globals.DATATYPE.VARCHAR:
                    return OleDbType.VarChar;

                case Globals.DATATYPE.NVARCHAR:
                    return OleDbType.VarWChar;

                case Globals.DATATYPE.NTEXT:
                    return OleDbType.LongVarWChar;

                case Globals.DATATYPE.BINARY:
                    return OleDbType.Binary;

                case Globals.DATATYPE.BLOB:
                    return OleDbType.LongVarBinary;

                case Globals.DATATYPE.CLOB:
                    return OleDbType.LongVarChar;

                case Globals.DATATYPE.NCLOB:
                    return OleDbType.LongVarWChar;
            }
            return integer;
        }

        public bool IsConnected()
        {
            if ((this.objConnection == null) || (this.objConnection.State != ConnectionState.Open))
            {
                return false;
            }
            return true;
        }

        public void RollBackTransaction()
        {
            if (this.objTransaction != null)
            {
                this.objTransaction.Rollback();
                this.objTransaction = null;
            }
        }

        private OleDbCommand SetCommand(string strSQL)
        {
            if (this.objTransaction == null)
            {
                this.objCommand = new OleDbCommand(strSQL, this.objConnection);
            }
            else
            {
                this.objCommand = new OleDbCommand(strSQL, this.objConnection, this.objTransaction);
            }
            return this.objCommand;
        }

        private OleDbDataAdapter SetDataAdapter(OleDbCommand objCommand) =>
            new OleDbDataAdapter(objCommand);

        private OleDbDataAdapter SetDataAdapter(string strSQL) =>
            new OleDbDataAdapter(strSQL, this.objConnection);

        // Properties
        IDbConnection IData.IConnection
        {
            get
            {
                return this.objConnection;
            }
            set
            {
                this.objConnection = (OleDbConnection)value;
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
                this.objTransaction = (OleDbTransaction)value;
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
                this.objCommand = (OleDbCommand)value;
            }
        }
    }
}