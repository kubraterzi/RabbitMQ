
namespace RabbitMQEDevletPDF
{
    partial class documentCreaterForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.connectBtn = new System.Windows.Forms.Button();
            this.connectionStringTxt = new System.Windows.Forms.TextBox();
            this.connectionStringLbl = new System.Windows.Forms.Label();
            this.createPdfBtn = new System.Windows.Forms.Button();
            this.logInfotxt = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.connectBtn);
            this.groupBox1.Controls.Add(this.connectionStringTxt);
            this.groupBox1.Controls.Add(this.connectionStringLbl);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(591, 71);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(506, 26);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 2;
            this.connectBtn.Text = "Connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // connectionStringTxt
            // 
            this.connectionStringTxt.Location = new System.Drawing.Point(124, 27);
            this.connectionStringTxt.Name = "connectionStringTxt";
            this.connectionStringTxt.Size = new System.Drawing.Size(364, 23);
            this.connectionStringTxt.TabIndex = 1;
            this.connectionStringTxt.Text = "amqps://gbnmnylz:KzHtnsE1Fhwa6r9_uO-dbcGxjTVApuUu@clam.rmq.cloudamqp.com/gbnmnylz" +
    "";
            // 
            // connectionStringLbl
            // 
            this.connectionStringLbl.AutoSize = true;
            this.connectionStringLbl.Location = new System.Drawing.Point(9, 30);
            this.connectionStringLbl.Name = "connectionStringLbl";
            this.connectionStringLbl.Size = new System.Drawing.Size(109, 15);
            this.connectionStringLbl.TabIndex = 0;
            this.connectionStringLbl.Text = "Connection String :";
            // 
            // createPdfBtn
            // 
            this.createPdfBtn.Location = new System.Drawing.Point(245, 98);
            this.createPdfBtn.Name = "createPdfBtn";
            this.createPdfBtn.Size = new System.Drawing.Size(145, 30);
            this.createPdfBtn.TabIndex = 1;
            this.createPdfBtn.Text = "Create PDF Document";
            this.createPdfBtn.UseVisualStyleBackColor = true;
            this.createPdfBtn.Click += new System.EventHandler(this.createPdfBtn_Click);
            // 
            // logInfotxt
            // 
            this.logInfotxt.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.logInfotxt.Location = new System.Drawing.Point(12, 168);
            this.logInfotxt.Multiline = true;
            this.logInfotxt.Name = "logInfotxt";
            this.logInfotxt.Size = new System.Drawing.Size(581, 200);
            this.logInfotxt.TabIndex = 2;
            // 
            // documentCreaterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 380);
            this.Controls.Add(this.logInfotxt);
            this.Controls.Add(this.createPdfBtn);
            this.Controls.Add(this.groupBox1);
            this.Name = "documentCreaterForm";
            this.Text = "EDevlet - Document Creater";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.TextBox connectionStringTxt;
        private System.Windows.Forms.Label connectionStringLbl;
        private System.Windows.Forms.Button createPdfBtn;
        private System.Windows.Forms.TextBox logInfotxt;
    }
}

