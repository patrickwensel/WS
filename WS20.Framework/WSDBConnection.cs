using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web.UI;
using Devart.Data.Oracle;


namespace WS20.Framework
{
    public class WSDBConnection
    {
        private OracleConnection mConnection;
        private ArrayList mReaderList;
        private OracleTransaction mTransaction;
        private Page mPage = null; // if the user passes a Page in, DB calls will be written to the Page.Trace

        const int DEFAULT_TIMEOUT = -9214;

        /// <summary>
        /// Create a <c>WSDBConnection</c> object
        /// </summary>
        /// <param name="thePage">Pass in a reference to the current page in order to record <c>Trace</c> information</param>
        public WSDBConnection(Page thePage)
        {
            mReaderList = new ArrayList();
            mPage = thePage;
        }

        /// <summary>
        /// Create a <c>WSDBConnection</c> object
        /// </summary>
        public WSDBConnection()
            : this(null)
        {
        }

        /// <summary>
        /// Destroys the <c>WSDBConnection</c> object; hopefully (but probably rarely) cleans up any open <c>OracleDataReader</c> objects
        /// </summary>
        ~WSDBConnection()
        {
            foreach (OracleDataReader aReader in mReaderList)
            {
                if (aReader != null)
                    aReader.Close();
            }
            if (mConnection != null)
                mConnection.Close();
        }

        /// <summary>
        /// Opens a connection to the specified database
        /// </summary>
        /// <param name="theSource">The source database, as listed in the datastore</param>
        /// <param name="theTimeOut">Enter a timeout value to override the default timeout</param>
        /// <returns>Returns true if database is successfuly opened</returns>
        public bool Open(string theSource, int theTimeOut)
        {
            switch (theSource)
            {
                case "WEB":
                    mConnection = new OracleConnection(ConfigurationManager.ConnectionStrings[theSource].ConnectionString);
                    mConnection.Open();
                    return true;
                case "JDE_WEB":
                    mConnection = new OracleConnection(ConfigurationManager.ConnectionStrings[theSource].ConnectionString);
                    mConnection.Open();
                    return true;
                case "OLI":
                    mConnection = new OracleConnection(ConfigurationManager.ConnectionStrings[theSource].ConnectionString);
                    mConnection.Open();
                    return true;
                case "SISR":
                    mConnection = new OracleConnection(ConfigurationManager.ConnectionStrings[theSource].ConnectionString);
                    mConnection.Open();
                    return true;
                case "CCKEY":
                    mConnection = new OracleConnection(ConfigurationManager.ConnectionStrings[theSource].ConnectionString);
                    mConnection.Open();
                    return true;

                default:


                    return false;
            }

            //JLawrence - Leave this as a reference to see how they were reading the connection info from DataStore.xml
            //DataSet aDS = new DataSet();
            //aDS.ReadXml("c:\\DataStore.xml");

            //int aNumSources = aDS.Tables["DataSource"].Rows.Count;
            //int aSourceNum = -1;
            //for (int i = 0; i < aNumSources; i++)
            //{
            //    if (aDS.Tables["DataSource"].Rows[i]["Name"].ToString().ToLower() == theSource.ToLower())
            //    {
            //        aSourceNum = i;
            //        break;
            //    }
            //}

            //if (aSourceNum < 0)
            //    return false;

            //DataRow aRow = aDS.Tables["DataSource"].Rows[aSourceNum];

            //string aLoginStr = String.Format("Data Source={0}; User Id={1}; Password={2}; Pooling={3}; Connect Timeout={4};",
            //    aRow["Source"].ToString(),
            //    aRow["Login"].ToString(),
            //    "web",
            //    aRow["Pooling"].ToString(),
            //    (theTimeOut == DEFAULT_TIMEOUT ? aRow["Timeout"].ToString() : theTimeOut.ToString()));

            //mConnection = new OracleConnection(aLoginStr);
            //mConnection.Open();



            //return true;
        }

        /// <summary>
        /// Opens a connection to the specified database
        /// </summary>
        /// <param name="theSource">The source database, as listed in the datastore</param>
        /// <returns>Returns true if database is successfuly opened</returns>
        public bool Open(string theSource)
        {
            Open(theSource, DEFAULT_TIMEOUT);

            return true;
        }

        /// <summary>
        /// Closes the database connection
        /// </summary>
        public void Close()
        {
            if (mConnection != null && mConnection.State != ConnectionState.Closed) { mConnection.Close(); }
        }

        /// <summary>
        /// Private method Returns the results of a database query in the form of a OracleDataAdapter
        /// </summary>
        /// <param name="theQuery"></param>
        /// <returns></returns>
        private OracleDataAdapter GetOracleDataAdapter(string theQuery)
        {
            OracleDataAdapter anAdapter = new OracleDataAdapter(theQuery, mConnection);

            return anAdapter;
        }

        /// <summary>
        /// Returns the results of a database query in the form of a DataTable
        /// </summary>
        /// <param name="theQuery">The query to be passed to the database</param>
        /// <returns>Returns the results of the query in a DataTable</returns>
        public DataTable GetDataTable(string theQuery)
        {
            if (mPage != null)
                mPage.Trace.Write("WSDBConnection", theQuery);

            OracleDataAdapter anAdapter = GetOracleDataAdapter(theQuery);
            DataTable aDT = new DataTable();
            anAdapter.Fill(aDT);

            if (mPage != null)
                mPage.Trace.Write("WSDBConnection", "GetDataTable Complete");

            return aDT;
        }

        /// <summary>
        /// Returns the results of a database query in the form of an OracleDataReader
        /// </summary>
        /// <param name="theQuery">The query to be passed to the database</param>
        /// <returns>Returns the OracleDataReader results of the query</returns>
        public OracleDataReader GetReader(string theQuery)
        {
            OracleDataReader aReader = null;

            if (mPage != null)
                mPage.Trace.Write("WSDBConnection", theQuery);

            try
            {
                OracleCommand anOC = new OracleCommand(theQuery, mConnection);
                anOC.CommandType = CommandType.Text;
                aReader = anOC.ExecuteReader();
                mReaderList.Add(aReader);
                anOC.Dispose();
            }
            catch (OracleException e)
            {
                string aMessage = e.Message + theQuery;
                throw new Exception(aMessage);
            }

            if (mPage != null)
                mPage.Trace.Write("WSDBConnection", "Query Complete");

            return aReader;
        }

        /// <summary>
        /// Executes a command in the database
        /// </summary>
        /// <param name="theCommand">The command to be executed in the database</param>
        /// <returns>Returns the number of rows affected by the command</returns>
        public int ExecuteCommand(string theCommand)
        {
            int aNumRows = 0;

            if (mPage != null)
                mPage.Trace.Write("WSDBConnection", theCommand);

            try
            {
                OracleCommand aCommand = new OracleCommand(theCommand, mConnection);
                aNumRows = aCommand.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                string aMessage = e.Message + theCommand;
                throw new Exception(aMessage);
            }

            if (mPage != null)
                mPage.Trace.Write("WSDBConnection", aNumRows + " Rows Updated");

            return aNumRows;
        }

        private OracleConnection GetConnection()
        {
            return mConnection;
        }

        /// <summary>
        /// Begins a transaction session
        /// </summary>
        /// <remarks>No queries or commands will be executed until CommitTransaction is called</remarks>
        public void BeginTransaction()
        {
            mTransaction = mConnection.BeginTransaction();
        }

        /// <summary>
        /// Commits a transaction session, or rolls back if one or more queries and commands fail
        /// </summary>
        /// <returns>Returns true if all queries and commands were sucessfully committed</returns>
        public bool CommitTransaction()
        {
            try
            {
                mTransaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Rolls back a transaction session
        /// </summary>
        public void RollbackTransaction()
        {
            mTransaction.Rollback();
        }

        /// <summary>
        /// Creates a WSProcedure object used to access a SQL procedure or function
        /// </summary>
        /// <param name="theProcName">The name of the SQL procedure or function</param>
        /// <returns>Returns the uninitialized WSProcedure object</returns>
        /// <remarks>After the object has been created, it must be initialized through WSProcedure functions before the SQL function or procedure is called.</remarks>
        public WSProcedure InitProcedure(string theProcName)
        {
            WSProcedure aProcedure = new WSProcedure(mConnection, theProcName);

            return aProcedure;
        }
    }
}
