using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sa.DB
{
    public enum enFieldStatus : byte
    {
        Initial_Value = 0,
        Original_Value = 1,
        Has_Changed = 2,
    }


    public class Field
    {
        //-------------------------------------------------------------------------------------------------------------
        public class StringField
        {
            enFieldStatus iFieldStatus = enFieldStatus.Initial_Value;
            string _FieldName = "";

            string _Default = "";
            string _OldValue = "";
            string _NewValue = "";

            //---------------------------------------------------------------------------------------------------------
            public StringField(string sFieldName)
            {
                _FieldName = sFieldName;
            }
            //---------------------------------------------------------------------------------------------------------
            public StringField(string sFieldName, string sValue)
            {
                _FieldName = sFieldName;
                _Default = sValue;
                _OldValue = _Default;
                _NewValue = _Default;
            }
            //---------------------------------------------------------------------------------------------------------
            public void Reset()
            {
                _OldValue = _Default;
                _NewValue = _Default;
                iFieldStatus = enFieldStatus.Initial_Value;
            }
            //---------------------------------------------------------------------------------------------------------
            public void SetAll(string sValue)
            {
                _OldValue = sValue;
                _NewValue = sValue;
                iFieldStatus = enFieldStatus.Original_Value;
            }
            //---------------------------------------------------------------------------------------------------------
            public void ModifyInfo(StringBuilder sbUpdateInfo)
            {
                if ((iFieldStatus == enFieldStatus.Has_Changed) && (_OldValue != _NewValue))
                    sbUpdateInfo.Append("[" + _FieldName + " = " + _OldValue + " -> " + _NewValue + "]");
            }
            //---------------------------------------------------------------------------------------------------------
            public string FieldName { get { return _FieldName; } }
            //---------------------------------------------------------------------------------------------------------
            public string OldValue { get { return _OldValue; } }
            //---------------------------------------------------------------------------------------------------------
            public string NewValue { get { return _NewValue; } }
            //---------------------------------------------------------------------------------------------------------
            public string Value
            {
                set
                {
                    switch (iFieldStatus)
                    {
                        case enFieldStatus.Initial_Value:
                            _OldValue = value;
                            iFieldStatus = enFieldStatus.Original_Value;
                            break;
                        case enFieldStatus.Original_Value:
                            _NewValue = value;
                            iFieldStatus = enFieldStatus.Has_Changed;
                            break;
                        case enFieldStatus.Has_Changed:
                            _NewValue = value;
                            break;
                    }
                }
                get
                {
                    string sRet = "";
                    if (iFieldStatus == enFieldStatus.Initial_Value) sRet = _OldValue;
                    if (iFieldStatus == enFieldStatus.Original_Value) sRet = _OldValue;
                    if (iFieldStatus == enFieldStatus.Has_Changed) sRet = _NewValue;
                    return sRet;
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public class Int32Field
        {
            enFieldStatus iFieldStatus = enFieldStatus.Initial_Value;
            string _FieldName = "";
            Int32 _Default = 0;
            Int32 _OldValue = 0;
            Int32 _NewValue = 0;

            //---------------------------------------------------------------------------------------------------------
            public Int32Field(string sFieldName)
            {
                _FieldName = sFieldName;
            }
            //---------------------------------------------------------------------------------------------------------
            public Int32Field(string sFieldName, Int32 iValue)
            {
                _FieldName = sFieldName;
                _Default = iValue;
                _OldValue = _Default;
                _NewValue = _Default;
            }
            //---------------------------------------------------------------------------------------------------------
            public void Reset()
            {
                _OldValue = _Default;
                _NewValue = _Default;
                iFieldStatus = enFieldStatus.Initial_Value;
            }
            //---------------------------------------------------------------------------------------------------------
            public void SetAll(Int32 iValue)
            {
                _OldValue = iValue;
                _NewValue = iValue;
                iFieldStatus = enFieldStatus.Has_Changed;
            }
            //---------------------------------------------------------------------------------------------------------
            public void ModifyInfo(StringBuilder sbUpdateInfo)
            {
                if ((iFieldStatus == enFieldStatus.Has_Changed) && (_OldValue != _NewValue))
                    sbUpdateInfo.Append("[" + _FieldName + " = " + _OldValue.ToString() + " -> " + _NewValue.ToString() + "]");
            }
            //---------------------------------------------------------------------------------------------------------
            public string FieldName { get { return _FieldName; } }
            //---------------------------------------------------------------------------------------------------------
            public Int32 OldValue { get { return _OldValue; } }
            //---------------------------------------------------------------------------------------------------------
            public Int32 NewValue { get { return _NewValue; } }

            //---------------------------------------------------------------------------------------------------------
            public Int32 Value
            {
                set
                {
                    switch (iFieldStatus)
                    {
                        case enFieldStatus.Initial_Value:
                            _OldValue = value;
                            iFieldStatus = enFieldStatus.Original_Value;
                            break;
                        case enFieldStatus.Original_Value:
                            _NewValue = value;
                            iFieldStatus = enFieldStatus.Has_Changed;
                            break;
                        case enFieldStatus.Has_Changed:
                            _NewValue = value;
                            break;
                    }
                }
                get
                {
                    Int32 iRet = 0;
                    if (iFieldStatus == enFieldStatus.Initial_Value) iRet = _OldValue;
                    if (iFieldStatus == enFieldStatus.Original_Value) iRet = _OldValue;
                    if (iFieldStatus == enFieldStatus.Has_Changed) iRet = _NewValue;
                    return iRet;
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public class BooleanField
        {
            enFieldStatus iFieldStatus = enFieldStatus.Initial_Value;
            string _FieldName = "";
            Boolean _Default = false;
            Boolean _OldValue = false;
            Boolean _NewValue = false;

            //---------------------------------------------------------------------------------------------------------
            public BooleanField(string sFieldName)
            {
                _FieldName = sFieldName;
            }
            //---------------------------------------------------------------------------------------------------------
            public BooleanField(string sFieldName, Boolean blVaiue)
            {
                _FieldName = sFieldName;
                _Default = blVaiue;
                _OldValue = _Default;
                _NewValue = _Default;
            }
            //---------------------------------------------------------------------------------------------------------
            public void Reset()
            {
                _OldValue = _Default;
                _NewValue = _Default;
                iFieldStatus = enFieldStatus.Initial_Value;
            }
            //---------------------------------------------------------------------------------------------------------
            public void SetAll(Boolean blValue)
            {
                _OldValue = blValue;
                _NewValue = blValue;
                iFieldStatus = enFieldStatus.Has_Changed;
            }
            //-------------------------------------------------------------------------------------------------------------
            public void ModifyInfo(StringBuilder sbUpdateInfo)
            {
                if ((iFieldStatus == enFieldStatus.Has_Changed) && (_OldValue != _NewValue))
                    sbUpdateInfo.Append("[" + _FieldName + " = " + _OldValue.ToString() + " -> " + _NewValue.ToString() + "]");
            }
            //---------------------------------------------------------------------------------------------------------
            public string FieldName { get { return _FieldName; } }
            //---------------------------------------------------------------------------------------------------------
            public Boolean OldValue { get { return _OldValue; } }
            //---------------------------------------------------------------------------------------------------------
            public Boolean NewValue { get { return _NewValue; } }
            //---------------------------------------------------------------------------------------------------------
            public Boolean Value
            {
                set
                {
                    switch (iFieldStatus)
                    {
                        case enFieldStatus.Initial_Value:
                            _OldValue = value;
                            iFieldStatus = enFieldStatus.Original_Value;
                            break;
                        case enFieldStatus.Original_Value:
                            _NewValue = value;
                            iFieldStatus = enFieldStatus.Has_Changed;
                            break;
                        case enFieldStatus.Has_Changed:
                            _NewValue = value;
                            break;
                    }
                }
                get
                {
                    Boolean blRet = false;
                    if (iFieldStatus == enFieldStatus.Initial_Value) blRet = _OldValue;
                    if (iFieldStatus == enFieldStatus.Original_Value) blRet = _OldValue;
                    if (iFieldStatus == enFieldStatus.Has_Changed) blRet = _NewValue;
                    return blRet;
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public class DateTimeField
        {
            enFieldStatus iFieldStatus = enFieldStatus.Initial_Value;
            string _FieldName = "";
            DateTime _Default = new DateTime(1980, 1, 1, 0, 0, 0);
            DateTime _OldValue = new DateTime(1980, 1, 1, 0, 0, 0);
            DateTime _NewValue = new DateTime(1980, 1, 1, 0, 0, 0);
            //---------------------------------------------------------------------------------------------------------
            public DateTimeField(string sFieldName)
            {
                _FieldName = sFieldName;
            }
            //---------------------------------------------------------------------------------------------------------
            public DateTimeField(string sFieldName, DateTime dtValue)
            {
                _FieldName = sFieldName;
                _Default = dtValue;
                _OldValue = _Default;
                _NewValue = _Default;
            }
            //---------------------------------------------------------------------------------------------------------
            public void Reset()
            {
                _OldValue = _Default;
                _NewValue = _Default;
                iFieldStatus = enFieldStatus.Initial_Value;
            }
            //---------------------------------------------------------------------------------------------------------
            public void SetAll(DateTime dtValue)
            {
                _OldValue = dtValue;
                _NewValue = dtValue;
                iFieldStatus = enFieldStatus.Has_Changed;
            }
            //-------------------------------------------------------------------------------------------------------------
            public void ModifyInfo(StringBuilder sbUpdateInfo)
            {
                if ((iFieldStatus == enFieldStatus.Has_Changed) && (_OldValue != _NewValue))
                    sbUpdateInfo.Append("[" + _FieldName + " = " + _OldValue.ToString() + " -> " + _NewValue.ToString() + "]");
            }
            //---------------------------------------------------------------------------------------------------------
            public string FieldName { get { return _FieldName; } }
            //---------------------------------------------------------------------------------------------------------
            public DateTime OldValue { get { return _OldValue; } }
            //---------------------------------------------------------------------------------------------------------
            public DateTime NewValue { get { return _NewValue; } }
            //---------------------------------------------------------------------------------------------------------
            public DateTime Value
            {
                set
                {
                    switch (iFieldStatus)
                    {
                        case enFieldStatus.Initial_Value:
                            _OldValue = value;
                            iFieldStatus = enFieldStatus.Original_Value;
                            break;
                        case enFieldStatus.Original_Value:
                            _NewValue = value;
                            iFieldStatus = enFieldStatus.Has_Changed;
                            break;
                        case enFieldStatus.Has_Changed:
                            _NewValue = value;
                            break;
                    }
                }
                get
                {
                    DateTime dtRet = new DateTime(1980, 1, 1, 0, 0, 0);
                    if (iFieldStatus == enFieldStatus.Initial_Value) dtRet = _OldValue;
                    if (iFieldStatus == enFieldStatus.Original_Value) dtRet = _OldValue;
                    if (iFieldStatus == enFieldStatus.Has_Changed) dtRet = _NewValue;
                    return dtRet;
                }
            }
        }
    }
}
