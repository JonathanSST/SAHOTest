using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;


namespace MyDataObjectLib
{
    public class DBParameter
    {
        /// <summary>keyList</summary>
        private ArrayList ArrayKey = new ArrayList();


        /// <summary>valueList</summary>
        private ArrayList ArrayValue = new ArrayList();


        /// <summary>typeList</summary>
        private ArrayList ArrayType = new ArrayList();


        /// <summary>commandObject</summary>
        private IDbCommand command;
    }
}
