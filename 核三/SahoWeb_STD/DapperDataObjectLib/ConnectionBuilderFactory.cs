using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Configuration;
using System.IO;


namespace DapperDataObjectLib
{
    /// <summary>建立ConnectonBuilder的物件</summary>
    public class ConnectionBuilderFactory
    {
        private const string MsSql = "MsSql";
        private const string OleDb = "OleDb";
        private const string Oracle = "Oracle";
        private const string Sqlite = "Sqlite";

        public static IConnectionBuilder CreateConnectionBuilder(string _DbKind, string _DbConnection)
        {
            if (_DbKind.Equals(ConnectionBuilderFactory.MsSql))
            {
                return new SqlConnectionBuilder(_DbConnection);
            }
            else if (_DbKind.Equals(ConnectionBuilderFactory.Oracle))
            {
                return new OracleConnectionBuilder(_DbConnection);
            }
            else if (_DbKind.Equals(ConnectionBuilderFactory.OleDb))
            {
                return new OleConnectionBuilder(_DbConnection);
            }
            else if (_DbKind.Equals(ConnectionBuilderFactory.Sqlite))
            {
                return new SqliteConnectionBuilder(_DbConnection);
            }
            throw new ApplicationException(_DbKind);
        }



        public static IConnectionBuilder CreateConnectionBuilder()
        {

            string xmlPath = "";// ConfigurationSettings.AppSettings.Get("ConnectionStringsPath");

            String[] keys = ConfigurationManager.AppSettings.AllKeys;
            foreach (String key in keys)
            {
                if (key.IndexOf("ConnectionStringsPath") >= 0)
                {
                    xmlPath = ConfigurationManager.AppSettings[key];

                    if (File.Exists(xmlPath))
                        continue;
                }
            }
            return ConnectionBuilderFactory.CreateConnectionBuilder(xmlPath);
        }


        /// <summary>Creates DataObject via ConnectionStrings.xml.</summary>
        /// <exception cref="DatabaseNotSupportedException">When you uses the database that is not suppored by ConnectionBuilder</exception>
        /// <returns></returns>
        public static IConnectionBuilder CreateConnectionBuilder(String newXmlPath)
        {

            string xmlPath = newXmlPath;
            if (xmlPath == null || xmlPath.Length == 0)
            {
                xmlPath = ".\\ConnectionStrings.xml";
            }
            return ConnectionBuilderFactory.ReadXml(xmlPath);

        }




        /// <summary>由xml文件建立資料庫連線</summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        private static IConnectionBuilder ReadXml(String xmlPath)
        {
            string databaseName = "";
            string databaseKind = "";
            string connectionString = "";

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(xmlPath);
            }
            catch (XmlException ex)
            {
                throw new XmlException("ConnectionStrings.xml's format is incorrect. ", ex);
            }

            //parse xml
            ParseXml(doc, ref databaseName, ref databaseKind, ref connectionString);
            return CreateConnectionBuilder(databaseKind, connectionString);
            //if (databaseKind.Equals(ConnectionBuilderFactory.MsSql))
            //{
            //    //				log.Debug("create SqlConnectionBuilder, connectString="+connectionString);
            //    return new SqlConnectionBuilder(connectionString);
            //}
            throw new ApplicationException(databaseKind);
        }


          /// <summary> 透過xml文件取得資料提供者名稱、資料類型、連線字串</summary>
        /// <param name="doc"></param>
        /// <param name="databaseName"></param>
        /// <param name="databaseKind"></param>
        /// <param name="connectionString"></param>
        private static void ParseXml(XmlDocument doc, ref string databaseName, ref string databaseKind, ref string connectionString)
        {

            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                foreach (XmlNode node in rootNode.ChildNodes) //root node
                {
                    if (node.Name.Equals("ConnectionStrings"))
                    {
                        foreach (XmlNode node2 in node.ChildNodes)
                        {
                            if (node2.Name.Equals("Database")) //Database 
                            {
                                foreach (XmlAttribute attribute in node2.Attributes)
                                {
                                    if (attribute.Name.Equals("Name")) //Database.Name attribute
                                    {
                                        databaseName = attribute.Value;

                                    }
                                    else if (attribute.Name.Equals("Kind")) //Database.Kind attribute
                                    {
                                        databaseKind = attribute.Value;

                                    }
                                }

                                foreach (XmlNode node3 in node2)
                                {
                                    if (node3.Name.Equals("ConnectionString"))
                                    {
                                        connectionString = node3.InnerText;

                                    }
                                }




                            } //end if (node2.Name.Equals ("Database"))
                        }

                    }

                }
            }
        }//end method


    }
}
