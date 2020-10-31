namespace FTPServer
{
    partial class Form1
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
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.buttonListen = new System.Windows.Forms.Button();
            this.textBoxFolder = new System.Windows.Forms.TextBox();
            this.SelectFolder = new System.Windows.Forms.Button();
            this.listBoxClient = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(8, 8);
            this.textBoxIP.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(119, 22);
            this.textBoxIP.TabIndex = 0;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(130, 8);
            this.textBoxPort.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(42, 22);
            this.textBoxPort.TabIndex = 1;
            // 
            // buttonListen
            // 
            this.buttonListen.Location = new System.Drawing.Point(177, 8);
            this.buttonListen.Margin = new System.Windows.Forms.Padding(2);
            this.buttonListen.Name = "buttonListen";
            this.buttonListen.Size = new System.Drawing.Size(45, 20);
            this.buttonListen.TabIndex = 2;
            this.buttonListen.Text = "Listen";
            this.buttonListen.UseVisualStyleBackColor = true;
            this.buttonListen.Click += new System.EventHandler(this.buttonListen_Click);
            // 
            // textBoxFolder
            // 
            this.textBoxFolder.Location = new System.Drawing.Point(8, 35);
            this.textBoxFolder.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxFolder.Name = "textBoxFolder";
            this.textBoxFolder.Size = new System.Drawing.Size(163, 22);
            this.textBoxFolder.TabIndex = 3;
            // 
            // SelectFolder
            // 
            this.SelectFolder.Location = new System.Drawing.Point(178, 33);
            this.SelectFolder.Margin = new System.Windows.Forms.Padding(2);
            this.SelectFolder.Name = "SelectFolder";
            this.SelectFolder.Size = new System.Drawing.Size(84, 22);
            this.SelectFolder.TabIndex = 4;
            this.SelectFolder.Text = "Select Folder";
            this.SelectFolder.UseVisualStyleBackColor = true;
            this.SelectFolder.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBoxClient
            // 
            this.listBoxClient.FormattingEnabled = true;
            this.listBoxClient.ItemHeight = 12;
            this.listBoxClient.Location = new System.Drawing.Point(8, 71);
            this.listBoxClient.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxClient.Name = "listBoxClient";
            this.listBoxClient.Size = new System.Drawing.Size(241, 172);
            this.listBoxClient.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 58);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "Connected Client";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 252);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxClient);
            this.Controls.Add(this.SelectFolder);
            this.Controls.Add(this.textBoxFolder);
            this.Controls.Add(this.buttonListen);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.textBoxIP);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "FTPServer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Button buttonListen;
        private System.Windows.Forms.TextBox textBoxFolder;
        private System.Windows.Forms.Button SelectFolder;
        private System.Windows.Forms.ListBox listBoxClient;
        private System.Windows.Forms.Label label1;
    }
}

