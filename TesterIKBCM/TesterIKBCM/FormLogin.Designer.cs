namespace TesterIKBCM
{
    partial class FormLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
            this.panelLogin = new System.Windows.Forms.Panel();
            this.btnPassVisible = new System.Windows.Forms.Button();
            this.comboUser = new System.Windows.Forms.ComboBox();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.txtID = new System.Windows.Forms.TextBox();
            this.lblwelcom = new System.Windows.Forms.Label();
            this.lblPass = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.panelRegister = new System.Windows.Forms.Panel();
            this.label58 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.cbUser = new System.Windows.Forms.ComboBox();
            this.accessLevelCb = new System.Windows.Forms.ComboBox();
            this.lblUser2 = new System.Windows.Forms.Label();
            this.tbConId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblPass2 = new System.Windows.Forms.Label();
            this.tbUserId = new System.Windows.Forms.TextBox();
            this.lblPass3 = new System.Windows.Forms.Label();
            this.picPanel = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnGoToMain = new System.Windows.Forms.Button();
            this.panelLogin.SuspendLayout();
            this.panelRegister.SuspendLayout();
            this.picPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelLogin
            // 
            this.panelLogin.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelLogin.Controls.Add(this.btnPassVisible);
            this.panelLogin.Controls.Add(this.comboUser);
            this.panelLogin.Controls.Add(this.btnAddUser);
            this.panelLogin.Controls.Add(this.btnLogin);
            this.panelLogin.Controls.Add(this.txtID);
            this.panelLogin.Controls.Add(this.lblwelcom);
            this.panelLogin.Controls.Add(this.lblPass);
            this.panelLogin.Controls.Add(this.lblUser);
            this.panelLogin.Controls.Add(this.label4);
            this.panelLogin.Controls.Add(this.lblInfo);
            this.panelLogin.Controls.Add(this.btnGoToMain);
            this.panelLogin.Location = new System.Drawing.Point(392, 4);
            this.panelLogin.Name = "panelLogin";
            this.panelLogin.Size = new System.Drawing.Size(406, 401);
            this.panelLogin.TabIndex = 2;
            // 
            // btnPassVisible
            // 
            this.btnPassVisible.Location = new System.Drawing.Point(332, 219);
            this.btnPassVisible.Name = "btnPassVisible";
            this.btnPassVisible.Size = new System.Drawing.Size(18, 19);
            this.btnPassVisible.TabIndex = 4;
            this.btnPassVisible.UseVisualStyleBackColor = true;
            this.btnPassVisible.MouseLeave += new System.EventHandler(this.btnPassVisible_MouseLeave);
            this.btnPassVisible.MouseHover += new System.EventHandler(this.btnPassVisible_MouseHover);
            // 
            // comboUser
            // 
            this.comboUser.FormattingEnabled = true;
            this.comboUser.Location = new System.Drawing.Point(193, 159);
            this.comboUser.Name = "comboUser";
            this.comboUser.Size = new System.Drawing.Size(121, 21);
            this.comboUser.TabIndex = 3;
            // 
            // btnAddUser
            // 
            this.btnAddUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.btnAddUser.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnAddUser.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddUser.Location = new System.Drawing.Point(193, 336);
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(99, 29);
            this.btnAddUser.TabIndex = 2;
            this.btnAddUser.Text = "    Add User";
            this.btnAddUser.UseVisualStyleBackColor = true;
            this.btnAddUser.Visible = false;
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(193, 218);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(124, 20);
            this.txtID.TabIndex = 1;
            this.txtID.Text = "1214";
            this.txtID.UseSystemPasswordChar = true;
            this.txtID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtID_KeyPress);
            // 
            // lblwelcom
            // 
            this.lblwelcom.AutoSize = true;
            this.lblwelcom.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblwelcom.ForeColor = System.Drawing.Color.SkyBlue;
            this.lblwelcom.Location = new System.Drawing.Point(107, 54);
            this.lblwelcom.Name = "lblwelcom";
            this.lblwelcom.Size = new System.Drawing.Size(197, 18);
            this.lblwelcom.TabIndex = 0;
            this.lblwelcom.Text = "Fill To Login K125 Tester";
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblPass.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblPass.Location = new System.Drawing.Point(71, 217);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(65, 18);
            this.lblPass.TabIndex = 0;
            this.lblPass.Text = "User ID";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblUser.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblUser.Location = new System.Drawing.Point(71, 158);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(88, 18);
            this.lblUser.TabIndex = 0;
            this.lblUser.Text = "UserName";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.label4.ForeColor = System.Drawing.Color.SteelBlue;
            this.label4.Location = new System.Drawing.Point(310, 336);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Go To Main";
            this.label4.Visible = false;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblInfo.ForeColor = System.Drawing.Color.Salmon;
            this.lblInfo.Location = new System.Drawing.Point(85, 408);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(12, 16);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = ".";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.label2.ForeColor = System.Drawing.Color.SteelBlue;
            this.label2.Location = new System.Drawing.Point(41, 334);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Add";
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.label59.ForeColor = System.Drawing.Color.SteelBlue;
            this.label59.Location = new System.Drawing.Point(248, 334);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(43, 15);
            this.label59.TabIndex = 8;
            this.label59.Text = "Delete";
            // 
            // panelRegister
            // 
            this.panelRegister.Controls.Add(this.label58);
            this.panelRegister.Controls.Add(this.label57);
            this.panelRegister.Controls.Add(this.btnLoad);
            this.panelRegister.Controls.Add(this.btnEdit);
            this.panelRegister.Controls.Add(this.label59);
            this.panelRegister.Controls.Add(this.cbUser);
            this.panelRegister.Controls.Add(this.btnDelete);
            this.panelRegister.Controls.Add(this.label2);
            this.panelRegister.Controls.Add(this.accessLevelCb);
            this.panelRegister.Controls.Add(this.btnClose);
            this.panelRegister.Controls.Add(this.btnRegister);
            this.panelRegister.Controls.Add(this.lblUser2);
            this.panelRegister.Controls.Add(this.tbConId);
            this.panelRegister.Controls.Add(this.label3);
            this.panelRegister.Controls.Add(this.lblPass2);
            this.panelRegister.Controls.Add(this.tbUserId);
            this.panelRegister.Controls.Add(this.lblPass3);
            this.panelRegister.Location = new System.Drawing.Point(53, 30);
            this.panelRegister.Name = "panelRegister";
            this.panelRegister.Size = new System.Drawing.Size(385, 372);
            this.panelRegister.TabIndex = 4;
            this.panelRegister.Visible = false;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.label58.ForeColor = System.Drawing.Color.SteelBlue;
            this.label58.Location = new System.Drawing.Point(171, 333);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(35, 15);
            this.label58.TabIndex = 16;
            this.label58.Text = "Load";
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.label57.ForeColor = System.Drawing.Color.SteelBlue;
            this.label57.Location = new System.Drawing.Point(106, 334);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(28, 15);
            this.label57.TabIndex = 17;
            this.label57.Text = "Edit";
            // 
            // cbUser
            // 
            this.cbUser.FormattingEnabled = true;
            this.cbUser.Location = new System.Drawing.Point(190, 93);
            this.cbUser.Name = "cbUser";
            this.cbUser.Size = new System.Drawing.Size(124, 21);
            this.cbUser.TabIndex = 13;
            // 
            // accessLevelCb
            // 
            this.accessLevelCb.FormattingEnabled = true;
            this.accessLevelCb.Items.AddRange(new object[] {
            "Manager",
            "Operator"});
            this.accessLevelCb.Location = new System.Drawing.Point(190, 234);
            this.accessLevelCb.Name = "accessLevelCb";
            this.accessLevelCb.Size = new System.Drawing.Size(124, 21);
            this.accessLevelCb.TabIndex = 12;
            // 
            // lblUser2
            // 
            this.lblUser2.AutoSize = true;
            this.lblUser2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblUser2.ForeColor = System.Drawing.Color.OliveDrab;
            this.lblUser2.Location = new System.Drawing.Point(28, 93);
            this.lblUser2.Name = "lblUser2";
            this.lblUser2.Size = new System.Drawing.Size(88, 18);
            this.lblUser2.TabIndex = 6;
            this.lblUser2.Text = "UserName";
            // 
            // tbConId
            // 
            this.tbConId.Location = new System.Drawing.Point(190, 188);
            this.tbConId.Name = "tbConId";
            this.tbConId.Size = new System.Drawing.Size(124, 20);
            this.tbConId.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.label3.ForeColor = System.Drawing.Color.OliveDrab;
            this.label3.Location = new System.Drawing.Point(28, 234);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 18);
            this.label3.TabIndex = 7;
            this.label3.Text = "Access Level";
            // 
            // lblPass2
            // 
            this.lblPass2.AutoSize = true;
            this.lblPass2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblPass2.ForeColor = System.Drawing.Color.OliveDrab;
            this.lblPass2.Location = new System.Drawing.Point(33, 143);
            this.lblPass2.Name = "lblPass2";
            this.lblPass2.Size = new System.Drawing.Size(65, 18);
            this.lblPass2.TabIndex = 8;
            this.lblPass2.Text = "User ID";
            // 
            // tbUserId
            // 
            this.tbUserId.Location = new System.Drawing.Point(190, 143);
            this.tbUserId.Name = "tbUserId";
            this.tbUserId.Size = new System.Drawing.Size(124, 20);
            this.tbUserId.TabIndex = 11;
            // 
            // lblPass3
            // 
            this.lblPass3.AutoSize = true;
            this.lblPass3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblPass3.ForeColor = System.Drawing.Color.OliveDrab;
            this.lblPass3.Location = new System.Drawing.Point(28, 189);
            this.lblPass3.Name = "lblPass3";
            this.lblPass3.Size = new System.Drawing.Size(89, 18);
            this.lblPass3.TabIndex = 9;
            this.lblPass3.Text = "Confirm ID";
            // 
            // picPanel
            // 
            this.picPanel.Controls.Add(this.pictureBox2);
            this.picPanel.Location = new System.Drawing.Point(14, 13);
            this.picPanel.Name = "picPanel";
            this.picPanel.Size = new System.Drawing.Size(372, 366);
            this.picPanel.TabIndex = 7;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::TesterIKBCM.Properties.Resources.IMG_1299;
            this.pictureBox2.Location = new System.Drawing.Point(5, 17);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(359, 302);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // btnLoad
            // 
            this.btnLoad.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnLoad.Image = global::TesterIKBCM.Properties.Resources.load;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(173, 297);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(29, 29);
            this.btnLoad.TabIndex = 14;
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            this.btnEdit.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnEdit.Image = global::TesterIKBCM.Properties.Resources.editUser;
            this.btnEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEdit.Location = new System.Drawing.Point(106, 298);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(29, 29);
            this.btnEdit.TabIndex = 15;
            this.btnEdit.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.ForeColor = System.Drawing.Color.Crimson;
            this.btnDelete.Image = global::TesterIKBCM.Properties.Resources.del2;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(251, 298);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(29, 29);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnClose.Image = global::TesterIKBCM.Properties.Resources.close2;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(351, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(31, 29);
            this.btnClose.TabIndex = 5;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRegister
            // 
            this.btnRegister.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnRegister.Image = global::TesterIKBCM.Properties.Resources.adduser;
            this.btnRegister.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRegister.Location = new System.Drawing.Point(40, 298);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(29, 29);
            this.btnRegister.TabIndex = 5;
            this.btnRegister.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(-4, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(442, 410);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // btnLogin
            // 
            this.btnLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.btnLogin.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnLogin.Image = global::TesterIKBCM.Properties.Resources.Picture5;
            this.btnLogin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogin.Location = new System.Drawing.Point(193, 298);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(99, 29);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "     Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnGoToMain
            // 
            this.btnGoToMain.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnGoToMain.Image = global::TesterIKBCM.Properties.Resources.indic2;
            this.btnGoToMain.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGoToMain.Location = new System.Drawing.Point(328, 298);
            this.btnGoToMain.Name = "btnGoToMain";
            this.btnGoToMain.Size = new System.Drawing.Size(29, 29);
            this.btnGoToMain.TabIndex = 5;
            this.btnGoToMain.UseVisualStyleBackColor = true;
            this.btnGoToMain.Visible = false;
            this.btnGoToMain.Click += new System.EventHandler(this.btnGoToMain_Click);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 409);
            this.Controls.Add(this.picPanel);
            this.Controls.Add(this.panelRegister);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panelLogin);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormLogin";
            this.Text = "Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLogin_FormClosing);
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.panelLogin.ResumeLayout(false);
            this.panelLogin.PerformLayout();
            this.panelRegister.ResumeLayout(false);
            this.panelRegister.PerformLayout();
            this.picPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLogin;
        private System.Windows.Forms.Button btnPassVisible;
        private System.Windows.Forms.ComboBox comboUser;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblwelcom;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Panel panelRegister;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.ComboBox cbUser;
        private System.Windows.Forms.ComboBox accessLevelCb;
        private System.Windows.Forms.Label lblUser2;
        private System.Windows.Forms.TextBox tbConId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblPass2;
        private System.Windows.Forms.TextBox tbUserId;
        private System.Windows.Forms.Label lblPass3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnGoToMain;
        private System.Windows.Forms.Button btnAddUser;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel picPanel;
    }
}