using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileManagerLibrary.Base.UIData
{
    [Serializable()]
    public abstract class UIDataBase
    {
        protected string tostringvar;

        protected void AddValue(string name, object objvalue)
        {
            if (tostringvar != "")
                tostringvar += ", ";
            tostringvar += name + " = " + objvalue.ToString();
        }
    }
}
