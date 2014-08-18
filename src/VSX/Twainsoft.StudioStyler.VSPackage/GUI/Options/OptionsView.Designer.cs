namespace Twainsoft.StudioStyler.VSPackage.GUI.Options
{
    partial class OptionsView
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
            this.label1 = new System.Windows.Forms.Label();
            this.stylesPerPage = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.stylesPerPage)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(73, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // stylesPerPage
            // 
            this.stylesPerPage.Location = new System.Drawing.Point(153, 18);
            this.stylesPerPage.Name = "stylesPerPage";
            this.stylesPerPage.Size = new System.Drawing.Size(52, 20);
            this.stylesPerPage.TabIndex = 1;
            this.stylesPerPage.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.stylesPerPage.ValueChanged += new System.EventHandler(this.stylesPerPage_ValueChanged);
            // 
            // OptionsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stylesPerPage);
            this.Controls.Add(this.label1);
            this.Name = "OptionsView";
            this.Size = new System.Drawing.Size(433, 299);
            ((System.ComponentModel.ISupportInitialize)(this.stylesPerPage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown stylesPerPage;
    }
}
