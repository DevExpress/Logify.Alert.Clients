namespace DevExpress.Logify.Win {
    partial class ConfirmReportSendForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent() {
            this.btnSend = new System.Windows.Forms.Button();
            this.btnDontSend = new System.Windows.Forms.Button();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.txtProblemDetails = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.pageComments = new System.Windows.Forms.TabPage();
            this.pageDetails = new System.Windows.Forms.TabPage();
            this.lblInfo = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.pageComments.SuspendLayout();
            this.pageDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(416, 426);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnDontSend
            // 
            this.btnDontSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDontSend.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDontSend.Location = new System.Drawing.Point(497, 426);
            this.btnDontSend.Name = "btnDontSend";
            this.btnDontSend.Size = new System.Drawing.Size(75, 23);
            this.btnDontSend.TabIndex = 5;
            this.btnDontSend.Text = "Don\'t Send";
            this.btnDontSend.UseVisualStyleBackColor = true;
            this.btnDontSend.Click += new System.EventHandler(this.btnDontSend_Click);
            // 
            // txtComments
            // 
            this.txtComments.AcceptsReturn = true;
            this.txtComments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtComments.Location = new System.Drawing.Point(6, 6);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtComments.Size = new System.Drawing.Size(540, 359);
            this.txtComments.TabIndex = 0;
            // 
            // txtProblemDetails
            // 
            this.txtProblemDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProblemDetails.Location = new System.Drawing.Point(6, 6);
            this.txtProblemDetails.Multiline = true;
            this.txtProblemDetails.Name = "txtProblemDetails";
            this.txtProblemDetails.ReadOnly = true;
            this.txtProblemDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtProblemDetails.Size = new System.Drawing.Size(540, 359);
            this.txtProblemDetails.TabIndex = 3;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.pageComments);
            this.tabControl.Controls.Add(this.pageDetails);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(560, 397);
            this.tabControl.TabIndex = 6;
            // 
            // pageComments
            // 
            this.pageComments.Controls.Add(this.txtComments);
            this.pageComments.Location = new System.Drawing.Point(4, 22);
            this.pageComments.Name = "pageComments";
            this.pageComments.Padding = new System.Windows.Forms.Padding(3);
            this.pageComments.Size = new System.Drawing.Size(552, 371);
            this.pageComments.TabIndex = 0;
            this.pageComments.Text = "Comments";
            this.pageComments.UseVisualStyleBackColor = true;
            // 
            // pageDetails
            // 
            this.pageDetails.Controls.Add(this.txtProblemDetails);
            this.pageDetails.Location = new System.Drawing.Point(4, 22);
            this.pageDetails.Name = "pageDetails";
            this.pageDetails.Padding = new System.Windows.Forms.Padding(3);
            this.pageDetails.Size = new System.Drawing.Size(552, 371);
            this.pageDetails.TabIndex = 1;
            this.pageDetails.Text = "Problem Details";
            this.pageDetails.UseVisualStyleBackColor = true;
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.Location = new System.Drawing.Point(12, 415);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(388, 34);
            this.lblInfo.TabIndex = 7;
            this.lblInfo.Text = "Your personal information will not be sent. You will not be contacted in response" +
    " to this report";
            // 
            // ConfirmReportSendForm
            // 
            this.AcceptButton = this.btnSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnDontSend;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnDontSend);
            this.Controls.Add(this.btnSend);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfirmReportSendForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Crash Report for";
            this.tabControl.ResumeLayout(false);
            this.pageComments.ResumeLayout(false);
            this.pageComments.PerformLayout();
            this.pageDetails.ResumeLayout(false);
            this.pageDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Button btnSend;
        protected System.Windows.Forms.Button btnDontSend;
        protected System.Windows.Forms.TextBox txtComments;
        protected System.Windows.Forms.TextBox txtProblemDetails;
        protected System.Windows.Forms.TabControl tabControl;
        protected System.Windows.Forms.TabPage pageComments;
        protected System.Windows.Forms.TabPage pageDetails;
        protected System.Windows.Forms.Label lblInfo;
    }
}