using System;
using System.Data;

namespace ProcapitaBoUExport
{
    static class DataTableFactory
    {
        public static DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("personnr", typeof(string)));
            dt.Columns.Add(new DataColumn("fornamn", typeof(string)));
            dt.Columns.Add(new DataColumn("efternamn", typeof(string)));
            dt.Columns.Add(new DataColumn("fullnamn", typeof(string)));
            dt.Columns.Add(new DataColumn("roll", typeof(string)));
            dt.Columns.Add(new DataColumn("rektorsomrade", typeof(string)));
            dt.Columns.Add(new DataColumn("rektorsomradeid", typeof(string)));
            dt.Columns.Add(new DataColumn("enhet", typeof(string)));
            dt.Columns.Add(new DataColumn("enhetid", typeof(string)));
            dt.Columns.Add(new DataColumn("skolform", typeof(string)));
            dt.Columns.Add(new DataColumn("skolformid", typeof(string)));
            dt.Columns.Add(new DataColumn("skolformkort", typeof(string)));
            dt.Columns.Add(new DataColumn("avdelning", typeof(string)));
            dt.Columns.Add(new DataColumn("avdelningid", typeof(string)));
            dt.Columns.Add(new DataColumn("klass", typeof(string)));
            dt.Columns.Add(new DataColumn("klassid", typeof(string)));
            dt.Columns.Add(new DataColumn("klassperiod", typeof(string)));
            dt.Columns.Add(new DataColumn("grupp", typeof(string)));
            dt.Columns.Add(new DataColumn("gruppid", typeof(string)));
            dt.Columns.Add(new DataColumn("grupperiod", typeof(string)));
            dt.Columns.Add(new DataColumn("amne", typeof(string)));
            dt.Columns.Add(new DataColumn("amneid", typeof(string)));
            dt.Columns.Add(new DataColumn("arskurs", typeof(string)));
            dt.Columns.Add(new DataColumn("gatuadress", typeof(string)));
            dt.Columns.Add(new DataColumn("postnummer", typeof(string)));
            dt.Columns.Add(new DataColumn("postort", typeof(string)));
            dt.Columns.Add(new DataColumn("vh1", typeof(string)));
            dt.Columns.Add(new DataColumn("vh2", typeof(string)));
            dt.Columns.Add(new DataColumn("uttagsdatum", typeof(DateTime)));
            return dt;
        }
    }
}
