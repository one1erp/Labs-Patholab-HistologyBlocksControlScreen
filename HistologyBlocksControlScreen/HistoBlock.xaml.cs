using LSExtensionWindowLib;
using LSSERVICEPROVIDERLib;
using Microsoft.Office.Interop.Excel;
using Oracle.ManagedDataAccess.Client;
using Patholab_Common;
using Patholab_DAL_V1;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;



namespace HistologyBlocksControlScreen
{
    /// <summary>
    /// Interaction logic for Cyto_screen.xaml
    /// </summary>
    public partial class Cyto_screen : UserControl
    {
        public Cyto_screen()
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
        private string connectrionString;
        string connectionString;
        OracleConnection oraCon;


        public Cyto_screen(INautilusServiceProvider sp, INautilusProcessXML xmlProcessor, INautilusDBConnection _ntlsCon,
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
        List<BlockRow> rows;
        public void Initilaize()
        {

            dal = new DataLayer();
            dal.Connect(_ntlsCon);
            _operator_id = (long)_ntlsUser.GetOperatorId();
            _session_id = _ntlsCon.GetSessionId();

            oraCon = GetConnection(_ntlsCon);
            SetDataGrid();            

        }
  
        public void SetDataGrid()
        {
            string query = $"select a.ALIQUOT_ID,au.u_patholab_aliquot_name name, au.U_ALIQUOT_STATION, au.U_OLD_ALIQUOT_STATION from lims_sys.aliquot a join lims_sys.aliquot_user au\r\non a.ALIQUOT_ID = au.ALIQUOT_ID\r\nwhere \r\nau.U_GLASS_TYPE = 'B' and a.status in('C','P','V','I')\r\nand a.name like 'B%'\r\nand au.U_LAST_LABORANT Changes according to the tfs= {_ntlsUser.GetOperatorId()} and au.U_ALIQUOT_STATION in ('30','45','60', '27', '40')";

            rows = new List<BlockRow>();


            try
            {
                using (OracleCommand cmd = new OracleCommand(query, oraCon))
                {
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                BlockRow block = new BlockRow
                                {
                                    ALIQUOT_ID = Convert.ToInt32(reader[0]),
                                    Name = (reader[1]).ToString(),
                                    U_ALIQUOT_STATION = reader[2].ToString(),
                                    U_OLD_ALIQUOT_STATION = reader[3].ToString(),
                                    ColorsVec = ReturnAliqStationColor(reader[2].ToString()),

                                };

                                rows.Add(block);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(2 +ex.Message);
                    }

                }


            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(1+ ex.Message);
            }



            dataGrid.ItemsSource = rows;



        }
      

        private bool UserFilter(object item)
        {
            try
            {
                if (String.IsNullOrEmpty(textBox1.Text))
                {

                    return true;
                }

                else
                    return ((item as BlockRow).Name.IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void textBoxScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
                view.Filter = UserFilter;
                CollectionViewSource.GetDefaultView(dataGrid.ItemsSource).Refresh();

            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)sender;
            textBox.Text = "";
            textBox.Foreground = System.Windows.Media.Brushes.Black;

        }
        public List<bool> ReturnAliqStationColor(string U_ALIQUOT_STATION)
        {
            List<bool> colors = new List<bool> { false, false, false };

            switch (U_ALIQUOT_STATION)
            {
                case "30":
                    {
                        colors[0] = true;
                        break;
                    }
                case "45":
                    {
                        colors[0] = true;
                        colors[1] = true;
                        break;
                    }
                case "60":
                    {
                        colors[0] = true;
                        colors[1] = true;
                        colors[2] = true;
                        break;
                    }
                default: {
                        colors[0] = false;
                        colors[1] = false;
                        colors[2] = false;
                        break;
                    }
            }
            return colors;  
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



    }
}

