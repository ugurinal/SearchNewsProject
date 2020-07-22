namespace SearchNewsProject
{
    partial class ListItem
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelDate = new System.Windows.Forms.Label();
            this.labelAuthor = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelContent = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelDate
            // 
            this.labelDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDate.Location = new System.Drawing.Point(3, 169);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(120, 30);
            this.labelDate.TabIndex = 9;
            this.labelDate.Text = "HÜRRİYET";
            this.labelDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelAuthor
            // 
            this.labelAuthor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAuthor.Location = new System.Drawing.Point(3, 139);
            this.labelAuthor.Name = "labelAuthor";
            this.labelAuthor.Size = new System.Drawing.Size(120, 30);
            this.labelAuthor.TabIndex = 8;
            this.labelAuthor.Text = "HÜRRİYET";
            this.labelAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 16);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(120, 120);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // labelContent
            // 
            this.labelContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelContent.Location = new System.Drawing.Point(135, 55);
            this.labelContent.Name = "labelContent";
            this.labelContent.Size = new System.Drawing.Size(738, 126);
            this.labelContent.TabIndex = 7;
            this.labelContent.Text = "label1\r\na\r\na\r\na";
            // 
            // labelTitle
            // 
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.labelTitle.Location = new System.Drawing.Point(135, 25);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(738, 26);
            this.labelTitle.TabIndex = 6;
            this.labelTitle.TabStop = true;
            this.labelTitle.Text = "Title\r\n";
            this.labelTitle.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelTitle_LinkClicked);
            // 
            // ListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelDate);
            this.Controls.Add(this.labelAuthor);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelContent);
            this.Controls.Add(this.labelTitle);
            this.Name = "ListItem";
            this.Size = new System.Drawing.Size(893, 200);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Label labelAuthor;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelContent;
        private System.Windows.Forms.LinkLabel labelTitle;
    }
}
