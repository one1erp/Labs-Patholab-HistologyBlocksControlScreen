using System;

using System.Collections.Generic;

using System.ComponentModel;

using System.Drawing;

using System.Data;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using LSExtensionWindowLib;

using LSSERVICEPROVIDERLib;

using Patholab_DAL_V1;

//using Oracle.ManagedDataAccess.Client;

using Patholab_Common;

using System.Runtime.InteropServices;

using System.Windows.Input;

using System.Windows.Forms;

using Telerik.WinControls.UI;

using Telerik.WinControls;
using System.Windows.Documents;

//using Telerik.WinControls.Data;



namespace HistologyBlocksControlScreen

{



    [ComVisible(true)]

    [ProgId("HistologyBlocksControlScreen.HistologyBlocksControlScreen")]

    public partial class HistologyBlocksControlScreen_host : UserControl, IExtensionWindow

    {

        #region Private members



        private INautilusProcessXML xmlProcessor;

        private INautilusUser _ntlsUser;

        private IExtensionWindowSite2 _ntlsSite;

        private INautilusServiceProvider sp;

        private INautilusDBConnection _ntlsCon;

        private DataLayer dal = null;

        long sid = 1;

        #endregion






        public HistologyBlocksControlScreen_host()

        {

            try

            {

                InitializeComponent();

                BackColor = Color.FromName("Control");

                this.Dock = DockStyle.Fill;

                this.AutoSize = true;

                this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

             

            }

            catch (Exception e)

            {

                MessageBox.Show(e.Message);

            }

        }






        #region implementing interface



        public bool CloseQuery()

        {

            DialogResult res = MessageBox.Show(@"?האם אתה בטוח שברצונך לצאת ", "HistologyBlocksControlScreen", MessageBoxButtons.YesNo);



            if (res == DialogResult.Yes)

            {

                if (dal != null)

                {

                    dal.Close();

                    dal = null;

                }

                if (_ntlsSite != null) _ntlsSite = null;



                //     if (connection != null) connection.Close();



                this.Dispose();



                return true;

            }

            else

            {

                return false;

            }

        }



        public WindowRefreshType DataChange()

        {

            return LSExtensionWindowLib.WindowRefreshType.windowRefreshNone;

        }



        public WindowButtonsType GetButtons()

        {

            return LSExtensionWindowLib.WindowButtonsType.windowButtonsNone;

        }


        public void Internationalise()

        {

        }


        public void PreDisplay()

        {

            xmlProcessor = Utils.GetXmlProcessor(sp);

            _ntlsUser = Utils.GetNautilusUser(sp);

            InitializeData();

        }


        private void InitializeData()
        {

            var w = new MainScreen(sp, xmlProcessor, _ntlsCon, _ntlsSite, _ntlsUser);
            elementHost1.Child = w;
            w.Initilaize();
            w.Focus();

        }


        public void RestoreSettings(int hKey)

        {

        }



        public bool SaveData()

        {

            return true;

        }



        public void SaveSettings(int hKey)

        {

        }



        public void SetParameters(string parameters)

        {

        }



        public void SetServiceProvider(object serviceProvider)

        {

            sp = serviceProvider as NautilusServiceProvider;

            _ntlsCon = Utils.GetNtlsCon(sp);

            this.sid = (long)_ntlsCon.GetSessionId();



        }



        public void SetSite(object site)

        {

            _ntlsSite = (IExtensionWindowSite2)site;

            _ntlsSite.SetWindowInternalName("מסך עבודה לבורנט");

            _ntlsSite.SetWindowRegistryName("מסך עבודה לבורנט");

            _ntlsSite.SetWindowTitle("מסך עבודה לבורנט");

        }



        public void Setup()

        {

        }



        public WindowRefreshType ViewRefresh()

        {

            return LSExtensionWindowLib.WindowRefreshType.windowRefreshNone;

        }



        public void refresh()

        {

        }



        #endregion



       





      

    





  



      








    }
}