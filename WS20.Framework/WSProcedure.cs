using System;
using System.Data;
using Devart.Data.Oracle;

namespace WS20.Framework
{
    public class WSProcedure
    {
        OracleConnection mConnection;
        OracleCommand mCommand;
        bool firstParam = true;

        /// <summary>
        /// Create a <c>WSProcedure</c> object
        /// </summary>
        /// <param name="theConnection">Pass in a <c>WSDBConnection</c> to the datasource containing the function or procedure to be called</param>
        /// <param name="theProcName">The name of the PL/SQL procedure or function</param>
        public WSProcedure(OracleConnection theConnection, string theProcName)
        {
            mConnection = theConnection;
            mCommand = new OracleCommand();
            mCommand.Connection = mConnection;
            mCommand.CommandText = theProcName;
            mCommand.CommandType = CommandType.StoredProcedure;
        }

        /// <summary>
        /// Adds a parameter to a procedure or function
        /// </summary>
        /// <remarks>
        /// The ReturnValue, if applicable, must be the first parameter set for the function.
        /// </remarks>
        /// <param name="theParamName">The parameter name</param>
        /// <param name="theParamType">The <c>OracleDbType</c> parameter type</param>
        /// <param name="theParamDir">The <c>ParameterDirection</c> parameter direction</param>
        /// <param name="theValue">Pass the value to an <c>Input</c> or <c>InputOutput</c> parameter, or pass <c>null</c> if it is an <c>Output</c> or <c>ReturnValue</c> parameter</param>
        public void AddParam(string theParamName, OracleDbType theParamType, ParameterDirection theParamDir, Object theValue)
        {
            // no parameter size in here
            OracleParameter aParam = new OracleParameter(theParamName, theParamType, theParamDir);
            if (theValue != null)
                aParam.Value = theValue;

            AddParam(aParam);
        }

        /// <summary>
        /// Adds a parameter to a procedure or function, including the parameter size, for use with data structures such as <c>varchar</c>
        /// </summary>
        /// <remarks>
        /// The ReturnValue, if applicable, must be the first parameter set for the function.
        /// </remarks>
        /// <param name="theParamName">The parameter name</param>
        /// <param name="theParamType">The <c>OracleDbType</c> parameter type</param>
        /// <param name="theParamDir">The <c>ParameterDirection</c> parameter direction</param>
        /// <param name="theSize">The parameter size</param>
        /// <param name="theValue">Pass the value to an <c>Input</c> or <c>InputOutput</c> parameter, or pass <c>null</c> if it is an <c>Output</c> or <c>ReturnValue</c> parameter</param>
        public void AddParam(string theParamName, OracleDbType theParamType, ParameterDirection theParamDir, int theSize, Object theValue)
        {
            OracleParameter aParam = new OracleParameter(theParamName, theParamType, theParamDir);
            if (theValue != null)
                aParam.Value = theValue;
            aParam.Size = theSize;

            AddParam(aParam);
        }

        private void AddParam(OracleParameter theParam)
        {
            if (!firstParam && theParam.Direction == ParameterDirection.ReturnValue)
                throw new Exception("ReturnValue must be the first parameter added.");
            else
            {
                mCommand.Parameters.Add(theParam);
                firstParam = false;
            }
        }

        /// <summary>
        /// Sets the <c>ArrayBindCount</c>, allowing array values to be passed in so that the function or procedure is called multiple times in one round trip
        /// </summary>
        /// <remarks>
        /// If <c>SetArrayBindCount</c> is not called before the execution of the function or procedure, the <c>ArrayBindCount</c> will default to 1
        /// </remarks>
        /// <param name="theCount">The number of elements in the value array(s) being passed to the function or procedure</param>
        
        //JLawrence - 9/23/2013 - comment this out - it is not needed and does not work with devart's dll
        //public void SetArrayBindCount(int theCount)
        //{
        //    mCommand.ArrayBindCount = theCount;
        //}

        /// <summary>
        /// Performs the function or procedure call after all of the parameters have been set
        /// </summary>
        /// <returns>Returns the number of results</returns>
        public int ExecuteNonQuery()
        {
            return mCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets the value returned by an <c>InputOutput</c>, <c>Output</c>, or <c>ReturnValue</c> parameter, or the array of values if <c>ArrayBindCount</c> is greater than 1
        /// </summary>
        /// <param name="theIndex">The array index (starting with 0) of the parameter, as determined by the order that the parameters were added with <c>AddParam</c></param>
        /// <returns>Returns the value or array of values returned through the given parameter</returns>
        public Object GetValue(int theIndex)
        {
            return mCommand.Parameters[theIndex].Value;
        }

        /// <summary>
        /// Gets the value returned by an <c>InputOutput</c>, <c>Output</c>, or <c>ReturnValue</c> parameter, or the array of values if <c>ArrayBindCount</c> is greater than 1
        /// </summary>
        /// <param name="theIndex">The string name of the parameter, as determined by <c>theParamName</c> set in <c>AddParam</c></param>
        /// <returns>Returns the value or array of values returned through the given parameter</returns>
        public Object GetValue(string theIndex)
        {
            return mCommand.Parameters[theIndex].Value;
        }
    }
}
