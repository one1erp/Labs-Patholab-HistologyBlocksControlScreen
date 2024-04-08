using LSExtensionWindowLib;
using LSSERVICEPROVIDERLib;
using Patholab_DAL_V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.WinControls.UI;

namespace HistologyBlocksControlScreen
{
    /// <summary>
    /// Interaction logic for MainXml_userCtrl.xaml
    /// </summary>
    public partial class HistologyManagmentScreen : UserControl
    {
        public HistologyManagmentScreen()
        {
            InitializeComponent();
        }

        public INautilusServiceProvider ServiceProvider { get; set; }
        private INautilusProcessXML xmlProcessor;
        private INautilusUser _ntlsUser;
        private IExtensionWindowSite2 _ntlsSite;
        private INautilusServiceProvider sp;
        public INautilusDBConnection _ntlsCon;
        private DataLayer dal;
        public bool DEBUG;
        private List<PHRASE_ENTRY> RotherStatus;
        private SDG_USER sdg;
        private U_DEBIT_USER debit;
        private long? _operator_id;
        private double _session_id;


        public HistologyManagmentScreen(INautilusServiceProvider sp, INautilusProcessXML xmlProcessor, INautilusDBConnection _ntlsCon,
           IExtensionWindowSite2 _ntlsSite, INautilusUser _ntlsUser)
        {
            if (_ntlsUser.GetRoleName().ToUpper() == "DEBUG") Debugger.Launch();
            InitializeComponent();
            this.ServiceProvider = sp;
            this.sp = sp;
            this.xmlProcessor = xmlProcessor;
            this._ntlsCon = _ntlsCon;
            this._ntlsSite = _ntlsSite;
            this._ntlsUser = _ntlsUser;
            this.DataContext = this;
        }

        DataGrid_HistoBlock gridView;
        public void Initilaize()
        {
            dal = new DataLayer();

            dal.Connect(_ntlsCon);
            _operator_id = (long)_ntlsUser.GetOperatorId();
            _session_id = _ntlsCon.GetSessionId();


            //loading worker list
            List<PHRASE_ENTRY> basketList = dal.GetPhraseEntries("Basket").ToList();

            cmbBasket.ItemsSource = basketList;
            cmbBasket.DisplayMemberPath = "PHRASE_DESCRIPTION";
            cmbBasket.SelectedValuePath = "ORDER_NUMBER";

            List<OPERATOR> labWorkerList = dal.FindBy<OPERATOR>(x => x.ROLE_ID == 64 || x.ROLE_ID == 121).ToList();

            cmbLabWorkers.ItemsSource = labWorkerList;
            cmbLabWorkers.DisplayMemberPath = "FULL_NAME";
            cmbLabWorkers.SelectedValuePath = "OPERATOR_ID";

            gridView = new DataGrid_HistoBlock(_ntlsCon);
            winformsHostHistoBlockGridView.Child = gridView;

            DataGrid_HistoBlock userControlGridView = (DataGrid_HistoBlock)winformsHostHistoBlockGridView.Child;

            radGridView = userControlGridView.GridHistoBlock;
            if (radGridView == null)
            {
                return;
            }
            datalist = radGridView.DataSource as List<HistoBlockRow>;

        }



        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFilters())
            {
                System.Windows.Forms.MessageBox.Show("חובה לבחור סל ותאריך מאקרו");
                return;
            }


            DataGridFiltering();

        }
        RadGridView radGridView;
        List<HistoBlockRow> datalist;
        private void DataGridFiltering()
        {
            //Debugger.Launch();
            DateTime selectedDate = dt_Macro.SelectedDate ?? DateTime.MinValue;
            var filteredList = datalist.Where(x => x.block_basket == cmbBasket.SelectedValue.ToString() && x.block_macro_date!= null && x.block_macro_date.Value.Date == selectedDate.Date);


            radGridView.DataSource = null;
            radGridView.DataSource = filteredList;

        }

        private bool ValidateFilters()
        {
            return cmbBasket.SelectedItem != null && dt_Macro.SelectedDate != null;
        }

        private bool ValidateLabWorker()
        {
            return cmbLabWorkers.SelectedItem != null;
        }


        private void LabWorkerAssociation(long op_id, string op_name)
        {
            DataGrid_HistoBlock userControlGridView = (DataGrid_HistoBlock)winformsHostHistoBlockGridView.Child;

            RadGridView radGridView = userControlGridView.GridHistoBlock;
            if (radGridView == null)
            {
                return;
            }

            foreach (int index in userControlGridView.checkedRowIndexes)
            {

                GridViewRowInfo row = radGridView.Rows[index];
                HistoBlockRow histoB_row = row.DataBoundItem as HistoBlockRow;

                var currentAliq = dal.FindBy<ALIQUOT_USER>(x => x.ALIQUOT_ID == histoB_row.Aliquot_id).FirstOrDefault();
                currentAliq.U_LAST_LABORANT = op_id;
                dal.InsertToSdgLog(histoB_row.sdgId, "HS.BLOCK_ASO", (long)_ntlsCon.GetSessionId(), "associating a block to Labront" + histoB_row.block_name + op_name);
                row.Cells[4].Value = op_name;
                row.Cells[0].Value = false;
            }


            dal.SaveChanges();
            userControlGridView.ClearList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!ValidateLabWorker())
            {
                System.Windows.Forms.MessageBox.Show("חובה לבחור עובד לשיוך");
                return;
            }

            var selectedOperator = (OPERATOR)cmbLabWorkers.SelectedItem;
            LabWorkerAssociation(selectedOperator.OPERATOR_ID, selectedOperator.FULL_NAME);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            radGridView.DataSource = null;
            radGridView.DataSource = datalist;
        }
    }
}
