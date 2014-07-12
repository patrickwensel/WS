using System;
using System.Collections;
using System.DirectoryServices;
using System.Web.UI;
using Devart.Data.Oracle;

namespace WS20.Framework
{
    public class WSSecurityPackage
    {
        /// <summary>
        /// Provides methods used to identify web users
        /// </summary>
        public abstract class NTLookup
        {
            /// <summary>
            /// Returns the current user's NT name
            /// </summary>
            /// <param name="thePage">The page the user is viewing</param>
            /// <returns>The user's NT login name</returns>
            public static string GetNTUserName(Page thePage)
            {
                return thePage.Request.ServerVariables["LOGON_USER"].Replace("WS\\", "");
            }

            /// <summary>
            /// Returns the current user's email address
            /// <remarks>
            /// Currently, this function simply appends "@willscot.com" to the username.
            /// In the future, this function will lookup the user in Active Directory and return 
            /// the true email address.
            /// </remarks>
            /// </summary>
            /// <param name="thePage">The page the user is viewing</param>
            /// <returns>The user's NT email address</returns>
            public static string GetNTEmail(Page thePage)
            {
                return GetNTUserName(thePage) + "@willscot.com";
            }

            /// <summary>
            /// Returns the current user's employee key
            /// <remarks>
            /// This function currently queries OLI based on the NT username.
            /// In the future, employee key should be stored in Active Directory.
            /// </remarks>
            /// </summary>
            /// <param name="thePage">The page the user is viewing</param>
            /// <returns>The user's employee key</returns>
            public static string GetEmployeeKey(Page thePage)
            {
                WSDBConnection aConn = new WSDBConnection(thePage);
                aConn.Open("OLI");

                string aQuery = String.Format(@"
					SELECT		employee_key
					FROM		mv_ws_emps
					WHERE		nt_user_id = '{0}'
					AND			UPPER(status) = 'A'",
                                              GetNTUserName(thePage));

                OracleDataReader aReader = aConn.GetReader(aQuery);

                string anEmpKey = "";
                if (aReader.Read())
                    anEmpKey = aReader["employee_key"].ToString();

                aReader.Close();

                return anEmpKey;
            }

            private static ResultPropertyCollection GetProperties(string theNTUserName)
            {
                DirectoryEntry anEntry = new DirectoryEntry("LDAP://ws3nt");
                DirectorySearcher aSearcher = new DirectorySearcher(anEntry);
                aSearcher.Filter = "(anr=" + theNTUserName + ")";
                SearchResult aSearchResult = aSearcher.FindOne();
                anEntry.Close();
                return aSearchResult.Properties;
            }
        }

        /// <summary>
        /// Structure used to define entity-page-access groupings in the Content Management System
        /// </summary>
        public struct AccessLevel
        {
            /// <summary>
            /// The entity identifier; syntax based on the entity type (can be employee key, location key, etc)
            /// </summary>
            public string mEntity;

            /// <summary>
            /// The entity NT name, only used in the case of EntityType ENTITY_USER
            /// </summary>
            public string mEntityNTName;

            /// <summary>
            /// The expanded entity name, such as a full name or location name
            /// </summary>
            public string mEntityName;

            /// <summary>
            /// The entity type, as defined in WSLookupTable
            /// </summary>
            public WSLookupTable.EntityType mType;

            /// <summary>
            /// The list of permissions available to this entity
            /// <remarks>
            /// Each index of the array corresponds to an AccessType enumerated value.
            /// </remarks>
            /// </summary>
            public bool[] mAccess;

            /// <summary>
            /// The list of sequence levels corresponding to the access levels defined in mAccess[]
            /// <remarks>
            /// A sequence levels meaning will vary by access type.
            /// By default (and when unused) the sequence level is set to -1.
            /// </remarks>
            /// </summary>
            public int[] mSeqLevel;

            /// <summary>
            /// The Content Management System page ID to which this entity-access pairing belongs
            /// </summary>
            public int mPageID;

            /// <summary>
            /// Constructor creates new AccessLevel
            /// </summary>
            /// <param name="theEntity">The entity key to which to assign access</param>
            /// <param name="theEntityNTName">The entity NT name corresponding to the entity key</param>
            /// <param name="theEntityName">The expanded entity name corresponding to the entity key</param>
            /// <param name="theType">The entity type</param>
            /// <param name="theAccess">The access type (one access type must be originally assigned)</param>
            /// <param name="theSeqLevel">The sequence level corresponding to the access type</param>
            /// <param name="thePageID">The page ID to which access is to be assigned</param>
            public AccessLevel(string theEntity, string theEntityNTName, string theEntityName,
                               WSLookupTable.EntityType theType, WSLookupTable.AccessType theAccess, int theSeqLevel, int thePageID)
            {
                mEntity = theEntity;
                mEntityNTName = theEntityNTName;
                mEntityName = theEntityName;
                mType = theType;
                int aNumAccessTypes = Enum.GetNames(typeof (WSLookupTable.AccessType)).Length;
                mAccess = new bool[aNumAccessTypes];
                //			for(int i=0; i<aNumAccessTypes; i++)  C# defaults bools to false
                //				mAccess[i] = false;
                mAccess[(int) theAccess] = true;
                mSeqLevel = new int[aNumAccessTypes];
                for (int i = 0; i < aNumAccessTypes; i++)
                    mSeqLevel[i] = -1;
                mSeqLevel[(int) theAccess] = theSeqLevel;
                mPageID = thePageID;
            }

            /// <summary>
            /// Creates a copy of an entity-access pairing for a new page
            /// </summary>
            /// <param name="aNewPageID">The new page to which the entity-access pairing will be assigned</param>
            /// <returns>The new <c>AccessLevel</c></returns>
            public AccessLevel Clone(int aNewPageID)
            {
                AccessLevel aNewAccessLevel = new AccessLevel(mEntity, mEntityNTName, mEntityName, mType,
                                                              WSLookupTable.AccessType.ACCESS_READ, 0, aNewPageID);
                int aNumAccessTypes = Enum.GetNames(typeof (WSLookupTable.AccessType)).Length;
                for (int i = 0; i < aNumAccessTypes; i++)
                {
                    aNewAccessLevel.mAccess[i] = mAccess[i];
                    aNewAccessLevel.mSeqLevel[i] = mSeqLevel[i];
                }

                return aNewAccessLevel;
            }
        }

        /// <summary>
        /// Defines complete security information for a single page in the Content Management System
        /// </summary>
        public class AccessInfo
        {
            /// <summary>
            /// The page ID for which the <c>AccessInfo</c> is defined
            /// </summary>
            protected int mPageID;

            /// <summary>
            /// The list of <c>AccessLevel</c> objects containing the security information for this context
            /// </summary>
            protected ArrayList mAccessList = new ArrayList();

            /// <summary>
            /// Constructor creates the security information view for the page
            /// </summary>
            /// <param name="thePageID">The page ID for which to define security</param>
            public AccessInfo(int thePageID)
            {
                mPageID = thePageID;
                if (thePageID > -1)
                {
                    WSDBConnection aConn = new WSDBConnection();
                    if (aConn.Open("INTRANET"))
                    {
                        PopulateAccessList(aConn);
                        aConn.Close();
                    }
                }
            }

            /// <summary>
            /// Public access method returns the AccessList
            /// <remarks>
            /// Probably not entirely kosher, but the Content Management System needs to manipulate the data directly.
            /// </remarks>
            /// </summary>
            /// <returns></returns>
            public ArrayList GetAccessList()
            {
                return mAccessList;
            }

            private void PopulateAccessList(WSDBConnection theConnection)
            {
                int aPageID = mPageID;
                ArrayList aReaderList = new ArrayList();

                // make a list of readers, travelling up to the parent, then traverse those lists in reverse
                do
                {
                    string aQuery = String.Format(@"
						SELECT		
						DISTINCT	 ca.entity_key
									,ca.nt_user_id
									,ca.resolved_entity_key
									,cltype.lookup_code		type_lookup_code
									,claccess.lookup_code	access_lookup_code
									,ca.include_flag
									,ca.sequence_level
						FROM		 v_cms_access	ca
									,cms_lookup		cltype
									,cms_lookup		claccess
									,cms_page		cp
						WHERE		 ca.page_id = {0}
						AND			 ca.entity_type = cltype.lookup_id
						AND			 ca.access_type = claccess.lookup_id
						ORDER BY	 ca.resolved_entity_key
									,ca.include_flag"
                                                  // we want to process excludes first to be safe
                                                  , aPageID);


                    OracleDataReader anAccessReader = theConnection.GetReader(aQuery);

                    aQuery = String.Format(@"
						SELECT		parent_id
						FROM		cms_page
						WHERE		page_id = {0}", aPageID);

                    OracleDataReader aParentReader = theConnection.GetReader(aQuery);


                    if (aParentReader.Read() && aParentReader["parent_id"] != DBNull.Value)
                        aPageID = Convert.ToInt32(aParentReader["parent_id"]);
                    else aPageID = -1;

                    aReaderList.Insert(0, anAccessReader);

                } while (aPageID > -1);

                foreach (OracleDataReader anAccessReader in aReaderList)
                {
                    while (anAccessReader.Read())
                    {
                        string anEntity = anAccessReader["entity_key"].ToString();
                        string anEntityNTName = anAccessReader["nt_user_id"].ToString();
                        string anEntityName = anAccessReader["resolved_entity_key"].ToString();
                        WSLookupTable.EntityType aEntityType =
                            WSLookupTable.WSLookup.GetEntityType(anAccessReader["type_lookup_code"].ToString());
                        WSLookupTable.AccessType anAccessType =
                            WSLookupTable.WSLookup.GetAccessType(anAccessReader["access_lookup_code"].ToString());
                        int aSeqLevel = Convert.ToInt32(anAccessReader["sequence_level"]);
                        if (anAccessReader["include_flag"].ToString().ToUpper() == "I")
                            AddAccess(anEntity, anEntityNTName, anEntityName, aEntityType, anAccessType, aSeqLevel,
                                      mPageID);
                        else if (anAccessReader["include_flag"].ToString().ToUpper() == "E")
                            RemoveAccess(anEntity, aEntityType, anAccessType, aSeqLevel, mPageID);
                    }

                    anAccessReader.Close();
                }
            }

            /// <summary>
            /// Adds an access level security definition to the current <c>AccessInfo</c>
            /// </summary>
            /// <param name="theEntity">The entity key to which to assign access</param>
            /// <param name="theEntityNTName">The entity NT name corresponding to the entity key</param>
            /// <param name="theEntityName">The expanded entity name corresponding to the entity key</param>
            /// <param name="theType">The entity type</param>
            /// <param name="theAccessCode">The access type (one access type must be originally assigned)</param>
            /// <param name="theSeqLevel">The sequence level corresponding to the access type</param>
            /// <param name="thePageID">The page ID to which access is to be assigned</param>
            protected void AddAccess(string theEntity, string theEntityNTName, string theEntityName,
                                     WSLookupTable.EntityType theType, WSLookupTable.AccessType theAccessCode,
                                     int theSeqLevel, int thePageID)
            {
                bool found = false;

                foreach (AccessLevel aLevel in mAccessList)
                {
                    if (aLevel.mEntity == theEntity && aLevel.mType == theType && aLevel.mPageID == thePageID)
                    {
                        aLevel.mAccess[(int) theAccessCode] = true;
                        aLevel.mSeqLevel[(int) theAccessCode] = theSeqLevel;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    mAccessList.Add(new AccessLevel(theEntity, theEntityNTName, theEntityName, theType, theAccessCode,
                                                    theSeqLevel, thePageID));
            }

            /// <summary>
            /// Removes a specific access level security definition from the current <c>AccessInfo</c>
            /// </summary>
            /// <param name="theEntity">The entity key to which to assign access</param>
            /// <param name="theType">The entity type</param>
            /// <param name="theAccessCode">The access type (one access type must be originally assigned)</param>
            /// <param name="theSeqLevel">The sequence level corresponding to the access type</param>
            /// <param name="thePageID">The page ID to which access is to be assigned</param>
            protected void RemoveAccess(string theEntity, WSLookupTable.EntityType theType,
                                        WSLookupTable.AccessType theAccessCode, int theSeqLevel, int thePageID)
            {
                ArrayList aRemoveList = new ArrayList();

                foreach (AccessLevel aLevel in mAccessList)
                {
                    if (aLevel.mEntity == theEntity && aLevel.mType == theType && aLevel.mPageID == thePageID &&
                        aLevel.mSeqLevel[(int) theAccessCode] == theSeqLevel)
                        aLevel.mAccess[(int) theAccessCode] = false;

                    bool hasRecords = false;
                    int aNumAccessTypes = Enum.GetNames(typeof (WSLookupTable.AccessType)).Length;
                    for (int i = 0; i < aNumAccessTypes; i++)
                        if (aLevel.mAccess[i])
                            hasRecords = true;
                    if (!hasRecords)
                        aRemoveList.Add(aLevel);
                }

                foreach (AccessLevel aLevel in aRemoveList)
                    mAccessList.Remove(aLevel);
            }

            /// <summary>
            /// Checks to determine if a user has a specific type of access to the page
            /// </summary>
            /// <param name="theEntityNTName">The NT name of the user whose credentials are to be checked</param>
            /// <param name="theAccessType">The access type to be checked</param>
            /// <param name="theEntityType">The entity type -- should always be ENTITY_OPEN or ENTITY_USER</param>
            /// <returns>Returns true if the user has access, else false</returns>
            public bool UserHasAccess(string theEntityNTName, WSLookupTable.AccessType theAccessType,
                                      WSLookupTable.EntityType theEntityType)
            {
                foreach (AccessLevel aLevel in mAccessList)
                {
                    if (aLevel.mType == WSLookupTable.EntityType.ENTITY_OPEN && aLevel.mAccess[(int) theAccessType])
                        // if everyone has the checked type of access, don't search for more advanced conditions
                        return true;
                    else if (aLevel.mEntityNTName == theEntityNTName &&
                             aLevel.mAccess[(int) WSLookupTable.AccessType.ACCESS_SUPERUSER] &&
                             theAccessType != WSLookupTable.AccessType.ACCESS_APPROVE)
                        // if superuser, don't even bother checking the other conditions
                        return true;
                    else if (aLevel.mType == theEntityType && aLevel.mEntityNTName == theEntityNTName &&
                             aLevel.mAccess[(int) theAccessType])
                        return true;
                }

                return false;
            }

            /// <summary>
            /// Checks to determine if a user explicitly has a specific type of access to the page
            /// <remarks>
            /// This is a low-level function used primarily by the CMS.
            /// The information returned by this function will not be useful under most circumstances.
            /// </remarks>
            /// </summary>
            /// <param name="theEntityNTName">The NT name of the user whose credentials are to be checked</param>
            /// <param name="theAccessType">The access type to be checked</param>
            /// <param name="theEntityType">The entity type -- should always be ENTITY_OPEN or ENTITY_USER</param>
            /// <returns>Returns true if the user has access, else false</returns>
            public bool UserHasExplicitAccess(string theEntityNTName, WSLookupTable.AccessType theAccessType,
                                              WSLookupTable.EntityType theEntityType)
            {
                foreach (AccessLevel aLevel in mAccessList)
                {
                    if (aLevel.mType == theEntityType && aLevel.mEntityNTName == theEntityNTName &&
                        aLevel.mAccess[(int) theAccessType])
                        return true;
                }

                return false;
            }

            /// <summary>
            /// Checks to determine if a user has a specific type of access to the page
            /// </summary>
            /// <param name="thePage">The <c>Page</c> that is being visited by the user whose credentials are to be checked</param>
            /// <param name="theAccessType">The access type to be checked</param>
            /// <returns>Returns true if the user has access, else false</returns>
            public bool UserHasAccess(Page thePage, WSLookupTable.AccessType theAccessType)
            {
                return UserHasAccess(NTLookup.GetNTUserName(thePage), theAccessType,
                                     WSLookupTable.EntityType.ENTITY_USER);
            }

            /// <summary>
            /// Gets the sequence number for a specific user-access pair on the page
            /// <remarks>
            /// Sequence number is used to differentiate between different levels of the same type of access
            /// </remarks>
            /// </summary>
            /// <param name="theEntityNTName">The NT name of the user whose credentials are to be checked</param>
            /// <param name="theAccessType">The access type to be checked</param>
            /// <param name="theEntityType">The entity type -- should always be ENTITY_OPEN or ENTITY_USER</param>
            /// <returns>Returns the sequence number for the given user-access pair on the page</returns>
            public int GetSeqNum(string theEntityNTName, WSLookupTable.AccessType theAccessType,
                                 WSLookupTable.EntityType theEntityType)
            {
                foreach (AccessLevel aLevel in mAccessList)
                {
                    if (aLevel.mType == theEntityType && aLevel.mEntityNTName == theEntityNTName &&
                        aLevel.mAccess[(int) theAccessType])
                        return aLevel.mSeqLevel[(int) theAccessType];
                }

                return -1;
            }
        }

        /// <summary>
        /// Defines complete security information for every page in the Content Management System
        /// </summary>
        public class CompleteAccessInfo : AccessInfo
        {
            /// <summary>
            /// <c>CompleteAccessInfo</c> constructor
            /// </summary>
            public CompleteAccessInfo()
                : base(-1)
            {
                PopulateAccessList();
            }

            /// <summary>
            /// Structure used by the class internally to populate the access list
            /// </summary>
            private struct ReaderStruct
            {
                public string mLookupKey;
                public string mNTName;
                public string mResolvedEntityKey;
                public string mTypeLookupCode;
                public string mAccessLookupCode;
                public string mIncludeFlag;
                public int mSeqLevel;
                public int mPageID;
            }

            private ArrayList mReaderStructList = new ArrayList();
            private Hashtable mParentMap = new Hashtable();
            private Hashtable mRightsMap = new Hashtable();

            private void PopulateAccessList()
            {
                WSDBConnection aConn = new WSDBConnection();
                if (aConn.Open("INTRANET"))
                {
                    string aQuery = @"
							SELECT		 ca.entity_key
										,ca.nt_user_id
										,ca.resolved_entity_key
										,cltype.lookup_code			type_lookup_code
										,claccess.lookup_code		access_lookup_code
										,ca.include_flag
										,cp.page_id
										,ca.sequence_level
							FROM		 v_cms_access	ca
										,cms_lookup		cltype
										,cms_lookup		claccess
										,cms_page		cp
							WHERE		 ca.page_id = cp.page_id (+)
							AND			 ca.entity_type = cltype.lookup_id
							AND			 ca.access_type = claccess.lookup_id
							ORDER BY	 ca.resolved_entity_key
										,ca.include_flag"; // we want to process excludes first to be safe

                    OracleDataReader aReader = aConn.GetReader(aQuery);
                    while (aReader.Read())
                    {
                        ReaderStruct aReaderStruct = new ReaderStruct();
                        aReaderStruct.mLookupKey = aReader["entity_key"].ToString();
                        aReaderStruct.mNTName = aReader["nt_user_id"].ToString();
                        aReaderStruct.mResolvedEntityKey = aReader["resolved_entity_key"].ToString();
                        aReaderStruct.mTypeLookupCode = aReader["type_lookup_code"].ToString();
                        aReaderStruct.mAccessLookupCode = aReader["access_lookup_code"].ToString();
                        aReaderStruct.mIncludeFlag = aReader["include_flag"].ToString();
                        aReaderStruct.mPageID = Convert.ToInt32(aReader["page_id"]);
                        aReaderStruct.mSeqLevel = Convert.ToInt32(aReader["sequence_level"]);
                        mReaderStructList.Add(aReaderStruct);
                    }

                    aReader.Close();

                    aQuery = @"
						SELECT		page_id, parent_id
						FROM		cms_page";

                    aReader = aConn.GetReader(aQuery);
                    while (aReader.Read())
                    {
                        mParentMap[Convert.ToInt32(aReader["page_id"])] = (aReader["parent_id"] != DBNull.Value)
                                                                              ? Convert.ToInt32(aReader["parent_id"])
                                                                              : -1;
                        mRightsMap[Convert.ToInt32(aReader["page_id"])] = false;
                    }

                    aReader.Close();

                    IDictionaryEnumerator anEnumerator = mParentMap.GetEnumerator();

                    while (anEnumerator.MoveNext()) // map each page's settings
                        SetAccess((int) anEnumerator.Key);

                    aConn.Close();
                }
            }

            private void SetAccess(int thePageID)
            {
                if (thePageID < 0 || (bool) mRightsMap[thePageID])
                    // we have already mapped this page's security settings
                    return;

                int aParentID = (int) mParentMap[thePageID];
                SetAccess(aParentID);

                // first copy the rights from the parent
                if (aParentID > 0)
                {
                    ArrayList aNewAccessList = new ArrayList();
                    foreach (AccessLevel aLevel in mAccessList)
                        if (aParentID == aLevel.mPageID)
                            aNewAccessList.Add(aLevel.Clone(thePageID));
                    foreach (AccessLevel aNewLevel in aNewAccessList)
                        mAccessList.Add(aNewLevel);
                }

                // then set the rights from the mReaderStructList info for this page
                foreach (ReaderStruct aReaderStruct in mReaderStructList)
                {
                    if (aReaderStruct.mPageID == thePageID)
                    {
                        WSLookupTable.EntityType aEntityType =
                            WSLookupTable.WSLookup.GetEntityType(aReaderStruct.mTypeLookupCode);
                        WSLookupTable.AccessType anAccessType =
                            WSLookupTable.WSLookup.GetAccessType(aReaderStruct.mAccessLookupCode);
                        if (aReaderStruct.mIncludeFlag.ToUpper() == "I")
                            AddAccess(aReaderStruct.mLookupKey, aReaderStruct.mNTName, aReaderStruct.mResolvedEntityKey,
                                      aEntityType, anAccessType, aReaderStruct.mSeqLevel, thePageID);
                        else if (aReaderStruct.mIncludeFlag.ToUpper() == "E")
                            RemoveAccess(aReaderStruct.mLookupKey, aEntityType, anAccessType, aReaderStruct.mSeqLevel,
                                         thePageID);
                    }
                }

                mRightsMap[thePageID] = true;
            }

            /// <summary>
            /// Function throws an exception due to lack of a PageID
            /// </summary>
            /// <param name="theEntity">The name of the entity whose credentials are to be checked</param>
            /// <param name="theAccessType">The access type to be checked</param>
            /// <returns>This function throws an exception prior to returning</returns>
            public bool UserHasAccess(string theEntity, WSLookupTable.AccessType theAccessType)
            {
                throw new Exception("CompleteAccessInfo must be passed a PageID");
            }

            /// <summary>
            /// Function throws an exception due to lack of a PageID
            /// </summary>
            /// <param name="thePage">The <c>Page</c> being visited by the entity whose credentials are to be checked</param>
            /// <param name="theAccessType">The access type to be checked</param>
            /// <returns>This function throws an exception prior to returning</returns>
            public new bool UserHasAccess(Page thePage, WSLookupTable.AccessType theAccessType)
            {
                throw new Exception("CompleteAccessInfo must be passed a PageID");
            }

            /// <summary>
            /// Checks to determine if a user has a specific type of access to the page
            /// </summary>
            /// <param name="theEntityNTName">The NT name of the user whose credentials are to be checked</param>
            /// <param name="theAccessType">The access type to be checked</param>
            /// <param name="theEntityType">The entity type -- should always be ENTITY_OPEN or ENTITY_USER</param>
            /// <param name="thePageID">The id of the page for which access is to be checked</param>
            /// <returns>Returns true if the user has access, else false</returns>
            public bool UserHasAccess(string theEntityNTName, WSLookupTable.AccessType theAccessType,
                                      WSLookupTable.EntityType theEntityType, int thePageID)
            {
                foreach (AccessLevel aLevel in mAccessList)
                {
                    if (aLevel.mPageID != thePageID)
                        continue;
                    if (aLevel.mType == WSLookupTable.EntityType.ENTITY_OPEN && aLevel.mAccess[(int) theAccessType])
                        // if everyone has the checked type of access, don't search for more advanced conditions
                        return true;
                    else if (aLevel.mEntityNTName == theEntityNTName &&
                             aLevel.mAccess[(int) WSLookupTable.AccessType.ACCESS_SUPERUSER] &&
                             theAccessType != WSLookupTable.AccessType.ACCESS_APPROVE)
                        // if superuser, don't even bother checking the other conditions
                        return true;
                    else if (aLevel.mType == theEntityType && aLevel.mEntityNTName == theEntityNTName &&
                             aLevel.mAccess[(int) theAccessType])
                        return true;
                }

                return false;
            }

            /// <summary>
            /// Checks to determine if a user has a specific type of access to the page
            /// </summary>
            /// <param name="thePage">The <c>Page</c> that is being visited by the user whose credentials are to be checked</param>
            /// <param name="theAccessType">The access type to be checked</param>
            /// <param name="thePageID">The id of the page for which access is to be checked</param>
            /// <returns>Returns true if the user has access, else false</returns>
            public bool UserHasAccess(Page thePage, WSLookupTable.AccessType theAccessType, int thePageID)
            {
                return UserHasAccess(NTLookup.GetNTUserName(thePage), theAccessType,
                                     WSLookupTable.EntityType.ENTITY_USER, thePageID);
            }

            /// <summary>
            /// Checks to determine if a user explicitly has a specific type of access to the page
            /// <remarks>
            /// This is a low-level function used primarily by the CMS.
            /// The information returned by this function will not be useful under most circumstances.
            /// </remarks>
            /// </summary>
            /// <param name="theEntityNTName">The NT name of the user whose credentials are to be checked</param>
            /// <param name="theAccessType">The access type to be checked</param>
            /// <param name="theEntityType">The entity type -- should always be ENTITY_OPEN or ENTITY_USER</param>
            /// <param name="thePageID">The id of the page for which access is to be checked</param>
            /// <returns>Returns true if the user has access, else false</returns>
            public bool UserHasExplicitAccess(string theEntityNTName, WSLookupTable.AccessType theAccessType,
                                              WSLookupTable.EntityType theEntityType, int thePageID)
            {
                foreach (AccessLevel aLevel in mAccessList)
                {
                    if (aLevel.mType == theEntityType && aLevel.mEntityNTName == theEntityNTName &&
                        aLevel.mAccess[(int) theAccessType])
                        return true;
                }

                return false;
            }

            /// <summary>
            /// Checks to determine if a user explicitly has a specific type of access to the page
            /// <remarks>
            /// This is a low-level function used primarily by the CMS.
            /// The information returned by this function will not be useful under most circumstances.
            /// </remarks>
            /// </summary>
            /// <param name="thePage">The <c>Page</c> that is being visited by the user whose credentials are to be checked</param>
            /// <param name="theAccessType">The access type to be checked</param>
            /// <param name="thePageID">The id of the page for which access is to be checked</param>
            /// <returns>Returns true if the user has access, else false</returns>
            public bool UserHasExplicitAccess(Page thePage, WSLookupTable.AccessType theAccessType, int thePageID)
            {
                return UserHasExplicitAccess(NTLookup.GetNTUserName(thePage), theAccessType,
                                             WSLookupTable.EntityType.ENTITY_USER, thePageID);
            }
        }
    }
}