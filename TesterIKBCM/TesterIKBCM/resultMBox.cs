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
    public partial class resultMBox : Form
    {
        public resultMBox()
        {
            InitializeComponent();
        }
        public resultMBox(string result, Color c)
        {
            InitializeComponent();
            lblPass.Text = result;
            this.BackColor = c;
        }

        private void resultMBox_Load(object sender, EventArgs e)
        {
            try
            {
                lblPass.Text = AutoTest.resultBoxText;
                this.BackColor = AutoTest.resultBoxColor;

                lblresult.Text = AutoTest.AutoTestResult;
                lblresult2.Text = AutoTest.AutoTestResult2;
                lblresult3.Text = AutoTest.InputMsg;

            }

            catch (Exception ex)
            {

            }


        }

        private void lblPass_Click(object sender, EventArgs e)
        {
            lblresult.Visible = !lblresult.Visible;
            lblresult2.Visible = !lblresult2.Visible;
            lblresult3.Visible = !lblresult3.Visible;
            lblresult4.Visible = !lblresult4.Visible;

        }
    }
}
