using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistologyBlocksControlScreen { 

    //public class AliqRow
    //{
    //    [DisplayName("מספר")]
    //    public string U_ALIQUOT_NAME { get; set; }

    //    [DisplayName("תחנה נוכחית")]
    //    public string U_ALIQUOT_STATION { get; set; }

    //    [DisplayName("תחנה קודמת")]
    //    public string U_OLD_ALIQUOT_STATION { get; set; }

    //    [DisplayName("בלוק/סלייד")]
    //    public string U_GLASS_TYPE { get; set; }

    //    [DisplayName("CELL BLOCK")]
    //    public string U_IS_CELL_BLOCK { get; set; }

    //    [DisplayName("לבורנט לבלוק")]
    //    public string? U_LAST_LABORANT { get; set; }


    //    [Browsable(false)]
    //    public string SDG_ID { get; set; }
    //    [Browsable(false)]
    //    public long ALIQUOT_ID { get; set; }
    //}

    //public class SDGsLogRowList
    //{
    //    [Browsable(false)]
    //    public long SDG_ID { get; set; }

    //    [DisplayName("מספר מקרה")]
    //    public string Name { get; set; }

    //    [DisplayName("שם מקרה")]
    //    public string U_Patholab_Number { get; set; }
    //    [Browsable(false)]
    //    public string? Last_Application_Code { get; set; }

    //    [DisplayName("פתולוג משויך")]
    //    public string? Patholog_Name { get; set; }

    //    [DisplayName("תחנה אחרונה")]
    //    public string? Info { get; set; }

    //    [Browsable(false)]
    //    public string? Entity { get; set; }

    //    [DisplayName("תאריך קבלה")]
    //    public DateTime? Received_On { get; set; }


    //}

    //public class SpecificSdgLogRow
    //{

    //    [Browsable(false)]
    //    public long SdgId { get; set; }

    //    [DisplayName("תאריך")]
    //    public DateTime? Time { get; set; }

    //    [DisplayName("תחנה")]
    //    public string Info { get; set; }

    //    [DisplayName("פירוט")]
    //    public string Description { get; set; }

    //}

    public class BlockRow
    {

        [Browsable(false)]
        public int ALIQUOT_ID { get; set; }

        [DisplayName("מספר בלוק")]
        public string Name { get; set; }

        [Browsable(false)]
        public string U_ALIQUOT_STATION { get; set; }

        [Browsable(false)]
        public string U_OLD_ALIQUOT_STATION { get; set; }

        [Browsable(false)]
        public List<bool> ColorsVec { get;  set; }
    }




}
