using LSExtensionWindowLib;
using LSSERVICEPROVIDERLib;
using Oracle.ManagedDataAccess.Client;
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
using FireEventV2;
using System.Windows.Forms.Integration;
using LaborantBarcodingStaion;

namespace HistologyBlocksControlScreen
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : UserControl
    {
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
        private string connectrionString;
        string connectionString;
        OracleConnection oraCon;
        List<BlockRow> rows;

        public MainScreen()
        {

        }
        public MainScreen(INautilusServiceProvider sp, INautilusProcessXML xmlProcessor, INautilusDBConnection _ntlsCon,
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

        public void Initilaize()
        {

            dal = new DataLayer();
            dal.Connect(_ntlsCon);
            _operator_id = (long)_ntlsUser.GetOperatorId();
            _session_id = _ntlsCon.GetSessionId();

            oraCon = GetConnection(_ntlsCon);

            Cyto_screen cyto_Screen = new Cyto_screen(sp, xmlProcessor, _ntlsCon, _ntlsSite, _ntlsUser);
            cyto_Screen.Initilaize();
            hostGrid.Children.Add(cyto_Screen);

        }
        public OracleConnection GetConnection(INautilusDBConnection ntlsCon)
        {

            OracleConnection connection = null;

            if (ntlsCon != null)
            {


                // Initialize variables
                String roleCommand;
                // Try/Catch block
                try
                {


                    var C = ntlsCon.GetServerIsProxy();
                    var C2 = ntlsCon.GetServerName();
                    var C4 = ntlsCon.GetServerType();

                    var C6 = ntlsCon.GetServerExtra();

                    var C8 = ntlsCon.GetPassword();
                    var C9 = ntlsCon.GetLimsUserPwd();
                    var C10 = ntlsCon.GetServerIsProxy();
                    var DD = _ntlsSite;




                    var u = _ntlsUser.GetOperatorName();
                    var u1 = _ntlsUser.GetWorkstationName();



                    string _connectionString = ntlsCon.GetADOConnectionString();

                    var splited = _connectionString.Split(';');

                    var cs = "";

                    for (int i = 1; i < splited.Count(); i++)
                    {
                        cs += splited[i] + ';';
                    }
                    //<<<<<<< .mine
                    var username = ntlsCon.GetUsername();
                    if (string.IsNullOrEmpty(username))
                    {
                        var serverDetails = ntlsCon.GetServerDetails();
                        cs = "User Id=/;Data Source=" + serverDetails + ";";
                    }


                    //Create the connection
                    connection = new OracleConnection(cs);



                    // Open the connection
                    connection.Open();

                    // Get lims user password
                    string limsUserPassword = ntlsCon.GetLimsUserPwd();

                    // Set role lims user
                    if (limsUserPassword == "")
                    {
                        // LIMS_USER is not password protected
                        roleCommand = "set role lims_user";
                    }
                    else
                    {
                        // LIMS_USER is password protected.
                        roleCommand = "set role lims_user identified by " + limsUserPassword;
                    }

                    // set the Oracle user for this connecition
                    OracleCommand command = new OracleCommand(roleCommand, connection);

                    // Try/Catch block
                    try
                    {
                        // Execute the command
                        command.ExecuteNonQuery();
                    }
                    catch (Exception f)
                    {
                        // Throw the exception
                        throw new Exception("Inconsistent role Security : " + f.Message);
                    }

                    // Get the session id
                    _session_id = ntlsCon.GetSessionId();

                    // Connect to the same session
                    string sSql = string.Format("call lims.lims_env.connect_same_session({0})", _session_id);

                    // Build the command
                    command = new OracleCommand(sSql, connection);

                    // Execute the command
                    command.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    // Throw the exception
                    throw e;
                }

                // Return the connection
            }

            return connection;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            OpenNewScreen(clickedButton.Content.ToString());
        }

        private void OpenNewScreen(string sentFrom)
        {
            hostGrid.Children.Clear();
            switch (sentFrom)
            {
                case "מסך בקרה":
                    {
                        Cyto_screen cyto_Screen = new Cyto_screen(sp, xmlProcessor, _ntlsCon, _ntlsSite, _ntlsUser);
                        cyto_Screen.Initilaize();
                        hostGrid.Children.Add(cyto_Screen);
                        break;
                    }
                case "מסך חיתוך":
                    {
                        try
                        {
                            LaborantBarcoding laborant = new LaborantBarcoding(sp, xmlProcessor, _ntlsCon, _ntlsSite, _ntlsUser, 300000);
                            laborant.InitializeData();
                            hostGrid.Children.Add(laborant);
                            break;
                        }
                        catch (Exception e)
                        {
                            break;
                        }


                    }
                case "מסך טרימינג":
                    {
                        FireEventCls v1 = new FireEventCls(_ntlsSite, sp, "TrimmingScreen");
                        WindowsFormsHost windowsFormsHost = new WindowsFormsHost();
                        windowsFormsHost.Child = v1;
                        hostGrid.Children.Add(windowsFormsHost);
                        break;
                    }
                case "מסך שיקוע":
                    {
                        System.Windows.Forms.MessageBox.Show("(הגישה למסך זה אפשרית רק מסרגל הכלים (זמנית");
                        break;
                    }

            }
        }


    }
}
