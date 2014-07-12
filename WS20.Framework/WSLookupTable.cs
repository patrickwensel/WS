namespace WS20.Framework
{
    public class WSLookupTable
    {
        /// <summary>
        /// Enumerated values of different security entity types
        /// </summary>
        public enum EntityType
        {
            ENTITY_USER,
            ENTITY_GROUP,
            ENTITY_LOCATION,
            ENTITY_OPEN
        }

        /// <summary>
        /// Enumerated values of different security access types
        /// </summary>
        public enum AccessType : int
        {
            ACCESS_READ = 0,
            ACCESS_WRITE,
            ACCESS_APPROVE,
            ACCESS_PUBLISH,
            ACCESS_DELETE,
            ACCESS_PERMISSIONS,
            ACCESS_SUPERUSER
        }

        /// <summary>
        /// Provides lookup access functions to translate from db or other strings to enum values and back
        /// </summary>
        public class WSLookup
        {
            /// <summary>
            /// Translate from an entity type string code to an entity type enumerated value
            /// </summary>
            /// <param name="theTypeCode">The string to be matched to the enumerated value</param>
            /// <returns>The corresponding enumerated value</returns>
            public static EntityType GetEntityType(string theTypeCode)
            {
                EntityType aEntityType = EntityType.ENTITY_USER;

                switch (theTypeCode.ToUpper())
                {
                    case "U":
                    case "ENTITY_USER":
                        aEntityType = EntityType.ENTITY_USER;
                        break;
                    case "G":
                    case "ENTITY_GROUP":
                        aEntityType = EntityType.ENTITY_GROUP;
                        break;
                    case "L":
                    case "ENTITY_LOCATION":
                        aEntityType = EntityType.ENTITY_LOCATION;
                        break;
                    case "O":
                    case "ENTITY_OPEN":
                        aEntityType = EntityType.ENTITY_OPEN;
                        break;
                }

                return aEntityType;
            }

            /// <summary>
            /// Translate from an entity type enumerated value to an entity type string code
            /// </summary>
            /// <param name="theEntityType">The enumerated value to be matched to the string code</param>
            /// <returns>The corresponding string code</returns>
            public static string GetGroupCode(EntityType theEntityType)
            {
                string aTypeCode = "U";

                switch (theEntityType)
                {
                    case EntityType.ENTITY_USER:
                        aTypeCode = "U";
                        break;
                    case EntityType.ENTITY_GROUP:
                        aTypeCode = "G";
                        break;
                    case EntityType.ENTITY_LOCATION:
                        aTypeCode = "L";
                        break;
                    case EntityType.ENTITY_OPEN:
                        aTypeCode = "O";
                        break;
                }

                return aTypeCode;
            }

            /// <summary>
            /// Translate from an access type string code to an access type enumerated value
            /// </summary>
            /// <param name="theAccessCode">The string to be matched to the enumerated value</param>
            /// <returns>The corresponding enumerated value</returns>
            public static AccessType GetAccessType(string theAccessCode)
            {
                AccessType anAccessType = AccessType.ACCESS_READ;

                switch (theAccessCode.ToUpper())
                {
                    case "ARE":
                        anAccessType = AccessType.ACCESS_READ;
                        break;
                    case "AWR":
                        anAccessType = AccessType.ACCESS_WRITE;
                        break;
                    case "AAP":
                        anAccessType = AccessType.ACCESS_APPROVE;
                        break;
                    case "APU":
                        anAccessType = AccessType.ACCESS_PUBLISH;
                        break;
                    case "ADE":
                        anAccessType = AccessType.ACCESS_DELETE;
                        break;
                    case "APE":
                        anAccessType = AccessType.ACCESS_PERMISSIONS;
                        break;
                    case "ASU":
                        anAccessType = AccessType.ACCESS_SUPERUSER;
                        break;
                }

                return anAccessType;
            }

            /// <summary>
            /// Translate from an access type enumerated value to an access type string code
            /// </summary>
            /// <param name="theAccessType">The enumerated value to be matched to the string code</param>
            /// <returns>The corresponding string code</returns>
            public static string GetAccessCode(AccessType theAccessType)
            {
                string anAccessCode = "ARE";

                switch (theAccessType)
                {
                    case AccessType.ACCESS_READ:
                        anAccessCode = "ARE";
                        break;
                    case AccessType.ACCESS_WRITE:
                        anAccessCode = "AWR";
                        break;
                    case AccessType.ACCESS_APPROVE:
                        anAccessCode = "AAP";
                        break;
                    case AccessType.ACCESS_PUBLISH:
                        anAccessCode = "APU";
                        break;
                    case AccessType.ACCESS_DELETE:
                        anAccessCode = "ADE";
                        break;
                    case AccessType.ACCESS_PERMISSIONS:
                        anAccessCode = "APE";
                        break;
                    case AccessType.ACCESS_SUPERUSER:
                        anAccessCode = "ASU";
                        break;
                }

                return anAccessCode;
            }
        }
    }
}
