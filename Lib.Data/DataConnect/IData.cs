using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Data.DataConnect
{
    public interface IData
    {
        // Methods
        void AddParameter(params object[] objArrParam);
        void AddParameter(Hashtable hstParameter);
        void AddParameter(string strParameterName, object objValue);
        void AddParameter(string strParameterName, object objValue, Globals.DATATYPE enDataType);
        void BeginTransaction();
        void CommitTransaction();
        bool Connect();
        void CreateNewSqlText(string strSQL);
        void CreateNewStoredProcedure(string strStoreProName);
        void CreateNewStoredProcedure(string strStoreProName, int intTimeOut);
        bool Disconnect();
        void Dispose();
        int ExecNonQuery();
        ArrayList ExecQueryToArrayList(string strSQL);
        byte[] ExecQueryToBinary(string strSQL);
        IDataAdapter ExecQueryToDataAdapter(string strSQL);
        IDataReader ExecQueryToDataReader(string strSQL);
        DataSet ExecQueryToDataSet(string strSQL);
        DataTable ExecQueryToDataTable(string strSQL);
        Hashtable ExecQueryToHashtable(string strSQL);
        string ExecQueryToString(string strSQL);
        ArrayList ExecStoreToArrayList();
        ArrayList ExecStoreToArrayList(string strOutParameter);
        byte[] ExecStoreToBinary();
        byte[] ExecStoreToBinary(string strOutParameter);
        IDataAdapter ExecStoreToDataAdapter();
        IDataAdapter ExecStoreToDataAdapter(string strOutParameter);
        IDataReader ExecStoreToDataReader();
        IDataReader ExecStoreToDataReader(string strOutParameter);
        DataSet ExecStoreToDataSet();
        DataSet ExecStoreToDataSet(params string[] strOutParameter);
        DataTable ExecStoreToDataTable();
        DataTable ExecStoreToDataTable(string strOutParameter);
        Hashtable ExecStoreToHashtable();
        Hashtable ExecStoreToHashtable(string strOutParameter);
        string ExecStoreToString();
        string ExecStoreToString(string strOutParameter);
        void ExecUpdate(string strSQL);
        void ExecUpdate(string strSQL, params IDataParameter[] objParameters);
        void ExecUpdate(string strSQL, ArrayList arrParameters);
        bool IsConnected();
        void RollBackTransaction();

        // Properties
        IDbConnection IConnection { get; set; }
        IDbCommand ICommand { get; set; }
        IDbTransaction ITransaction { get; set; }
    }





}
