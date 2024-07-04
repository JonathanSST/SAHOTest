using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperDataObjectLib
{
    public class Tools
    {
        public static bool TestConnString(ref string sConnString, string sDBName)
        {
            try
            {
                OrmDataObject odo = new DapperDataObjectLib.OrmDataObject(sDBName, sConnString);
                return odo.CheckConnection();
            }
            catch (Exception ex)
            {
                return false;
            }                        
            //return false;
        }
    }
}
