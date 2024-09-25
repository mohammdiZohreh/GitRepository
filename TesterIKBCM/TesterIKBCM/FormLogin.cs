using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TesterIKBCM
{
    public partial class FormLogin : Form
    {
        public static FormLogin instance;
        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (comboUser.Text != "" && txtID.Text != "")
            {
                int UID = int.Parse(txtID.Text);
                string user = comboUser.Text;
                SettingHouse.UserName = user;
                SettingHouse.UserID = UID;

                bool res = DataBase.LoadUser();

                if (res && (user == SettingHouse.UserName) && UID == SettingHouse.UserID)
                {
                    //TabControlVisible(true);
                    lblInfo.Text = "????";
                    //panelLogin.Visible = false;
                    SettingHouse.loginLabel = $"Welcome {user}";
                    lblwelcom.Text = $"Welcome {user}";
                    lblwelcom.ForeColor = Color.SteelBlue;

                    LoginData.UserID = SettingHouse.UserID;
                    LoginData.UserName = SettingHouse.UserName;
                    LoginData.accessLevel = SettingHouse.accessLevel;
                    LoginData.IsLogin = true;

                    this.Close();

                }
                else
                {
                    lblwelcom.ForeColor = Color.Tomato;
                    lblInfo.Text = "Username Or UserID Is Incorect";
                    lblwelcom.Text = "Username Or UserID Is Incorect";
                    LoginData.IsLogin = false;

                }
            }
        }

        private void btnPassVisible_MouseHover(object sender, EventArgs e)
        {
            txtID.UseSystemPasswordChar = false;

        }

        private void btnPassVisible_MouseLeave(object sender, EventArgs e)
        {
            txtID.UseSystemPasswordChar = true;

        }
        private void LoadUsers()
        {
            comboUser.Items.Clear();
            cbUser.Items.Clear();

            bool res = DataBase.LoadAllUsers();
            if (res)
            {
                foreach (var s in DataBase.Users)
                {
                    comboUser.Items.Add(s);
                    cbUser.Items.Add(s);

                }

                comboUser.Text = DataBase.Users[0];
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            LoadUsers();
            picPanel.BringToFront();
            pictureBox2.BringToFront();
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!LoginData.IsLogin)
            {
                SettingHouse.accessLevel = 0;
                LoginData.accessLevel = 0;
                panelRegister.Visible = false;
            }
           
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void btnGoToMain_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            panelRegister.Visible = false;
            picPanel.Visible = true;
        }

        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // Enter key pressed
                btnLogin_Click(null, null);
            }
        }
    }
    //public class AutoClosingMessageBox
    //{
    //    System.Threading.Timer _timeoutTimer;
    //    string _caption;
    //    DialogResult _result;
    //    DialogResult _timerResult;
    //    AutoClosingMessageBox(string text, string caption, int timeout, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxButtons buttons = MessageBoxButtons.OK, DialogResult timerResult = DialogResult.None)
    //    {
    //        _caption = caption;
    //        _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
    //            null, timeout, System.Threading.Timeout.Infinite);
    //        _timerResult = timerResult;
    //        using (_timeoutTimer)
    //            _result = MessageBox.Show(text, caption, buttons, icon);
    //    }
    //    public static DialogResult Show(string text, string caption, int timeout, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxButtons buttons = MessageBoxButtons.OK, DialogResult timerResult = DialogResult.None)
    //    {
    //        return new AutoClosingMessageBox(text, caption, timeout, icon, buttons, timerResult)._result;
    //    }
    //    void OnTimerElapsed(object state)
    //    {
    //        IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
    //        if (mbWnd != IntPtr.Zero)
    //            SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
    //        _timeoutTimer.Dispose();
    //        _result = _timerResult;
    //    }
    //    const int WM_CLOSE = 0x0010;
    //    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    //    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    //    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    //    static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    //}
}
